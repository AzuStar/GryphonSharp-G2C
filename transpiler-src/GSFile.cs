using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using GSharp.GSIL.GCode;
using GSharp.GSIL.GData;
using GSharp.GSIL.GScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GryphonSharpTranspiler
{
    internal class GSFile
    {
        public string FileName;
        public string NamespaceCode;
        public string NamespaceDirectory;
        /// <summary>
        /// Script's body
        /// </summary>
        public Script ScriptBody;

        public Dictionary<int, CodeExpression> MatchedExpressions = new Dictionary<int, CodeExpression>();

        public GSFile(GSProject project, string path)
        {
            // keep for debugging
            // ITraceWriter tracert = new MemoryTraceWriter();
            // JsonSerializerSettings settings = new JsonSerializerSettings(){
            //     TraceWriter = tracert,
            //     MissingMemberHandling = MissingMemberHandling.Ignore
            // };
            ScriptBody = JsonConvert.DeserializeObject<Script>(File.ReadAllText(path));
            if (ScriptBody == null)
            {
                Debug.Fail("File " + Path.GetRelativePath(project.src, path) + " in source folder could not be read: " + project.src);
                return;
            }
            ScriptBody.PostDeserialize();
            FileName = Path.GetFileNameWithoutExtension(path);
            NamespaceDirectory = Path.TrimEndingDirectorySeparator(Path.Combine(project.rootNamespace, Path.GetRelativePath(project.src, Path.GetDirectoryName(path))));
            NamespaceCode = NamespaceDirectory.Replace(Path.DirectorySeparatorChar, '.').Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, '.');
        }

        public String GenerateSource(CodeDomProvider provider)
        {
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            CodeNamespace ns = new CodeNamespace(NamespaceCode);
            codeUnit.Namespaces.Add(ns);

            CodeTypeDeclaration mainClass = new CodeTypeDeclaration(FileName);
            ns.Types.Add(mainClass);

            IEnumerable<KeyValuePair<int, GSharp.GSIL.GCode.Node>> funcEntries = ScriptBody.codeNodes.Where((kv) => kv.Value.type == GSharp.GSIL.GCode.Type.executionEnter);

            IEnumerable<KeyValuePair<int, GSharp.GSIL.GCode.Node>> codeEntry = funcEntries.Where((kv) => kv.Value.target == "Main"); // first doesnt work, dont touch

            funcEntries = funcEntries.Where((kv) => kv.Value.target != "Main");

            if (codeEntry.Any())
            {
                CodeEntryPointMethod entry = new CodeEntryPointMethod();

                GSharp.GSIL.GCode.Node currentNode = codeEntry.First().Value;

                FunctionBuilder(entry, currentNode);

                mainClass.Members.Add(entry);
            }

            foreach (KeyValuePair<int, GSharp.GSIL.GCode.Node> kv in funcEntries)
            {
                CodeMemberMethod func = new CodeMemberMethod();
                func.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                func.Name = kv.Value.target;
                FunctionBuilder(func, kv.Value);
                mainClass.Members.Add(func);
            }

            string srcText;
            using (StringWriter sw = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(codeUnit, sw, new CodeGeneratorOptions()
                {
                    IndentString = "   ",
                    VerbatimOrder = true,
                });
                srcText = sw.ToString();
            }
            return srcText;
        }

        private CodeParameterDeclarationExpression ResolveDataToParameterExpression(GSharp.GSIL.GData.Node dataNode)
        {
            if(dataNode.type==0) return null;
            // this can only be type 1
            CodeParameterDeclarationExpression cpd = new CodeParameterDeclarationExpression();
            cpd.Type = new CodeTypeReference(dataNode.vmType);
            cpd.Name = "auto_" + (uint)FileName.GetHashCode() + dataNode.id.ToString();
            return cpd;
        }
        private CodeExpression ResolveDataToCodeExpression(GSharp.GSIL.GData.Node dataNode)
        {
            CodeExpression ce = null;
            switch (dataNode.type)
            {
                case GSharp.GSIL.GData.Type.primitiveValue:
                    ce = new CodePrimitiveExpression(dataNode.value);
                    dataNode.vmType = dataNode.value.GetType();
                    break;
                case GSharp.GSIL.GData.Type.localValue:
                    if (MatchedExpressions.ContainsKey(dataNode.id))
                    {
                        ce = MatchedExpressions[dataNode.id];
                    }
                    else
                    {
                        ce = new CodeVariableReferenceExpression("auto_" + (uint)FileName.GetHashCode() + dataNode.id.ToString());
                    }
                    break;
                default:
                    Debug.Fail("DataTypeId: " + dataNode.type + " cannot be transformed.");
                    break;
            }

            return ce;
        }
        private GSharp.GSIL.GCode.Node FunctionBuilder(CodeMemberMethod func, GSharp.GSIL.GCode.Node firstNode)
        {
            GSharp.GSIL.GCode.Node currentNode = firstNode;
            while (true)
            {
                switch (currentNode.type)
                {
                    case GSharp.GSIL.GCode.Type.executionEnter:
                        // SECTION INCOMPLETE
                        if (currentNode.outputs == null) break;
                        foreach (int key in currentNode.outputs)
                        {
                            func.Parameters.Add(ResolveDataToParameterExpression(ScriptBody.dataNodes[key]));
                        }
                        break;
                    case GSharp.GSIL.GCode.Type.invokeOperatorCall:
                        CodeBinaryOperatorExpression cbo = new CodeBinaryOperatorExpression();
                        cbo.Operator = OperatorType.String2BinaryOperator(currentNode.target);
                        cbo.Left = ResolveDataToCodeExpression(ScriptBody.dataNodes[currentNode.inputs[0]]);
                        cbo.Right = ResolveDataToCodeExpression(ScriptBody.dataNodes[currentNode.inputs[1]]);
                        if (currentNode.inputs.Count > 2)
                        {
                            for (int i = 2; i < currentNode.inputs.Count; i++)
                            {
                                CodeBinaryOperatorExpression prevcbo = cbo;
                                cbo = new CodeBinaryOperatorExpression();
                                cbo.Operator = OperatorType.String2BinaryOperator(currentNode.target);
                                cbo.Left = prevcbo;
                                cbo.Right = ResolveDataToCodeExpression(ScriptBody.dataNodes[currentNode.inputs[1]]);
                            }
                        }
                        // record the output type
                        ScriptBody.dataNodes[currentNode.outputs[0]].vmType = ScriptBody.dataNodes[currentNode.outputs[0]].vmType;
                        MatchedExpressions.Add(currentNode.outputs[0], cbo);
                        break;
                    case GSharp.GSIL.GCode.Type.invokeStaticCall:
                    // SECTION INCOMPLETE
                    case GSharp.GSIL.GCode.Type.invokeInstanceCall:
                        // SECTION INCOMPLETE
                        // collect any arguments
                        List<CodeExpression> inputs = new List<CodeExpression>();
                        foreach (int input in currentNode.inputs)
                        {
                            inputs.Add(ResolveDataToCodeExpression(ScriptBody.dataNodes[input]));
                        }
                        CodeMethodInvokeExpression call;
                        CodeTypeReferenceExpression refExpr;

                        if (currentNode.type == GSharp.GSIL.GCode.Type.invokeStaticCall)
                        {
                            refExpr = new CodeTypeReferenceExpression(currentNode.reference);
                        }
                        else // dont yet handle instances and function calls
                        {
                            refExpr = new CodeTypeReferenceExpression("");
                        }
                        call = new CodeMethodInvokeExpression(refExpr, currentNode.target, inputs.ToArray());
                        // When there are outputs save them before dumping function into pipeline
                        if (currentNode.outputs != null)
                        {
                            CodeVariableDeclarationStatement cvd = new CodeVariableDeclarationStatement();
                            cvd.Name = "auto_" + (uint)FileName.GetHashCode() + currentNode.outputs[0].ToString();
                            cvd.InitExpression = call;
                            cvd.Type = new CodeTypeReference(ScriptBody.dataNodes[currentNode.outputs[0]].vmType);
                            func.Statements.Add(cvd);
                        }
                        else
                            func.Statements.Add(call);
                        break;
                    case GSharp.GSIL.GCode.Type.executionExit:
                        // SECTION INCOMPLETE
                        if (currentNode.inputs == null) break;
                        GSharp.GSIL.GData.Node funcReturn = ScriptBody.dataNodes[currentNode.inputs[0]];
                        func.Statements.Add(new CodeMethodReturnStatement(ResolveDataToCodeExpression(funcReturn)));
                        func.ReturnType = func.ReturnType = new CodeTypeReference(funcReturn.vmType);
                        break;
                    default:
                        Debug.Fail("NodeTypeId " + currentNode.type + " cannot be transformed.");
                        break;
                }
                if (currentNode.execution == -1) break;
                currentNode = ScriptBody.codeNodes[currentNode.execution];
            }
            return currentNode;
        }
    }
}
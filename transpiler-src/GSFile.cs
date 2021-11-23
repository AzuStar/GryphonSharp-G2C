using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
        public string NamespacePath;
        /// <summary>
        /// Script's body
        /// </summary>
        public Script ScriptBody;

        public GSFile(string path)
        {
            // keep for debugging
            // ITraceWriter tracert = new MemoryTraceWriter();
            // JsonSerializerSettings settings = new JsonSerializerSettings(){
            //     TraceWriter = tracert,
            //     MissingMemberHandling = MissingMemberHandling.Ignore
            // };
            ScriptBody = JsonConvert.DeserializeObject<Script>(File.ReadAllText(path));
            ScriptBody.PostDeserialize();
            string[] split = path.Split("/");
            FileName = split[^1];
            FileName = new Regex("(.+)\\.gs$").Match(FileName).Groups[1].ToString();
            NamespacePath = "Root"; // testing, so root for now

        }

        public String GenerateSource(CodeDomProvider provider)
        {
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            CodeNamespace ns = new CodeNamespace(NamespacePath);
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
                func.Name = kv.Value.target;
                FunctionBuilder(func, kv.Value);
                func.ReturnType = new CodeTypeReference(typeof(int));
                func.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "test"));
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

        private CodeExpression ResolveDataToCodeExpression(GSharp.GSIL.GData.Node dataNode)
        {
            CodeExpression ce = null;
            switch (dataNode.type)
            {
                case GSharp.GSIL.GData.Type.primitiveValue:
                    ce = new CodePrimitiveExpression(dataNode.value);
                    break;
                case GSharp.GSIL.GData.Type.localValue:
                    ce = new CodeVariableReferenceExpression("auto_" + (uint)FileName.GetHashCode() + dataNode.id.ToString());
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
            while (currentNode.execution != -1)
            {
                switch (currentNode.type)
                {
                    case GSharp.GSIL.GCode.Type.executionEnter:
                        // SECTION INCOMPLETE
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
                        func.Statements.Add(cbo);
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
                        func.Statements.Add(call);
                        break;
                    case GSharp.GSIL.GCode.Type.executionExit:
                        // SECTION INCOMPLETE
                        func.Statements.Add(new CodeMethodReturnStatement(ResolveDataToCodeExpression(ScriptBody.dataNodes[currentNode.inputs[0]])));
                        break;
                    default:
                        Debug.Fail("NodeTypeId " + currentNode.type + " cannot be transformed.");
                        break;
                }
                currentNode = ScriptBody.codeNodes[currentNode.execution];
            }
            return currentNode;
        }
    }
}
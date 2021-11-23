using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using GSharp.GSIL.GCode;
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

            IEnumerable<KeyValuePair<int, Node>> funcEntries = ScriptBody.codeNodes.Where((kv) => kv.Value.type == Type.executionEnter);

            IEnumerable<KeyValuePair<int, Node>> codeEntry = funcEntries.Where((kv) => kv.Value.target == "Main"); // first doesnt work, dont touch

            funcEntries = funcEntries.Where((kv) => kv.Value.target != "Main");

            if (codeEntry.Any())
            {
                CodeEntryPointMethod entry = new CodeEntryPointMethod();

                Node currentNode = codeEntry.First().Value;

                while (currentNode.execution != -1 || currentNode.type != GSharp.GSIL.GCode.Type.executionExit)
                {
                    switch(currentNode.type){
                        case Type.executionEnter:

                        default:
                        Console.Error("TypeId "+currentNode.type+" not found!");
                    }
                    if (currentNode.type == GSharp.GSIL.GCode.Type.invokeInstanceCall || currentNode.type == GSharp.GSIL.GCode.Type.invokeStaticCall)
                    {
                        // collect any arguments
                        List<CodeExpression> inputs = new List<CodeExpression>();
                        foreach (int input in currentNode.inputs)
                        {
                            GSharp.GSIL.GData.Node n = ScriptBody.dataNodes[input];
                            if (n.type == GSharp.GSIL.GData.Type.primitiveValue)
                            {
                                inputs.Add(new CodePrimitiveExpression(n.value));
                            }
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
                        entry.Statements.Add(call);

                    }
                    currentNode = ScriptBody.codeNodes[currentNode.execution];
                }

                mainClass.Members.Add(entry);
            }

            foreach (KeyValuePair<int, Node> kv in funcEntries)
            {
                CodeMemberMethod func = new CodeMemberMethod();
                func.Name = kv.Value.target;
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

    }
}
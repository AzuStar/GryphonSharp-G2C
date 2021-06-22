using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Channels;
using GSharp.System.GNode;
using GSharp.System.GScript;
using Newtonsoft.Json;

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
            ScriptBody = JsonConvert.DeserializeObject<Script>(File.ReadAllText(path));
            string[] split = path.Split("/");
            FileName = split[^1];
            FileName = FileName.Split(@".gs")[0]; // @ for crossplatform
            NamespacePath = "Root"; // testing, so root for now

        }

        public String GenerateSource(CodeDomProvider provider)
        {
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            CodeNamespace ns = new CodeNamespace(NamespacePath);
            codeUnit.Namespaces.Add(ns);

            CodeTypeDeclaration mainClass = new CodeTypeDeclaration(FileName);
            ns.Types.Add(mainClass);

            IEnumerable<KeyValuePair<int, Node>> funcEntries = ScriptBody.code.Where((kv) => kv.Value.type == 0);

            IEnumerable<KeyValuePair<int, Node>> codeEntry = funcEntries.Where((kv) => kv.Value.target == "Main"); // first doesnt cut, dont touch

            funcEntries = funcEntries.Where((kv) => kv.Value.target != "Main");

            if (codeEntry.Any())
            {
                CodeEntryPointMethod entry = new CodeEntryPointMethod();

                Node currentNode = codeEntry.First().Value;

                while (currentNode.execOut != null || currentNode.type != GSharp.System.GNode.Type.executionExit)
                {
                    if (currentNode.type == GSharp.System.GNode.Type.callInstance || currentNode.type == GSharp.System.GNode.Type.callStatic)
                    {
                        List<CodeExpression> inputs = new List<CodeExpression>();
                        foreach (int input in currentNode.inputs)
                        {
                            Node n = ScriptBody.code[input];
                            if(n.type == GSharp.System.GNode.Type.primitiveValue){
                                inputs.Add(new CodePrimitiveExpression(n.outputs[0]));
                            }
                        }
                        CodeMethodInvokeExpression call;
                        CodeTypeReferenceExpression refExpr;

                        if (currentNode.type == GSharp.System.GNode.Type.callStatic)
                        {
                            refExpr = new CodeTypeReferenceExpression(currentNode.reference);
                        }
                        else
                        {
                            refExpr = new CodeTypeReferenceExpression("");
                        }
                        call = new CodeMethodInvokeExpression(refExpr, currentNode.target, inputs.ToArray());
                        entry.Statements.Add(call);

                    }
                    currentNode = ScriptBody.code[currentNode.execOut[0]];
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
                });
                srcText = sw.ToString();
            }
            return srcText;
        }

    }
}
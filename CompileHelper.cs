using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillBetterThanPHPLang {
    class CompileHelper {
        public static void Test(string code) {
            CSharpCodeProvider csp = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateInMemory = true;
            cp.GenerateExecutable = false;
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("StillBetterThanPHPLang.exe");
            cp.MainClass = "MainClass";
            var cc = csp.CreateCompiler();
            var output = cc.CompileAssemblyFromSource(cp, DummyCode.Prefix + code + DummyCode.Suffix);
            StringWriter sw = new StringWriter();
            foreach (CompilerError ce in output.Errors) {
                if (ce.IsWarning) continue;
                sw.WriteLine("{0}({1},{2}: error {3}: {4}", ce.FileName, ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText);
            }
            string errorText = sw.ToString();
            if (errorText.Length > 0)
                throw new ApplicationException(errorText);
            output.CompiledAssembly.GetType("StillBetterThanPHPLang.MainClass").GetMethod("Main").Invoke(null, new string[0]);
        }
    }
}

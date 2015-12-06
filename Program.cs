using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillBetterThanPHPLang {
    class EntryPoint {
        static void Main(string[] args) {
            var program = FileLoader.Load("index.stillbetterthanphp");
            var builder = new ProgramBuilder(program);
            var code = builder.BuildToCSharp();

            bool optimize = true;
            if (optimize) {
                code = CSharpOptimizer.OptimizeCSharp(code);
            }

            File.WriteAllText("output.cs", code);
            CompileHelper.Test(code);
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillBetterThanPHPLang {
    class Expression {
        public string SourceString;
        public Expression(string sourceString) {
            this.SourceString = sourceString;
        }
        char[] Operations = new char[] { '@', '=', '+', '-', '/', '*', '<', '>', ':', '?'};
        public string BuildToCSharp(char[,] program, ProgramBuilder builder, Point pos, Point step) {
            var src = "";
            if (SourceString.StartsWith("$")) {
                SourceString = SourceString.Substring(1);
                var operation_index = 0;
                for (int i = 0; i < SourceString.Length; i++) {
                    if (Operations.Contains(SourceString[i])) {
                        operation_index = i;
                        break;
                    }
                }
                var var_name = SourceString.Substring(0, operation_index);
                var operation = SourceString.Substring(operation_index, 1);
                if (operation != "@") {
                    var value = new Expression(SourceString.Substring(operation_index + 1)).BuildToCSharp(program, builder, pos, step);
                    if (operation == "=") {
                        src += builder.GetCurrentLineLabel() + ": " + "_ints[\"" + var_name + "\"] = " + value + ";\n";
                    }
                }
                if (operation == "@") {
                    src += builder.GetCurrentLineLabel() + ": print(_ints[\"" + var_name + "\"]);\n";
                }
                if (operation == "<" || operation == ">" || operation == ":" || operation == "?") {
                    var value = new Expression(SourceString.Substring(operation_index + 1)).BuildToCSharp(program, builder, pos, step);
                    var finalOperation = operation.Replace(":", "==");
                    var preIf = "";
                    var statement = "_ints[\"" + var_name + "\"]" + finalOperation + value;

                    if (operation == "?") {
                        preIf = "_tempString=Console.ReadLine();\n";
                        statement = "int.TryParse(_tempString,out _tempInt)";
                    }

                    src += builder.GetCurrentLineLabel() + ": " + preIf + "if(" + statement + ") {\n";
                    for (int i = 1; i <= 2; i++) {
                        if (i == 2) {
                            src += "else{\n";
                        } else {
                            if (operation == "?") {
                                src += "_ints[\"" + var_name + "\"]=_tempInt;\n";
                            }
                        }
                        var subBuilder = new ProgramBuilder(program, builder);
                        subBuilder.pos = (Point)builder.pos.Clone();
                        subBuilder.step = (Point)builder.step.Clone();
                        subBuilder.pos.x += subBuilder.step.x * i;
                        subBuilder.pos.y += subBuilder.step.y * i;
                        src += subBuilder.BuildToCSharp() + "}";
                    }
                    builder.ExpressionBuiltTree = true;
                }
            } else {
                var operation_index = 0;
                for (int i = 0; i < SourceString.Length; i++) {
                    if (Operations.Contains(SourceString[i])) {
                        operation_index = i;
                        break;
                    }
                }
                if (operation_index == 0) {
                    int throwaway = 0;
                    if (!int.TryParse(SourceString, out throwaway)) {
                        return "_ints[\"" + SourceString + "\"]";
                    }
                    return SourceString;
                }
                var var_name = new Expression(SourceString.Substring(0, operation_index)).BuildToCSharp(program, builder, (Point)pos.Clone(), (Point)step.Clone());
                var value = new Expression(SourceString.Substring(operation_index + 1)).BuildToCSharp(program, builder, (Point)pos.Clone(), (Point)step.Clone());
                return var_name + SourceString[operation_index] + value;
            }
            return src;
        }
    }
}

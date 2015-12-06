using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillBetterThanPHPLang {
    public static class CSharpOptimizer {
        public static string OptimizeCSharp(string str) {
            var recursed_str = OptimizeLabels(str);
            while (str.Trim() != recursed_str.Trim()) {
                str = recursed_str;
                recursed_str = OptimizeLabels(str);
            }
            return OptimizePrints(str).Trim();
        }
        static string OptimizePrints(string str) {
            var new_str = "";
            var lines = str.Split('\n').ToList<string>();
            for (int i = 0; i < lines.Count - 1; i++) {
                var line = lines[i];
                if (line.StartsWith("print(\"") && line.EndsWith("\");") && lines[i + 1].StartsWith("print(\"") && lines[i + 1].EndsWith("\");")) {
                    lines[i] = line.Insert(line.Length - 3, lines[i + 1].Substring(7, lines[i + 1].Length - 10));
                    lines[i] = lines[i];
                    lines.RemoveAt(i + 1);
                    i--;
                } else {
                    new_str += lines[i] + "\n";
                }
            }
            new_str = new_str.Replace("print(\"\");\n", "");

            return new_str;
        }
        static string OptimizeLabels(string str) {
            var lines = str.Split('\n');
            var usedLabels = new List<string>();
            for (int i = 0; i < lines.Length; i++) {
                var line = lines[i];
                if (line.StartsWith("goto")) {
                    var label = line.Replace("goto ", "").Replace(";", "");
                    usedLabels.Add(label);
                }
            }
            str = "";
            for (int i = 0; i < lines.Length; i++) {
                var line = lines[i];
                if (line.StartsWith("Label")) {
                    var label = line.Substring(0, line.IndexOf(":"));
                    if (!usedLabels.Contains(label)) {
                        line = line.Substring(line.IndexOf(":") + 2);
                    }
                }
                str += line + "\n";
            }
            return str;
        }
    }
}

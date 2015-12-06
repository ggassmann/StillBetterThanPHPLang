using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillBetterThanPHPLang {
    class FileLoader {
        public static char[,] Load(string path) {
            var lines = File.ReadAllLines(path);
            var max_width = -1;
            foreach (var line in lines) {
                max_width = Math.Max(max_width, line.Replace("\t", "    ").Length); //Tabs are always replaced with 4, master race
            }
            var program = new char[max_width, lines.Length];
            var y = 0;
            foreach(var line in lines) {
                var x = 0;
                foreach (var c in line) {
                    if (c == '\t') { 
                        for (int i = 0; i < 4; i++) {
                            program[x, y] = ' ';
                            x++;
                        }
                    } else {
                        program[x, y] = c;
                       
                        x++;
                    }
                }
                y++;
            }
            return program;
        }
    }
}

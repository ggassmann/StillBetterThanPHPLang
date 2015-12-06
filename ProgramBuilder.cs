using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StillBetterThanPHPLang {
    enum KeyChars {
        ArrowRight = '>',
        ArrowLeft = '<',
        ArrowDown = 'v',
        ArrowUp = '^',
        EndProgram = ';',
        MirrorBackward = '\\',
        MirrorForward = '/',
        RandomDirection = '#',
        Skip = ',',
        ForcePrintNextChar = '*',
        Newline = '\"',
        StartExpression = '[',
        EndExpression = ']',
    }
    enum ReadingState {
        Main,
        Expression,
    }
    class ProgramBuilder {
        public char[,] Program;
        public ProgramBuilder ParentProgramBuilder;

        public List<KeyValuePair<Point, Point>> SplitPoints = new List<KeyValuePair<Point, Point>>();
        public List<string> SplitStepChangePoints = new List<string>();
        public bool IsSplitPoint(Point pos, Point step) {
            foreach(var pair in SplitPoints) {
                if (pair.Key.x == pos.x && pair.Key.y == pos.y && pair.Value.x == step.x && pair.Value.y == step.y) {
                    return true;
                }
            }
            if (ParentProgramBuilder == null) {
                return false;
            }
            return ParentProgramBuilder.IsSplitPoint(pos, step);
        }

        public ProgramBuilder(char[,] program) {
            this.Program = program;
        }
        public ProgramBuilder(char[,] program, ProgramBuilder parentBuilder) {
            this.Program = program;
            this.ParentProgramBuilder = parentBuilder;
        }
        public Point pos = new Point();
        public Point step = new Point(1, 0);

        public ProgramBuilder RootBuilder{get{
            if (ParentProgramBuilder == null) return this;
            else return ParentProgramBuilder.RootBuilder;
        }}

        public List<Point> BuiltRandoms = new List<Point>();
        public bool HasBuiltRandom(Point point) {
            foreach (Point p in RootBuilder.BuiltRandoms) {
                if (point.x == p.x && point.y == p.y) {
                    return true;
                }
            }
            return false;
        }

        public void ResetVars() {
            pos = new Point();
            step = new Point(1, 0);
        }
        public char GetCurrentChar() {
            return Program[pos.x, pos.y];
        }

        ReadingState current_state = ReadingState.Main;

        public string GetCurrentLineLabel() {
            return 
                "Label" + pos.x.ToString().Replace("-", "minus") + 
                "x" + pos.y.ToString().Replace("-", "minus") + 
                "x" + step.x.ToString().Replace("-", "minus") + 
                "x" + step.y.ToString().Replace("-", "minus");
        }
        public string GetCurrentLineLabelNoDirection() {
            return "Label" + pos.x.ToString().Replace("-", "minus") + "x" + pos.y.ToString().Replace("-", "minus");
        }

        public bool ExpressionBuiltTree = false;

        public string BuildToCSharp() {
            char current_char = GetCurrentChar();
            var new_step = new Point(
                (current_char == (char)KeyChars.ArrowLeft ? -1 : current_char == (char)KeyChars.ArrowRight ? 1 : 0),
                (current_char == (char)KeyChars.ArrowUp ? -1 : current_char == (char)KeyChars.ArrowDown ? 1 : 0)
            );
            if (IsSplitPoint(pos, step)) {
                if (HasBuiltRandom(pos)) {
                    return "goto Label" + pos.x + "x" + pos.y + ";\n";
                }
                return "goto " + GetCurrentLineLabel() + ";\n";
            }
            SplitPoints.Add(new KeyValuePair<Point, Point>((Point)pos.Clone(), (Point)step.Clone()));

            var src = "";
            var no_print = false;

            src += GetCurrentLineLabel() + ": ";

            if (current_char == (char)KeyChars.ArrowDown || current_char == (char)KeyChars.ArrowUp || current_char == (char)KeyChars.ArrowLeft || current_char == (char)KeyChars.ArrowRight) {
                SplitStepChangePoints.Add(pos.x + "x" + pos.y + "x" + step.x + "x" + step.y + "x" + new_step.x + "x" + new_step.y);
                src += "print(\"\");\n";
                step = new_step;
                no_print = true;
            }
            if (current_char == (char)KeyChars.MirrorBackward) {
                if (step.x > 0 && step.y == 0) {
                    step.x = 0;
                    step.y = 1;
                } else if (step.x < 0 && step.y == 0) {
                    step.x = 0;
                    step.y = -1;
                } else if (step.y > 0 && step.x == 0) {
                    step.x = 1;
                    step.y = 0;
                } else if (step.y < 0 && step.x == 0) {
                    step.x = -1;
                    step.y = 0;
                }
                no_print = true;
            }
            if (current_char == (char)KeyChars.MirrorForward) {
                if (step.x > 0 && step.y == 0) {
                    step.x = 0;
                    step.y = -1;
                } else if (step.x < 0 && step.y == 0) {
                    step.x = 0;
                    step.y = 1;
                } else if (step.y > 0 && step.x == 0) {
                    step.x = -1;
                    step.y = 0;
                } else if (step.y < 0 && step.x == 0) {
                    step.x = 1;
                    step.y = 0;
                }
                no_print = true;
            }
            if (current_char == (char)KeyChars.RandomDirection) {
                if (!HasBuiltRandom(new Point(pos.x, pos.y))) {
                    RootBuilder.BuiltRandoms.Add(new Point(pos.x, pos.y));
                    var potential_steps = new Point[] { new Point(0, 1), new Point(1, 0), new Point(-1, 0), new Point(0, -1) };
                    src += GetCurrentLineLabelNoDirection() + ": _tempInt = _r.Next(0, 3);\n";
                    var index = 0;
                    foreach (var potential_step in potential_steps) {
                        var innerProgramBuilder = new ProgramBuilder(Program, this);
                        innerProgramBuilder.pos.x = pos.x + potential_step.x;
                        innerProgramBuilder.pos.y = pos.y + potential_step.y;
                        innerProgramBuilder.step.x = potential_step.x;
                        innerProgramBuilder.step.y = potential_step.y;
                        var inner = innerProgramBuilder.BuildToCSharp();
                        src += "if(_tempInt==" + index + "){\n" + inner + "}\n";
                        index++;
                    }
                } else {
                    src += "goto " + GetCurrentLineLabelNoDirection() + ";\n";
                }
                return src;
            }

            if (current_char == (char)KeyChars.EndProgram) {
                return "";
            }

            if (current_char == (char)KeyChars.Skip) {
                no_print = true;
            }

            if (current_char == (char)KeyChars.ForcePrintNextChar) {
                pos.x += step.x;
                pos.y += step.y;
            }

            if (current_char == (char)KeyChars.Newline) {
                src += "print(\"\\n\");\n";
                no_print = true;
            }

            if (current_char == (char)KeyChars.StartExpression) {
                current_state = ReadingState.Expression;
                src += GetExpression().BuildToCSharp(Program, this, (Point)pos.Clone(), (Point)step.Clone());
                if (ExpressionBuiltTree) {
                    return src;
                }
                no_print = true;
            }

            if (!no_print && current_state == ReadingState.Main) {
                src += "print(\"" + GetCurrentChar().ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\");\n";
            }
            pos.x += step.x;
            pos.y += step.y;
            return src + BuildToCSharp();
        }

        Expression GetExpression() {
            string src = "";
            while (current_state == ReadingState.Expression) {
                pos.x += step.x;
                pos.y += step.y;
                if (GetCurrentChar() == ']') {
                    current_state = ReadingState.Main;
                    break;
                }
                src += GetCurrentChar();
            }
            var expression = new Expression(src);
            return expression;
        }
    }
}

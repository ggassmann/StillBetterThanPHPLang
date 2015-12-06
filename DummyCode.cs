using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillBetterThanPHPLang {
    public class DummyCode {
        public const string Prefix = "using System; using System.Collections.Generic; namespace StillBetterThanPHPLang { class MainClass { static Random _r = new Random(); static Dictionary<string, int> _ints = new Dictionary<string, int>(); static string _tempString = \"\"; static int _tempInt = 0;\npublic static void print(object o) { Console.Write(o); } public static void Main() { ";
        public const string Suffix = "} } }";
    }
}

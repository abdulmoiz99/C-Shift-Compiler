using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CShiftCompiler
{
    class ClassIdentification
    {
        public static string IsKeyword(string value)
        {
            Dictionary<string, List<string>> keywords = new Dictionary<string, List<string>>()
            {
                //Data Types
                {"Data Type", new List<string>() {"int", "string", "char", "bool", "long", "double", "float"} },
                {"void", new List<string>() {"void"} },

                //Access Modifiers
                {"Access Modifiers", new List<string>() {"public", "private", "protected"} },

                //Conditional
                {"if", new List<string>() {"if"} },
                {"else", new List<string>() {"else"} },
                {"elif", new List<string>() {"elif"} },

                //Iterative
                {"foreach", new List<string>() {"foreach"} },
                {"for", new List<string>() {"for"} },
                {"until", new List<string>() {"until"} },

                //Jump
                {"jump", new List<string>() {"jump"} },
                {"skip-stop", new List<string>() {"skip", "stop"} },
                {"return", new List<string>() {"return"} },

                //Exception Handling
                {"toss", new List<string>() {"toss"} },
                {"try", new List<string>() {"try"} },
                {"catch", new List<string>() {"catch"} },
                {"finally", new List<string>() {"finally"} },

                //Data Defining
                {"var", new List<string>() {"var"}  },
                {"const", new List<string>() {"const"}  },
                {"struct", new List<string>() {"struct"}  },

                //Object Oriented
                {"class", new List<string>() {"class"}  },
                {"interface", new List<string>() {"interface"} },
                {"abstract", new List<string>() {"abstract"} },
                {"final", new List<string>() {"final"} },
                {"virtual", new List<string>() {"virtual"} },
                {"override", new List<string>() {"override"} },
                {"static", new List<string>() {"static"} },

                //Helper
                {"in", new List<string>() {"in"} },
                {"new", new List<string>() {"new"} },
                {"ref", new List<string>() {"ref"} }
            };

            foreach (string classPart in keywords.Keys)
            {
                List<string> valueParts = keywords[classPart];
                if (valueParts.Contains(value)) return classPart;
            }

            return String.Empty;
        }

        public static string IsOperator(string value)
        {
            Dictionary<string, List<string>> operators = new Dictionary<string, List<string>>()
            {
                //Arithmetic
                {"PM", new List<string>() {"+", "-"} },
                {"MDM", new List<string>() {"*", "/", "%"} },

                //Logical
                {"OR", new List<string>() {"||"} },
                {"AND", new List<string>() {"&&"} },
                {"!", new List<string>() {"!"} },

                //Relational
                {"Relational", new List<string>() {"<", ">", ">=", "<=", "!=", "=="} },
                
                //Inc-Dec
                {"Inc-Dec", new List<string>() {"++", "--"} },

                //Assignment
                {"=", new List<string>() {"="} },
                {"Compound-Assignment", new List<string>() {"+=", "-=", "/=", "*="} }
            };

            foreach (string classPart in operators.Keys)
            {
                List<string> valueParts = operators[classPart];
                if (valueParts.Contains(value)) return classPart;
            }

            return String.Empty;
        }

        public static bool IsPunctuator(string value) 
        {
            string[] punctuators = { ";", ",", ":", "(", ")", "{", "}", "[", "]" };

            if (punctuators.Contains(value)) return true;
            else return false;
        }

        public static bool IsIdentifier(string value) 
        {
            return Regex.IsMatch(value, @"^[a-zA-Z_]\w*$");
        }

        public static bool IsStringConstant(string value)
        {
            return Regex.IsMatch(value, "^\".*\"$");
        }

        public static bool IsFloatConstant(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]\d+$");
        }
        
        public static bool IsIntConstant(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d+$");
        }

        public static bool IsCharConstant(string value)
        {
            return Regex.IsMatch(value, @"^'[\\]?.'$");
        }
    }
}

using System;
using System.Collections.Generic;

namespace CShiftCompiler
{
    class SemanticAnalyzer
    {
        static List<MainTable> MT = new List<MainTable>();
        static List<FunctionTable> FT = new List<FunctionTable>();
        static Stack<int> scope = new Stack<int>();
        static int scopeValue = 0;
        public static List<string> errors = new List<string>();

        public static List<DataTable> Create_DT()
        {
            return new List<DataTable>();
        }

        public static bool Insert_FT(string name, string type)
        {
            bool isRedeclared = false;

            foreach (FunctionTable row in FT)
            {
                if (row.name == name && row.scope == scope.Peek())
                {
                    isRedeclared = true;
                    break;
                }
            }

            if (!isRedeclared)
            {
                FunctionTable tableRow = new FunctionTable();

                tableRow.name = name;
                tableRow.type = type;
                tableRow.scope = scopeValue;

                FT.Add(tableRow);
                return true;
            }

            else return false;
        }

        public static bool Insert_DT(string name, string type, string typeModifier, List<DataTable> link)
        {
            if (link == null) return true;

            bool isRedeclared = false;

            foreach (DataTable row in link)
            {
                if (row.name == name)
                {
                    if (row.type.Contains("->") && type.Contains("->"))
                    {
                        string rowParameters = row.type.Substring(0, row.type.IndexOf("->"));
                        string Parameters = type.Substring(0, type.IndexOf("->"));

                        if (rowParameters == Parameters)
                        {
                            isRedeclared = true;
                            break;
                        }
                    }
                    else
                    {
                        isRedeclared = true;
                        break;
                    }
                }
            }

            if (!isRedeclared)
            {
                DataTable tableRow = new DataTable();

                tableRow.name = name;
                tableRow.type = type;
                tableRow.typeModifier = typeModifier;

                link.Add(tableRow);
                return true;
            }

            else return false;
        }

        public static bool Insert_MT(string name, string type, string category, string parent, List<DataTable> link)
        {
            bool isRedeclared = false;

            foreach (MainTable row in MT)
            {
                if (row.name == name)
                {
                    isRedeclared = true;
                    break;
                }
            }

            if (!isRedeclared)
            {
                MainTable tableRow = new MainTable();

                tableRow.name = name;
                tableRow.type = type;
                tableRow.category = category;
                tableRow.parent = parent;
                tableRow.link = link;

                MT.Add(tableRow);
                return true;
            }

            else return false;
        }

        public static string Lookup_MT(string name, out string category)
        {
            foreach (MainTable row in MT)
            {
                if (row.name == name)
                {
                    category = row.category;
                    return row.type;
                }
            }

            category = "";
            return "";
        }

        public static void createScope()
        {
            scope.Push(++scopeValue);
        }

        public static void destroyScope()
        {
            scope.Pop();
        }

        public static string typeCheck(string t1, string t2, string opr)
        {
            string t3 = "";

            if (opr == "+")
            {
                if (t1 == "string" && t2 == "string")
                {
                    t3 = "string";
                }
                else if ((t1 == "string" && t2 == "int") || (t1 == "int" && t2 == "string"))
                {
                    t3 = "string";
                }
                else if ((t1 == "string" && t2 == "float") || (t1 == "float" && t2 == "string"))
                {
                    t3 = "string";
                }
                else if ((t1 == "string" && t2 == "char") || (t1 == "char" && t2 == "string"))
                {
                    t3 = "string";
                }
                else if ((t1 == "string" && t2 == "bool") || (t1 == "bool" && t2 == "string"))
                {
                    t3 = "string";
                }
                else if ((t1 == "float" && t2 == "int") || (t1 == "int" && t2 == "float"))
                {
                    t3 = "float";
                }
                else if ((t1 == "char" && t2 == "int") || (t1 == "int" && t2 == "char"))
                {
                    t3 = "int";
                }
                else if ((t1 == "char" && t2 == "float") || (t1 == "float" && t2 == "char"))
                {
                    t3 = "float";
                }
                else if (t1 == "char" && t2 == "char")
                {
                    t3 = "int";
                }
                else if (t1 == "float" && t2 == "float")
                {
                    t3 = "float";
                }
                else if (t1 == "int" && t2 == "int")
                {
                    t3 = "int";
                }
            }

            else if (opr == "-" || opr == "*" || opr == "/" || opr == "%")
            {
                if ((t1 == "float" && t2 == "int") || (t1 == "int" && t2 == "float"))
                {
                    t3 = "float";
                }
                else if ((t1 == "char" && t2 == "int") || (t1 == "int" && t2 == "char"))
                {
                    t3 = "int";
                }
                else if ((t1 == "char" && t2 == "float") || (t1 == "float" && t2 == "char"))
                {
                    t3 = "float";
                }
                else if (t1 == "char" && t2 == "char")
                {
                    t3 = "int";
                }
                else if (t1 == "float" && t2 == "float")
                {
                    t3 = "float";
                }
                else if (t1 == "int" && t2 == "int")
                {
                    t3 = "int";
                }
            }

            else if (opr == "==" || opr == "!=")
            {
                if (t1 == "string" && t2 == "string")
                {
                    t3 = "bool";
                }
                else if (t1 == "int" && t2 == "int")
                {
                    t3 = "bool";
                }
                else if (t1 == "char" && t2 == "char")
                {
                    t3 = "bool";
                }
                else if (t1 == "float" && t2 == "float")
                {
                    t3 = "bool";
                }
                else if ((t1 == "int" && t2 == "float") || (t1 == "float" && t2 == "int"))
                {
                    t3 = "bool";
                }
                else if ((t1 == "int" && t2 == "char") || (t1 == "char" && t2 == "int"))
                {
                    t3 = "bool";
                }
                else if ((t1 == "char" && t2 == "float") || (t1 == "float" && t2 == "char"))
                {
                    t3 = "bool";
                }
            }

            else if (opr == ">=" || opr == "<=" || opr == ">" || opr == "<")
            {
                if (t1 == "int" && t2 == "int")
                {
                    t3 = "bool";
                }
                else if (t1 == "char" && t2 == "char")
                {
                    t3 = "bool";
                }
                else if (t1 == "float" && t2 == "float")
                {
                    t3 = "bool";
                }
                else if ((t1 == "int" && t2 == "float") || (t1 == "float" && t2 == "int"))
                {
                    t3 = "bool";
                }
                else if ((t1 == "int" && t2 == "char") || (t1 == "char" && t2 == "int"))
                {
                    t3 = "bool";
                }
                else if ((t1 == "char" && t2 == "float") || (t1 == "float" && t2 == "char"))
                {
                    t3 = "bool";
                }
            }

            else if (opr == "||" || opr == "&&")
            {
                if (t1 == "bool" && t2 == "bool")
                {
                    t3 = "bool";
                }
            }

            return t3;
        }

        public static string typeCheck(string t1, string opr)
        {
            string t2 = "";

            if (opr == "++" || opr == "--")
            {
                if (t1 == "char" || t1 == "int" || t1 == "float")
                {
                    t2 = t1;
                }
            }

            else if (opr == "!")
            {
                if (t1 == "bool")
                {
                    t2 = t1;
                }
            }

            return t2;
        }
    }

    class MainTable 
    {
        public string name { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public string parent { get; set; }
        public List<DataTable> link { get; set; }
    }

    class DataTable 
    {
        public string name { get; set; }
        public string type { get; set; }
        public string typeModifier { get; set; }
    }

    class FunctionTable 
    {
        public string name { get; set; }
        public string type { get; set; }
        public int scope { get; set; }
    }
}

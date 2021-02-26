using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShiftCompiler
{
    class SemanticAnalyzer
    {
        static List<MainTable> MT = new List<MainTable>();
        static List<FunctionTable> FT = new List<FunctionTable>();
        static Stack<int> scope = new Stack<int>();
        static int scopeValue = 0;

        public static List<DataTable> Create_DT() 
        {
            return new List<DataTable>();
        }

        bool Insert_FT(string name, string type) 
        {
            bool isRedeclared = false;

            foreach (FunctionTable row in FT)
            {
                if (row.name == name && row.scope == scopeValue)
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

        string GetType(string name) 
        {
            foreach (MainTable row in MT)
            {
                if (row.name == name)
                {
                    return row.type;
                }
            }
            return "";
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

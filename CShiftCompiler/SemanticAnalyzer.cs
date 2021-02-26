using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShiftCompiler
{
    class SemanticAnalyzer
    {
        static List<MainTable> MT;
        static List<FunctionTable> FT;
        static Stack<int> scope;
        static int scopeValue = 0;

        BodyTable Create_DT() 
        {
            return new BodyTable();
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

        bool Insert_MT(string name, string type, string category, string parent, List<BodyTable> link) 
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

        string Lookup_MT(string name, out string category) 
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

        void createScope() 
        {
            scope.Push(++scopeValue);
        }

        void destroyScope() 
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
        public List<BodyTable> link { get; set; }
    }

    class BodyTable 
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

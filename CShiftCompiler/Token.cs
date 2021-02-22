using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShiftCompiler
{
    class Token
    {
        string valuePart;
        string classPart;
        int lineNumber;

        public Token()
        {
            valuePart = "";
            classPart = "";
            lineNumber = 0;
        }

        public Token(string valuePart, string classPart, int lineNumber)
        {
            this.valuePart = valuePart;
            this.classPart = classPart;
            this.lineNumber = lineNumber;
        }
        public Token(int lineNumber, string valuePart)
        {
            this.valuePart = valuePart;
            classPart = "";
            this.lineNumber = lineNumber;
        }
        public Token(string valuePart)
        {
            this.valuePart = valuePart;
            classPart = "";
            lineNumber = 0;
        }
        public Token(int lineNumber)
        {
            valuePart = "";
            classPart = "";
            this.lineNumber = lineNumber;
        }


        public void SetClass(string classPart)
        {
            this.classPart = classPart;
        }

        public void SetValue(string valuePart)
        {
            this.valuePart = valuePart;
        }
        public void SetLineNo(int num) 
        {
            this.lineNumber = num;
        }
        public string GetClass()
        {
            return classPart;
        }
        public string GetValue()
        {
            return valuePart;
        }
        public int GetLineNo()
        {
            return lineNumber;
        }

    }
}

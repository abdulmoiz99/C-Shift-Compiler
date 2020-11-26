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
        public Token(string valuePart, int lineNumber)
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

    class Tokens
    {
        private List<Token> tokens;

        public Tokens() 
        {
            tokens = new List<Token>();
        }

        public void Add(Token token) 
        {
            tokens.Add(token);
        }

        public void Add(string valuePart) 
        {
            tokens.Add(new Token(valuePart));
        }
        public void Add(string valuePart, int lineNumber)
        {
            tokens.Add(new Token(valuePart, lineNumber));
        }

        public void Add(string valuePart, string classPart, int lineNumber)
        {
            tokens.Add(new Token(valuePart, classPart, lineNumber));
        }

        public void SetClass(int index, string classPart) 
        {
            if (tokens.ElementAt(index) != null) 
            {
                tokens[index].SetClass(classPart);
            }
        }
        public void Display()
        {
            foreach (Token token in tokens) 
            {
                Console.WriteLine("(" + token.GetClass() + ", " + token.GetValue() + ", " + token.GetLineNo());
            }
        }


    }
}

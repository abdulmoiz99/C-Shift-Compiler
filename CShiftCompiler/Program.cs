using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CShiftCompiler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        //Application.EnableVisualStyles();
        //Application.SetCompatibleTextRenderingDefault(false);
        //Application.Run(new Form1());

        //Initializing lineCounter;
        static int lineCounter = 1;        

        static void Main()
        {
            List<Token> tokens = GenerateTokens(Application.StartupPath + @"\Input\input3.txt");

            foreach (Token token in tokens)
            {
                IdentifyClass(token);
            }
            //To display words
            foreach (var token in tokens)
            {
                Console.WriteLine(" (" + token.GetLineNo() + ", " + token.GetClass() + ", " + token.GetValue() + ")");
            }
            Console.ReadKey();
        }

        private static void IdentifyClass(Token token)
        {
            if (ClassIdentification.IsIdentifier(token.GetValue()))
            {
                string classPart = ClassIdentification.IsKeyword(token.GetValue());

                if (classPart == String.Empty)
                {
                    token.SetClass("ID");
                }
                else
                {
                    token.SetClass(classPart);
                }
            }
            //operator
            else if (ClassIdentification.IsOperator(token.GetValue()) != String.Empty)
            {
                token.SetClass(ClassIdentification.IsOperator(token.GetValue()));
            }
            //string const
            else if (ClassIdentification.IsStringConstant(token.GetValue()))
            {
                token.SetClass("string-constant");
            }
            //char const
            else if (ClassIdentification.IsCharConstant(token.GetValue()))
            {
                token.SetClass("char-constant");
            }
            //int
            else if (ClassIdentification.IsIntConstant(token.GetValue()))
            {
                token.SetClass("int-constant");
            }
            //float const
            else if (ClassIdentification.IsFloatConstant(token.GetValue()))
            {
                token.SetClass("float-constant");
            }
            else if (ClassIdentification.IsPunctuator(token.GetValue()))
            {
                token.SetClass(token.GetValue());
            }
            else
            {
                token.SetClass("Invalid Lexeme");
            }
        }

        private static List<Token> GenerateTokens(string inputFile)
        {
            //tokens list
            List<Token> tokens = new List<Token>();

            char[] punctuators = { ';', ',', ':', '(', ')', '{', '}', '[', ']' }; //Dot Excluded
            char[] operators = { '+', '-', '/', '*', '%', '=', '!', '<', '>', '&', '|' };
            string[] doubleOperators = { "++", "--", "!=", "==", "<=", ">=", "*=", "-=", "+=", "/=", "%=", "&&", "||" };

            //Reading input files
            var reader = new StreamReader(inputFile);
            var cr = reader.ReadToEnd().ToCharArray();

            //Reading 
            for (int i = 0; i < cr.Count(); i++)
            {
                //FOR ALPHABET, DIGIT AND UNDERSCORE
                if (Char.IsLetterOrDigit(cr[i]) || cr[i] == '_')
                {
                    Temp.Add(cr[i]);
                }

                //FOR DOT
                else if (cr[i] == '.')
                {
                    if (Temp.Length() == 0 || IsDigitsOnly(Temp.GetValue()))
                    {
                        if (Char.IsDigit(cr[i + 1]))
                        {
                            Temp.Add(cr[i++]);
                            Temp.Add(cr[i]);
                        }
                        else
                        {
                            if (Temp.Length() > 0) tokens.Add(new Token(lineCounter, Temp.Empty()));
                            tokens.Add(new Token(lineCounter, cr[i].ToString()));
                        }
                    }
                    else
                    {
                        tokens.Add(new Token(lineCounter, Temp.Empty()));
                        i--;
                    }
                }

                else
                {
                    if (Temp.Length() > 0) tokens.Add(new Token(lineCounter, Temp.Empty()));

                    //FOR STRING
                    if (cr[i] == '"')
                    {
                        i = ValidateString(tokens, cr, i);
                    }
                    //FOR CHARACTERS
                    else if (cr[i] == '\'')
                    {
                        i = ValidateCharacter(tokens, cr, i);
                    }
                    //FOR SINGLE LINE COMMENTS
                    else if (cr[i] == '/' && i + 1 < cr.Length && cr[i + 1] == '/')
                    {
                        while (cr[i] != '\n' && i + 1 < cr.Length)
                        {
                            i++;
                        }
                        if (cr[i] == '\n') lineCounter++;
                    }
                    //FOR MULTI LINE COMMENTS                   
                    else if (cr[i] == '/' && i + 1 < cr.Length && cr[i + 1] == '*' && i + 2 < cr.Length)
                    {
                        i += 2;

                        while (cr[i] != '*' || cr[i + 1] != '/')
                        {
                            if (i + 1 < cr.Length) i++;
                            else break;

                            if (cr[i] == '\n') lineCounter++;
                        }
                        i++;
                    }
                    //FOR LINEBREAK OR SPACE
                    else if (cr[i] == '\n' || cr[i] == ' ' || cr[i] == '\r')
                    {
                        if (cr[i] == '\n') lineCounter++;
                        //Ignore
                    }
                    //FOR PUNCTUATORS
                    else if (punctuators.Contains(cr[i]))
                    {
                        tokens.Add(new Token(lineCounter, cr[i].ToString()));
                    }
                    //FOR OPERATORS
                    else if (operators.Contains(cr[i]))
                    {
                        Temp.Add(cr[i]);

                        if (i + 1 < cr.Length)
                        {
                            string temp = cr[i].ToString() + cr[i + 1].ToString();
                            if (doubleOperators.Contains(temp))
                            {
                                Temp.Add(cr[++i]);
                            }
                        }
                        tokens.Add(new Token(lineCounter, Temp.Empty()));
                    }
                    //FOR UNKNOWN CHARACTERS
                    else
                    {
                        Temp.Add(cr[i]);
                    }
                }
            }

            //EMPTY TEMP AFTER ENDOFFILE
            if (Temp.Length() > 0) tokens.Add(new Token(lineCounter, Temp.Empty()));

            return tokens;
        }

        private static int ValidateString(List<Token> tokens, char[] cr, int pointer)
        {
            // Start Conditions - "
            do // For appending first inverted comma
            {
                // Include Conditions ("\") - increment for the next character
                if (cr[pointer] == '\\')
                {
                    if (pointer + 1 < cr.Length) // condition might be wrong: string.IsNullOrEmpty(cr[i + 1].ToString())
                    {
                        Temp.Add(cr[pointer++]);
                    }
                    else
                    {
                        tokens.Add(new Token(lineCounter, Temp.Empty()));
                        break;
                    }
                }
                Temp.Add(cr[pointer]);
                //check if the next poiners exists or not: if not break the loop
                if (pointer + 1 < cr.Length)
                {
                    pointer++;
                    //Adding the last inverted comma
                    if (cr[pointer] == '"') Temp.Add(cr[pointer]);
                }
                else break;

            } while (cr[pointer] != '"' && cr[pointer] != '\n');//Break Conditions - \n(New Line), ", end of file

            tokens.Add(new Token(lineCounter, Temp.Empty()));
            if (cr[pointer] == '\n') lineCounter++;
            return pointer;
        }
        private static int ValidateCharacter(List<Token> tokens, char[] cr, int pointer)
        {
            //Adding First quotation
            Temp.Add(cr[pointer]);
            int charCount = 0;
            //check if the next index is available or not
            if (pointer + 1 < cr.Length)
            {
                //check if the next index is '\'(Slash) or not
                //if slash is there take 3 characters otherwise take 2 characters
                if (cr[pointer + 1] == '\\') charCount = 3;
                else charCount = 2;
                //Loop to read 2 or 3 characters
                for (int readChar = 0; readChar < charCount; readChar++)
                {
                    //check if the next index is avaiable also there should be no newline character
                    if (pointer + 1 < cr.Length && cr[pointer + 1] != '\n') Temp.Add(cr[++pointer]);
                    else break;
                }
            }
            //create a token
            tokens.Add(new Token(lineCounter, Temp.Empty()));
            if (cr[pointer] == '\n') lineCounter++;
            return pointer;
        }

        static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}

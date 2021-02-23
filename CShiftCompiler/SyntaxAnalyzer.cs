using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShiftCompiler
{
    class SyntaxAnalyzer
    {
        static int index = 0;
        List<Token> tokens;

        public SyntaxAnalyzer(List<Token> tokens) 
        {
            this.tokens = tokens;
            if (if_elif_else()) //S i.e. Starting Non Terminal returns true indicates tree is complete.
            {
                //index++; //Temporary

                if (tokens[index].GetClass() == "$") // Input is completely parsed.
                {
                    Console.WriteLine("Source code executed successfully!");
                }
                else
                {
                    Console.WriteLine("Syntax error at line # " + tokens[index].GetLineNo());
                }
            }
            else 
            {
                Console.WriteLine("Syntax error at line # " + tokens[index].GetLineNo());
            }
        }

        private bool S()
        {
            throw new NotImplementedException();
        }

        private bool OE() 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec") 
            {
                if (AE()) 
                {
                    if (OEd())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool OEd()
        {
            if (tokens[index].GetClass() == "OR")
            {
                index++;

                if (AE())
                {
                    if (OEd())
                    {
                        return true;
                    }
                }
            }
            else 
            {
                if (tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}" || tokens[index].GetClass() == ";" || tokens[index].GetClass() == ",") 
                {
                    return true;
                }

            }

            return false;
        }

        private bool AE()
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (RE())
                {
                    if (AEd())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AEd() 
        {
            if (tokens[index].GetClass() == "AND")
            {
                index++;

                if (RE())
                {
                    if (AEd())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}" || tokens[index].GetClass() == ";" || tokens[index].GetClass() == ","
                    || tokens[index].GetClass() == "OR")
                {
                    return true;
                }
            }

            return false;
        }

        private bool RE() 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (E())
                {
                    if (REd())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool REd()
        {
            if (tokens[index].GetClass() == "Relational")
            {
                index++;

                if (E())
                {
                    if (REd())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}" || tokens[index].GetClass() == ";" || tokens[index].GetClass() == ","
                    || tokens[index].GetClass() == "OR" || tokens[index].GetClass() == "AND")
                {
                    return true;
                }
            }

            return false;
        }

        private bool E() 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (T())
                {
                    if (Ed())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Ed() 
        {
            if (tokens[index].GetClass() == "PM")
            {
                index++;

                if (T())
                {
                    if (Ed())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}" || tokens[index].GetClass() == ";" || tokens[index].GetClass() == ","
                    || tokens[index].GetClass() == "OR" || tokens[index].GetClass() == "AND" || tokens[index].GetClass() == "Relational")
                {
                    return true;
                }
            }

            return false;
        }

        private bool T() 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (F())
                {
                    if (Td())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Td() 
        {
            if (tokens[index].GetClass() == "MDM")
            {
                index++;

                if (F())
                {
                    if (Td())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}" || tokens[index].GetClass() == ";" || tokens[index].GetClass() == ","
                    || tokens[index].GetClass() == "OR" || tokens[index].GetClass() == "AND" || tokens[index].GetClass() == "Relational" || 
                    tokens[index].GetClass() == "PM")
                {
                    return true;
                }
            }

            return false;
        }

        private bool F() 
        {
            if (tokens[index].GetClass() == "ID")
            {
                index++;

                if (G())
                {
                    return true;
                }
            }
            else if (tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" || tokens[index].GetClass() == "char-constant" ||
                     tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant")
            {
                if (constant())
                {
                    return true;
                }
            }
            else if (tokens[index].GetClass() == "(")
            {
                index++;

                if (OE())
                {
                    if (tokens[index].GetClass() == ")")
                    {
                        index++;
                        return true;
                    }
                }
            }
            else if (tokens[index].GetClass() == "!")
            {
                index++;

                if (F())
                {
                    return true;
                }
            }
            else if (tokens[index].GetClass() == "Inc-Dec") 
            {
                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    index++;

                    if (X()) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool G() 
        {
            if (tokens[index].GetClass() == ".")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (G())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "(")
            {
                index++;

                if (PL())
                {
                    if (tokens[index].GetClass() == ")")
                    {
                        index++;

                        if (H())
                        {
                            return true;
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "Inc-Dec")
            {
                index++;

                return true;
            }

            else 
            {
                if (tokens[index].GetClass() == "MDM" || tokens[index].GetClass() == "PM" || tokens[index].GetClass() == "Relational" ||
                    tokens[index].GetClass() == "AND" || tokens[index].GetClass() == "OR" || tokens[index].GetClass() == ";" ||
                    tokens[index].GetClass() == "," || tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}")
                {
                    return true;
                }
            }

            return false;
        }

        private bool H() 
        {
            if (tokens[index].GetClass() == ".")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (G())
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "MDM" || tokens[index].GetClass() == "PM" || tokens[index].GetClass() == "Relational" ||
                    tokens[index].GetClass() == "AND" || tokens[index].GetClass() == "OR" || tokens[index].GetClass() == ";" ||
                    tokens[index].GetClass() == "," || tokens[index].GetClass() == ")" || tokens[index].GetClass() == "}")
                {
                    return true;
                }
            } 

            return false;
        }

        private bool constant() 
        {
            if (tokens[index].GetClass() == "int-constant") 
            {
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "float-constant")
            {
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "char-constant")
            {
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "string-constant")
            {
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "bool-constant")
            {
                index++;
                return true;
            }
            return false;
        }

        private bool X() 
        {
            if (tokens[index].GetClass() == ".")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (X())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "(")
            {
                index++;

                if (PL())
                {
                    if (tokens[index].GetClass() == ")")
                    {
                        index++;

                        if (tokens[index].GetClass() == ".")
                        {
                            index++;

                            if (tokens[index].GetClass() == "ID")
                            {
                                index++;

                                if (X())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "until" || tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type" || 
                    tokens[index].GetClass() == "if" || tokens[index].GetClass() == "for" || tokens[index].GetClass() == "foreach" || 
                    tokens[index].GetClass() == "toss" || tokens[index].GetClass() == "try" || tokens[index].GetClass() == "const" || 
                    tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "jump" || tokens[index].GetClass() == "skip" || 
                    tokens[index].GetClass() == "stop" || tokens[index].GetClass() == "Inc-Dec" || tokens[index].GetClass() == "}" || 
                    tokens[index].GetClass() == ")" || tokens[index].GetClass() == "=" || tokens[index].GetClass() == "Compound-Assignment" || 
                    tokens[index].GetClass() == "MDM" || tokens[index].GetClass() == "PM" || tokens[index].GetClass() == "Relational" || 
                    tokens[index].GetClass() == "AND" || tokens[index].GetClass() == "OR" || tokens[index].GetClass() == ";" || tokens[index].GetClass() == ",") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool PL() 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (asgn_value())
                {
                    if (PL1())
                    {
                        return true;
                    }
                }
            }
            else 
            {
                if (tokens[index].GetClass() == ")") 
                {
                    return true;
                }
            }
            return false;
        }

        private bool PL1() 
        {
            if (tokens[index].GetClass() == ",") 
            {
                index++;

                if (asgn_value())
                {
                    if (PL1())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (tokens[index].GetClass() == ")")
                {
                    return true;
                }
            }
            return false;
        }

        private bool asgn_value() 
        {
            if (tokens[index].GetClass() == "new")
            {
                index++;

                if (constructor())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
                 {
                    if (OE())
                    {
                        return true;
                    }
                 }

            return false;
        }

        private bool constructor() 
        {
            if (tokens[index].GetClass() == "ID")
            {
                index++;

                if (tokens[index].GetClass() == "(") 
                {
                    index++;

                    if (PL()) 
                    {
                        if (tokens[index].GetClass() == ")") 
                        {
                            index++;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool for_statement() 
        {
            if (tokens[index].GetClass() == "for") 
            {
                index++;

                if (tokens[index].GetClass() == "(") 
                {
                    index++;

                    if (c1()) 
                    {
                        if (c2()) 
                        {
                            if (tokens[index].GetClass() == ";") 
                            {
                                index++;

                                if (c3()) 
                                {
                                    if (tokens[index].GetClass() == ")") 
                                    {
                                        index++;

                                        if (tokens[index].GetClass() == "{") 
                                        {
                                            index++;

                                            if (MST()) 
                                            {
                                                if (tokens[index].GetClass() == "}") 
                                                {
                                                    index++;

                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool c1() 
        {
            if (tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type")
            {
                if (declaration())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "ID")
            {
                index++;

                if (asgn_statement())
                {
                    if (tokens[index].GetClass() == ";")
                    {
                        index++;

                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == ";") 
            {
                index++;

                return true;
            }

            return false;
        }

        private bool c2() 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (OE())
                {
                    return true;
                }
            }

            else 
            {
                if (tokens[index].GetClass() == ";") 
                {
                    return true;
                }
            }

                return false;
        }

        private bool c3() 
        {
            if (tokens[index].GetClass() == "ID")
            {
                index++;

                if (X())
                {
                    if (c4())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "Inc-Dec")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (X())
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == ")") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool c4() 
        {
            if (tokens[index].GetClass() == "=" || tokens[index].GetClass() == "Compound-Assignment")
            {
                if (asgn_operator())
                {
                    if (asgn_value())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "Inc-Dec") 
            {
                index++;
                return true;
            }

            return false;
        }

        private bool MST() //Pending
        {
            return true;
        }

        private bool declaration() 
        {
            if (tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type")
            {
                if (type())
                {
                    if (tokens[index].GetClass() == "ID")
                    {
                        index++;

                        if (initialization())
                        {
                            if (list())
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool asgn_statement() 
        {
            if (tokens[index].GetClass() == "." || tokens[index].GetClass() == "(" || tokens[index].GetClass() == "=" ||
                tokens[index].GetClass() == "Compound-Assignment")
            {
                if (X())
                {
                    if (asgn_operator())
                    {
                        if (asgn_value())
                        {
                            return true;
                        }
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == ";") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool asgn_operator() 
        {
            if (tokens[index].GetClass() == "=")
            {
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "Compound-Assignment") 
            {
                index++;

                return true;
            }

            return false;
        }

        private bool type() 
        {
            if (tokens[index].GetClass() == "var")
            {
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "Data-Type") 
            {
                index++;

                return true;
            }

            return false;
        }

        private bool initialization()
        {
            if (tokens[index].GetClass() == "=")
            {
                index++;

                if (asgn_value()) 
                {
                    return true;
                }
            }

            else 
            {
                if (tokens[index].GetClass() == ";" || tokens[index].GetClass() == ",") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool list() 
        {
            if (tokens[index].GetClass() == ";")
            {
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == ",") 
            {
                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    index++;

                    if (initialization()) 
                    {
                        if (list()) 
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool foreach_statement() 
        {
            if (tokens[index].GetClass() == "foreach") 
            {
                index++;

                if (tokens[index].GetClass() == "(") 
                {
                    index++;

                    if (ID()) 
                    {
                        if (tokens[index].GetClass() == "ID") 
                        {
                            index++;

                            if (tokens[index].GetClass() == "in") 
                            {
                                index++;

                                if (tokens[index].GetClass() == "ID") 
                                {
                                    index++;

                                    if (X()) 
                                    {
                                        if (tokens[index].GetClass() == ")") 
                                        {
                                            index++;

                                            if (tokens[index].GetClass() == "{") 
                                            {
                                                index++;

                                                if (MST()) 
                                                {
                                                    if (tokens[index].GetClass() == "}") 
                                                    {
                                                        index++;

                                                        return true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool ID() 
        {
            if (tokens[index].GetClass() == "Data-Type")
            {
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "ID") 
            {
                index++;

                return true;
            }

            return false;
        }

        private bool until_statement() 
        {
            if (tokens[index].GetClass() == "until") 
            {
                index++;

                if (tokens[index].GetClass() == "(") 
                {
                    index++;

                    if (OE()) 
                    {
                        if (tokens[index].GetClass() == ")") 
                        {
                            index++;

                            if (tokens[index].GetClass() == "{") 
                            {
                                index++;

                                if (MST()) 
                                {
                                    if (tokens[index].GetClass() == "}") 
                                    {
                                        index++;

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool if_elif_else() 
        {
            if (tokens[index].GetClass() == "if") 
            {
                index++;

                if (tokens[index].GetClass() == "(") 
                {
                    index++;

                    if (OE()) 
                    {
                        if (tokens[index].GetClass() == ")") 
                        {
                            index++;

                            if (tokens[index].GetClass() == "{") 
                            {
                                index++;

                                if (MST()) 
                                {
                                    if (tokens[index].GetClass() == "}") 
                                    {
                                        index++;

                                        if (elif()) 
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool elif() 
        {
            if (tokens[index].GetClass() == "elif")
            {
                index++;

                if (tokens[index].GetClass() == "(")
                {
                    index++;

                    if (OE())
                    {
                        if (tokens[index].GetClass() == ")")
                        {
                            index++;

                            if (tokens[index].GetClass() == "{")
                            {
                                index++;

                                if (MST())
                                {
                                    if (tokens[index].GetClass() == "}")
                                    {
                                        index++;

                                        if (elif())
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "else")
            {
                if (else_statement())
                {
                    return true;
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "until" || tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type" ||
                    tokens[index].GetClass() == "if" || tokens[index].GetClass() == "for" || tokens[index].GetClass() == "foreach" ||
                    tokens[index].GetClass() == "toss" || tokens[index].GetClass() == "try" || tokens[index].GetClass() == "const" ||
                    tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "jump" || tokens[index].GetClass() == "skip" ||
                    tokens[index].GetClass() == "stop" || tokens[index].GetClass() == "Inc-Dec" || tokens[index].GetClass() == "}") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool else_statement() 
        {
            if (tokens[index].GetClass() == "else") 
            {
                index++;

                if (tokens[index].GetClass() == "{") 
                {
                    index++;

                    if (MST()) 
                    {
                        if (tokens[index].GetClass() == "}") 
                        {
                            index++;

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}

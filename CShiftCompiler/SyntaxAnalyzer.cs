using System;
using System.Collections.Generic;

namespace CShiftCompiler
{
    class SyntaxAnalyzer
    {
        static int index = 0;
        List<Token> tokens;

        public SyntaxAnalyzer(List<Token> tokens) 
        {
            this.tokens = tokens;
            if (S()) //S i.e. Starting Non Terminal returns true indicates tree is complete.
            {
                if (tokens[index].GetClass() == "$") // Input is completely parsed.
                {
                    if (SemanticAnalyzer.errors.Count <= 0)
                    {
                        Console.WriteLine("Source code executed successfully!");
                    }
                    else 
                    {
                        foreach (string error in SemanticAnalyzer.errors) 
                        {
                            Console.WriteLine(error);
                        }
                    }
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
            if (tokens[index].GetClass() == "class" || tokens[index].GetClass() == "abstract" || tokens[index].GetClass() == "static" || 
                tokens[index].GetClass() == "final") 
            {
                if (main_class()) 
                {
                    if (defs()) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool defs()
        {
            if (tokens[index].GetClass() == "class" || tokens[index].GetClass() == "abstract" || tokens[index].GetClass() == "static" ||
                tokens[index].GetClass() == "final")
            {
                if (class_def())
                {
                    if (defs())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "interface")
            {
                if (interface_def())
                {
                    if (defs())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "struct")
            {
                if (struct_def())
                {
                    if (defs())
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "$") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool main_class()
        {
            if (tokens[index].GetClass() == "class" || tokens[index].GetClass() == "abstract" || tokens[index].GetClass() == "static" ||
                tokens[index].GetClass() == "final") 
            {
                string category = "";

                if (class_choice(ref category)) 
                {
                    if (tokens[index].GetClass() == "class") 
                    {
                        index++;

                        string type = "class";

                        if (tokens[index].GetClass() == "ID") 
                        {
                            string name = SemanticAnalyzer.currentClass = tokens[index].GetValue();

                            index++;

                            string parent = "";

                            if (inheritance(ref parent)) 
                            {
                                List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                                if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of class '" + name + "' at line # " + tokens[index].GetLineNo());
                                }

                                if (tokens[index].GetClass() == "{") 
                                {
                                    index++;

                                    SemanticAnalyzer.createScope();

                                    // Lookup

                                    if (tokens[index].GetClass() == "static") 
                                    {
                                        index++;

                                        if (tokens[index].GetClass() == "void") 
                                        {
                                            index++;

                                            if (tokens[index].GetValue() == "Main") 
                                            {
                                                index++;

                                                if (tokens[index].GetClass() == "(") 
                                                {
                                                    index++;

                                                    SemanticAnalyzer.createScope();

                                                    if (tokens[index].GetClass() == ")") 
                                                    {
                                                        index++;

                                                        if (tokens[index].GetClass() == "{") 
                                                        {
                                                            index++;

                                                            if (MST()) 
                                                            {
                                                                SemanticAnalyzer.destroyScope();

                                                                if (tokens[index].GetClass() == "}") 
                                                                {
                                                                    index++;

                                                                    if (class_body(ref refDT)) 
                                                                    {
                                                                        SemanticAnalyzer.destroyScope();

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
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool OE(ref string T2) 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec") 
            {
                string T1 = "";

                if (AE(ref T1)) 
                {
                    if (OEd(T1, ref T2))
                    {
                        if (T2 == "") T2 = T1;

                        return true;
                    }
                }
            }
            return false;
        }

        private bool OEd(string T1, ref string T3)
        {
            if (tokens[index].GetClass() == "OR")
            {
                string opr = tokens[index].GetValue();

                index++;

                string T2 = "";

                if (AE(ref T2))
                {
                    T3 = SemanticAnalyzer.typeCheck(T1, T2, opr);

                    if (T3 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible b/w " + T1 + " and " + T2 + " at line # " + tokens[index].GetLineNo());
                    }

                    string T4 = "";

                    if (OEd(T3, ref T4))
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

        private bool AE(ref string T2)
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                string T1 = "";

                if (RE(ref T1))
                {
                    if (AEd(T1, ref T2))
                    {
                        if (T2 == "") T2 = T1;

                        return true;
                    }
                }
            }
            return false;
        }

        private bool AEd(string T1, ref string T3) 
        {
            if (tokens[index].GetClass() == "AND")
            {
                string opr = tokens[index].GetValue();

                index++;

                string T2 = "";

                if (RE(ref T2))
                {
                    T3 = SemanticAnalyzer.typeCheck(T1, T2, opr);

                    if (T3 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible b/w " + T1 + " and " + T2 + " at line # " + tokens[index].GetLineNo());
                    }

                    string T4 = "";

                    if (AEd(T3, ref T4))
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

        private bool RE(ref string T2) 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                string T1 = "";

                if (E(ref T1))
                {
                    if (REd(T1, ref T2))
                    {
                        if (T2 == "") T2 = T1;

                        return true;
                    }
                }
            }
            return false;
        }

        private bool REd(string T1, ref string T3)
        {
            if (tokens[index].GetClass() == "Relational")
            {
                string opr = tokens[index].GetValue();

                index++;

                string T2 = "";

                if (E(ref T2))
                {
                    T3 = SemanticAnalyzer.typeCheck(T1, T2, opr);

                    if (T3 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible b/w " + T1 + " and " + T2 + " at line # " + tokens[index].GetLineNo());
                    }

                    string T4 = "";

                    if (REd(T3, ref T4))
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

        private bool E(ref string T2) 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                string T1 = "";

                if (T(ref T1))
                {
                    if (Ed(T1, ref T2))
                    {
                        if (T2 == "") T2 = T1;

                        return true;
                    }
                }
            }
            return false;
        }

        private bool Ed(string T1, ref string T3) 
        {
            if (tokens[index].GetClass() == "PM")
            {
                string opr = tokens[index].GetValue();        

                index++;

                string T2 = "";

                if (T(ref T2))
                {
                    T3 = SemanticAnalyzer.typeCheck(T1, T2, opr);

                    if (T3 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible b/w " + T1 + " and " + T2 + " at line # " + tokens[index].GetLineNo());
                    }

                    string T4 = "";

                    if (Ed(T3, ref T4))
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

        private bool T(ref string T2) 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec")
            {
                string T1 = "";

                if (F(ref T1))
                {
                    if (Td(T1, ref T2))
                    {
                        if (T2 == "") T2 = T1;

                        return true;
                    }
                }
            }
            return false;
        }

        private bool Td(string T1, ref string T3) 
        {
            if (tokens[index].GetClass() == "MDM")
            {
                string opr = tokens[index].GetValue();

                index++;

                string T2 = "";

                if (F(ref T2))
                {
                    T3 = SemanticAnalyzer.typeCheck(T1, T2, opr);

                    if (T3 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible b/w " + T1 + " and " + T2 + " at line # " + tokens[index].GetLineNo());
                    }

                    string T4 = "";

                    if (Td(T3, ref T4))
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

        private bool F(ref string T1) //Semantics done only for single variables and constants.
        {
            if (tokens[index].GetClass() == "ID")
            {
                string N = tokens[index].GetValue(); 

                index++;

                T1 = SemanticAnalyzer.Lookup_FT(N);

                if (T1 == String.Empty) 
                {
                    SemanticAnalyzer.errors.Add("Undeclared identifier '" + N + "' at line # " + tokens[index].GetLineNo());
                }

                if (G())
                {
                    return true;
                }
            }
            else if (tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" || tokens[index].GetClass() == "char-constant" ||
                     tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant")
            {

                if (constant(ref T1))
                {
                    return true;
                }
            }
            else if (tokens[index].GetClass() == "(")
            {
                index++;

                string type = "";

                if (OE(ref type))
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

                if (F(ref T1))
                {
                    //T1 = SemanticAnalyzer.typeCheck(T1, "!");

                    //if (T1 == String.Empty) 
                    //{
                    //    SemanticAnalyzer.errors.Add("The operator ! is incompatible with type " + T1 + " at line #" + tokens[index].GetLineNo());
                    //}

                    return true;
                }
            }
            else if (tokens[index].GetClass() == "Inc-Dec") 
            {
                string opr = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    string N = tokens[index].GetValue();

                    T1 = SemanticAnalyzer.Lookup_FT(N);

                    if (T1 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("Undeclared identifier '" + N + "' at line # " + tokens[index].GetLineNo());
                    }

                    index++;

                    if (X()) 
                    {
                        string T2 = SemanticAnalyzer.typeCheck(T1, opr);

                        if (T2 == String.Empty) 
                        {
                            SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible with " + T1 + " type variable at line # " + tokens[index].GetLineNo());
                        }

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

        private bool constant(ref string type) 
        {
            if (tokens[index].GetClass() == "int-constant") 
            {
                type = "int";
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "float-constant")
            {
                type = "float";
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "char-constant")
            {
                type = "char";
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "string-constant")
            {
                type = "string";
                index++;
                return true;
            }
            else if (tokens[index].GetClass() == "bool-constant")
            {
                type = "bool";
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
                    tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "jump" || tokens[index].GetClass() == "skip-stop" || 
                    tokens[index].GetClass() == "Inc-Dec" || tokens[index].GetClass() == "}" || 
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

        private bool asgn_value(string T = "") 
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
                    string type = "";

                    if (OE(ref type))
                    {
                        if (T != type && T != "var")
                        {
                            SemanticAnalyzer.errors.Add("Incompatible assignment b/w " + type + " and " + T + " at line # " + tokens[index].GetLineNo());
                        }

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

                    SemanticAnalyzer.createScope();

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
                                                SemanticAnalyzer.destroyScope();

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


            //Lookup Table

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
                string type = "";
                if (OE(ref type))
                {
                    if (type != "bool")
                    {
                        SemanticAnalyzer.errors.Add("Incompatible type b/w " + type + " and bool at line # " + tokens[index].GetLineNo());
                    }

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

        private bool MST()
        {
            if (tokens[index].GetClass() == "until" || tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type" ||
                tokens[index].GetClass() == "if" || tokens[index].GetClass() == "for" || tokens[index].GetClass() == "foreach" ||
                tokens[index].GetClass() == "toss" || tokens[index].GetClass() == "try" || tokens[index].GetClass() == "const" ||
                tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "jump" || tokens[index].GetClass() == "skip-stop" ||
                tokens[index].GetClass() == "Inc-Dec")
            {
                if (SST())
                {
                    if (MST())
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "}" || tokens[index].GetClass() == "return") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool declaration() 
        {
            if (tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type")
            {
                string T = "";

                if (type(ref T))
                {
                    if (tokens[index].GetClass() == "ID")
                    {
                        string name = tokens[index].GetValue(); 

                        index++;

                        if (!SemanticAnalyzer.Insert_FT(name, T)) 
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                        }

                        if (initialization(T))
                        {
                            if (list(T, "", null))
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

        private bool type(ref string type) 
        {
            if (tokens[index].GetClass() == "var")
            {
                type = "var";

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "Data-Type") 
            {
                type = tokens[index].GetValue();

                index++;

                return true;
            }

            return false;
        }

        private bool initialization(string T)
        {
            if (tokens[index].GetClass() == "=")
            {
                index++;

                if (asgn_value(T)) 
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

        private bool list(string type, string typeModifier, List<DataTable> link) 
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
                    string name = tokens[index].GetValue();

                    index++;

                    if (link != null) 
                    {
                        if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                        }
                    }

                    else if (!SemanticAnalyzer.Insert_FT(name, type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (initialization(type)) 
                    {
                        if (list(type, typeModifier, link)) 
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
                    SemanticAnalyzer.createScope();

                    index++;

                    string type = "";

                    if (ID(ref type)) 
                    {
                        if (tokens[index].GetClass() == "ID") 
                        {
                            string name = tokens[index].GetValue();

                            index++;

                            if (!SemanticAnalyzer.Insert_FT(name, type))
                            {
                                SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                            }

                            if (tokens[index].GetClass() == "in") 
                            {
                                index++;

                                //Lookup

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
                                                    SemanticAnalyzer.destroyScope();

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

        private bool ID(ref string type) 
        {
            if (tokens[index].GetClass() == "Data-Type")
            {
                type = tokens[index].GetValue();

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "ID") 
            {
                type = tokens[index].GetValue();

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

                    string type = "";

                    if (OE(ref type)) 
                    {
                        if (type != "bool")
                        {
                            SemanticAnalyzer.errors.Add("Incompatible type b/w " + type + " and bool at line # " + tokens[index].GetLineNo());
                        }

                        if (tokens[index].GetClass() == ")") 
                        {
                            index++;

                            if (tokens[index].GetClass() == "{") 
                            {
                                SemanticAnalyzer.createScope();

                                index++;

                                if (MST()) 
                                {
                                    SemanticAnalyzer.destroyScope();

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

                    string type = "";

                    if (OE(ref type)) 
                    {
                        if (type != "bool")
                        {
                            SemanticAnalyzer.errors.Add("Incompatible type b/w " + type + " and bool at line # " + tokens[index].GetLineNo());
                        }

                        if (tokens[index].GetClass() == ")") 
                        {
                            index++;

                            if (tokens[index].GetClass() == "{") 
                            {
                                SemanticAnalyzer.createScope();

                                index++;

                                if (MST()) 
                                {
                                    SemanticAnalyzer.destroyScope();

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

                    string type = "";

                    if (OE(ref type))
                    {
                        if (type != "bool")
                        {
                            SemanticAnalyzer.errors.Add("Incompatible type b/w " + type + " and bool at line # " + tokens[index].GetLineNo());
                        }

                        if (tokens[index].GetClass() == ")")
                        {
                            index++;

                            if (tokens[index].GetClass() == "{")
                            {
                                SemanticAnalyzer.createScope();

                                index++;

                                if (MST())
                                {
                                    SemanticAnalyzer.destroyScope();

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
                    tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "jump" || tokens[index].GetClass() == "skip-stop" ||
                    tokens[index].GetClass() == "Inc-Dec" || tokens[index].GetClass() == "}") 
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
                    SemanticAnalyzer.createScope();

                    index++;

                    if (MST()) 
                    {
                        SemanticAnalyzer.destroyScope();

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

        private bool toss() 
        {
            if (tokens[index].GetClass() == "toss") 
            {
                index++;

                if (tokens[index].GetClass() == "new") 
                {
                    index++;

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

                                    if (tokens[index].GetClass() == ";") 
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

        private bool try_statement() 
        {
            if (tokens[index].GetClass() == "try") 
            {
                index++;

                if (tokens[index].GetClass() == "{") 
                {
                    SemanticAnalyzer.createScope();

                    index++;

                    if (MST())
                    {
                        SemanticAnalyzer.destroyScope();

                        if (tokens[index].GetClass() == "}") 
                        {
                            index++;

                            if (catch_statement()) 
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool catch_statement()
        {
            if (tokens[index].GetClass() == "catch") 
            {
                index++;

                if (tokens[index].GetClass() == "(") 
                {
                    SemanticAnalyzer.createScope();

                    index++;

                    if (tokens[index].GetClass() == "ID") 
                    {
                        string type = tokens[index].GetValue();

                        index++;

                        string name = ""; 

                        if (ID_opt(ref name)) 
                        {
                            if (tokens[index].GetClass() == ")") 
                            {
                                index++;

                                if (!SemanticAnalyzer.Insert_FT(name, type))
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                                }

                                if (tokens[index].GetClass() == "{") 
                                {
                                    index++;

                                    if (MST()) 
                                    {
                                        SemanticAnalyzer.destroyScope();

                                        if (tokens[index].GetClass() == "}") 
                                        {
                                            index++;

                                            if (finally_statement()) 
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
            }

            return false;
        }

        private bool finally_statement()
        {
            if (tokens[index].GetClass() == "finally")
            {
                index++;

                if (tokens[index].GetClass() == "{")
                {
                    SemanticAnalyzer.createScope();

                    index++;

                    if (MST_finally())
                    {
                        SemanticAnalyzer.destroyScope();

                        if (tokens[index].GetClass() == "}")
                        {
                            index++;

                            return true;
                        }
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "until" || tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type" ||
                    tokens[index].GetClass() == "if" || tokens[index].GetClass() == "for" || tokens[index].GetClass() == "foreach" ||
                    tokens[index].GetClass() == "toss" || tokens[index].GetClass() == "try" || tokens[index].GetClass() == "const" ||
                    tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "jump" || tokens[index].GetClass() == "skip-stop" ||
                    tokens[index].GetClass() == "Inc-Dec" || tokens[index].GetClass() == "}")
                {
                    return true;
                }
            }

            return false;
        }

        private bool MST_finally()
        {
            if (tokens[index].GetClass() == "until" || tokens[index].GetClass() == "var" || tokens[index].GetClass() == "Data-Type" ||
                tokens[index].GetClass() == "if" || tokens[index].GetClass() == "for" || tokens[index].GetClass() == "foreach" ||
                tokens[index].GetClass() == "toss" || tokens[index].GetClass() == "try" || tokens[index].GetClass() == "const" ||
                tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "Inc-Dec")
            {
                if (SST_finally())
                {
                    if (MST_finally())
                    {
                        return true;
                    }
                }
            }

            else
            {
                if (tokens[index].GetClass() == "}" || tokens[index].GetClass() == "return")
                {
                    return true;
                }
            }

            return false;
        }

        private bool ID_opt(ref string name) 
        {
            if (tokens[index].GetClass() == "ID")
            {
                name = tokens[index].GetValue();

                index++;

                return true;
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

        private bool skip_stop() 
        {
            if (tokens[index].GetClass() == "skip-stop") 
            {
                index++;

                if (tokens[index].GetClass() == ";") 
                {
                    index++;

                    return true;
                }
            }

            return false;
        }

        private bool jump() 
        {
            if (tokens[index].GetClass() == "jump")
            {
                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    index++;

                    if (tokens[index].GetClass() == ";") 
                    {
                        index++;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool const_statement(ref string name, ref string type, List<DataTable> link) 
        {
            if (tokens[index].GetClass() == "const") 
            {
                string typeModifier = "const";

                index++;

                if (tokens[index].GetClass() == "Data-Type") 
                {
                    type = tokens[index].GetValue();

                    index++;

                    if (tokens[index].GetClass() == "ID") 
                    {
                        name = tokens[index].GetValue();

                        index++;

                        if (const_initialization(type)) 
                        {
                            if (const_list(type, typeModifier, link)) 
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool const_initialization(string T) 
        {
            if (tokens[index].GetClass() == "=") 
            {
                index++;

                string type = "";

                if (OE(ref type)) 
                {
                    if (T != type)
                    {
                        SemanticAnalyzer.errors.Add("Incompatible assignment b/w " + type + " and " + T + " at line # " + tokens[index].GetLineNo());
                    }

                    return true;
                }
            }

            return false;
        }

        private bool const_list(string type, string typeModifier, List<DataTable> link) 
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
                    string name = tokens[index].GetValue();

                    index++;

                    if (link != null) 
                    {
                        if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                        }
                    }

                    else if (!SemanticAnalyzer.Insert_FT(name, "const " + type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (const_initialization(type)) 
                    {
                        if (const_list(type, typeModifier, link)) 
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool init(string T) 
        {
            if (tokens[index].GetClass() == "ID" || tokens[index].GetClass() == "int-constant" || tokens[index].GetClass() == "float-constant" ||
                tokens[index].GetClass() == "char-constant" || tokens[index].GetClass() == "string-constant" || tokens[index].GetClass() == "bool-constant" ||
                tokens[index].GetClass() == "(" || tokens[index].GetClass() == "!" || tokens[index].GetClass() == "Inc-Dec") 
            {
                string type = "";

                if (OE(ref type)) 
                {
                    if (T != type && T != "var")
                    {
                        SemanticAnalyzer.errors.Add("Incompatible assignment b/w " + type + " and " + T + " at line # " + tokens[index].GetLineNo());
                    }

                    return true;
                }
            }

            else if (tokens[index].GetClass() == "new") 
            {
                index++;

                if (constructor()) 
                {
                    return true;
                }
            }

            return false;
        }

        private bool Z(string type) 
        {
            if (tokens[index].GetClass() == "(")
            {
                index++;

                if (PL())
                {
                    if (tokens[index].GetClass() == ")")
                    {
                        index++;

                        if (Z1())
                        {
                            return true;
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == ".")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (Z(type))
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "Inc-Dec")
            {
                string opr = tokens[index].GetValue();

                index++;

                string T2 = SemanticAnalyzer.typeCheck(type, opr);

                if (T2 == String.Empty)
                {
                    SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible with " + type + " type variable at line # " + tokens[index].GetLineNo());
                }

                if (tokens[index].GetClass() == ";")
                {
                    index++;

                    return true;
                }
            }

            else if (tokens[index].GetClass() == ";")
            {
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "ID")
            {
                string name = tokens[index].GetValue();

                index++;

                if (!SemanticAnalyzer.Insert_FT(name, type))
                {
                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                }

                if (constructor_choice())
                {
                    if (tokens[index].GetClass() == ";")
                    {
                        index++;

                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "=") 
            {
                index++;

                if (init(type)) 
                {
                    if (tokens[index].GetClass() == ";") 
                    {
                        index++;

                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == ":")
            {
                index++;

                if (SST())
                {
                    return true;
                }
            }

            return false;
        }

        private bool constructor_choice()
        {
            if (tokens[index].GetClass() == "=")
            {
                index++;

                if (tokens[index].GetClass() == "new")
                {
                    index++;

                    if (constructor())
                    {
                        return true;
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

        private bool Z1()
        {
            if (tokens[index].GetClass() == ";")
            {
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == ".") 
            {
                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    index++;

                    if (Z(String.Empty)) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SST() 
        {
            if (tokens[index].GetClass() == "until")
            {
                if (until_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "var")
            {
                string type = "var";

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    if (!SemanticAnalyzer.Insert_FT(name, type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (tokens[index].GetClass() == "=")
                    {
                        index++;

                        //Lookup

                        if (init(type))
                        {
                            if (tokens[index].GetClass() == ";") 
                            {
                                index++;

                                return true;
                            }
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "Data-Type")
            {
                string type = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    if (!SemanticAnalyzer.Insert_FT(name, type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (initialization(type))
                    {
                        if (list(type, "", null))
                        {
                            return true;
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "if")
            {
                if (if_elif_else())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "for")
            {
                if (for_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "foreach")
            {
                if (foreach_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "toss")
            {
                if (toss())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "try")
            {
                if (try_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "const")
            {
                string name = "", type = "";

                if (const_statement(ref name, ref type, null))
                {
                    if (!SemanticAnalyzer.Insert_FT(name, "const " + type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    return true;
                }
            }

            else if (tokens[index].GetClass() == "ID")
            {
                string N = tokens[index].GetValue();

                string type = SemanticAnalyzer.Lookup_FT(N);

                if (type == "") 
                {
                    SemanticAnalyzer.errors.Add("Semantic Error: Undeclared Identifier '" + N + "' at line # " + tokens[index].GetLineNo());
                }

                index++;

                if (Z(type))
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "jump")
            {
                if (jump())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "Inc-Dec")
            {
                string opr = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string N = tokens[index].GetValue();

                    string T1 = SemanticAnalyzer.Lookup_FT(N);

                    if (T1 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("Undeclared identifier '" + N + "' at line # " + tokens[index].GetLineNo());
                    }

                    string T2 = SemanticAnalyzer.typeCheck(T1, opr);

                    if (T2 == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("The operator " + opr + " is incompatible with " + T1 + " type variable at line # " + tokens[index].GetLineNo());
                    }

                    index++;

                    if (X())
                    {
                        if (tokens[index].GetClass() == ";")
                        {
                            index++;

                            return true;
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "skip-stop") 
            {
                if (skip_stop()) 
                {
                    return true;
                }
            }

            return false;
        }

        private bool SST_finally() 
        {
            if (tokens[index].GetClass() == "until")
            {
                if (until_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "var")
            {
                string type = "var";

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    if (!SemanticAnalyzer.Insert_FT(name, type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (tokens[index].GetClass() == "=")
                    {
                        index++;

                        //Lookup

                        if (init(type))
                        {
                            if (tokens[index].GetClass() == ";")
                            {
                                index++;

                                return true;
                            }
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "Data-Type")
            {
                string type = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    if (!SemanticAnalyzer.Insert_FT(name, type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (initialization(type))
                    {
                        if (list(type, "", null))
                        {
                            return true;
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "if")
            {
                if (if_elif_else())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "for")
            {
                if (for_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "foreach")
            {
                if (foreach_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "toss")
            {
                if (toss())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "try")
            {
                if (try_statement())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "const")
            {
                string name = "", type = "";

                if (const_statement(ref name, ref type, null))
                {
                    if (!SemanticAnalyzer.Insert_FT(name, "const " + type))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    return true;
                }
            }

            else if (tokens[index].GetClass() == "ID")
            {
                string type = tokens[index].GetValue();

                index++;

                if (Z(type))
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
                        if (tokens[index].GetClass() == ";") 
                        {
                            index++;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool struct_def() 
        {
            if (tokens[index].GetClass() == "struct") 
            {
                string category = "general";

                index++;

                string type = "struct";

                if (tokens[index].GetClass() == "ID") 
                {
                    string name = SemanticAnalyzer.currentClass = tokens[index].GetValue();

                    index++;

                    string parent = "none";

                    List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                    if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of struct '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (struct_body(ref refDT))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool struct_body(ref List<DataTable> link)
        {
            if (tokens[index].GetClass() == "{")
            {
                SemanticAnalyzer.createScope();

                index++;

                if (struct_content(ref link))
                {
                    SemanticAnalyzer.destroyScope();

                    if (tokens[index].GetClass() == "}") 
                    {
                        index++;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool struct_content(ref List<DataTable> link)
        {
            if (tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "const" || tokens[index].GetClass() == "static" ||
                tokens[index].GetClass() == "void" || tokens[index].GetClass() == "struct")
            {
                string name = "", type = "", typeModifier = "";

                if (struct_content2(ref name, ref type, ref typeModifier, link))
                {
                    if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (struct_content(ref link))
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "}") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool struct_content2(ref string name, ref string type, ref string typeModifer, List<DataTable> link)
        {
            if (tokens[index].GetClass() == "const")
            {
                typeModifer = "const";

                if (const_statement(ref name, ref type, link))
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "struct")
            {
                if (struct_def())
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "static" || tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "void") 
            {
                typeModifer = "null";

                if (static_choice(ref typeModifer)) 
                {
                    if (struct_content3(ref name, ref type, typeModifer, link)) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool struct_content3(ref string name, ref string type, string typeModifier, List<DataTable> link)
        {
            if (tokens[index].GetClass() == "Data-Type")
            {
                type = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    name = tokens[index].GetValue();

                    index++;

                    if (DT(ref type, typeModifier, link))
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "void") 
            {
                type = "void";

                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    name = tokens[index].GetValue();

                    index++;

                    if (tokens[index].GetClass() == "(") 
                    {
                        SemanticAnalyzer.createScope();

                        index++;

                        if (PL_def(ref type)) 
                        {
                            if (tokens[index].GetClass() == ")") 
                            {
                                index++;

                                if (tokens[index].GetClass() == "{") 
                                {

                                    index++;

                                    if (MST()) 
                                    {
                                        SemanticAnalyzer.destroyScope();

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

            return false;
        }

        private bool static_choice(ref string typeModifer) 
        {
            if (tokens[index].GetClass() == "static")
            {
                typeModifer = "static";

                index++;

                return true;
            }

            else 
            {
                if (tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "void") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool DT(ref string returnType, string typeModifier, List<DataTable> link) 
        {
            if (tokens[index].GetClass() == "=" || tokens[index].GetClass() == "," || tokens[index].GetClass() == ";")
            {
                if (initialization(returnType))
                {
                    if (list(returnType, typeModifier, link))
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "(") 
            {
                index++;

                SemanticAnalyzer.createScope();

                if (PL_def(ref returnType)) 
                {
                    if (tokens[index].GetClass() == ")") 
                    {
                        index++;                      

                        if (tokens[index].GetClass() == "{") 
                        {
                            index++;

                            if (MST()) 
                            {
                                if (return_statement(returnType)) 
                                {
                                    SemanticAnalyzer.destroyScope();

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

        private bool return_statement(string T)
        {
            if (tokens[index].GetClass() == "return") 
            {
                index++;

                string type = "";
                T = T.Substring(T.IndexOf("->") + 2);

                if (OE(ref type)) 
                {
                    if (T != type)
                    {
                        SemanticAnalyzer.errors.Add("Incompatible return type b/w " + type + " and " + T + " at line # " + tokens[index].GetLineNo());
                    }

                    if (tokens[index].GetClass() == ";") 
                    {
                        index++;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool PL_def(ref string returnType) 
        {
            if (tokens[index].GetClass() == "ref" || tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "ID")
            {
                string parameters = "";

                if (ref_choice(ref parameters))
                {
                    if (P_choice(ref parameters))
                    {
                        if (tokens[index].GetClass() == "ID")
                        {
                            string name = tokens[index].GetValue();

                            index++;

                            if (!SemanticAnalyzer.Insert_FT(name, parameters)) 
                            {
                                SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                            }

                            if (PL_def2(ref parameters))
                            {
                                returnType = parameters + "->" + returnType;

                                return true;
                            }
                        }
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == ")") 
                {
                    returnType = "->" + returnType;

                    return true;
                }
            }

            return false;
        }

        private bool PL_def2(ref string parameters) 
        {
            if (tokens[index].GetClass() == ",")
            {
                parameters += ",";

                index++;

                if (ref_choice(ref parameters))
                {
                    if (P_choice(ref parameters))
                    {
                        string[] param = parameters.Split(','); 

                        if (tokens[index].GetClass() == "ID")
                        {
                            string name = tokens[index].GetValue();

                            index++;

                            if (!SemanticAnalyzer.Insert_FT(name, param[param.Length - 1]))
                            {
                                SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                            }

                            if (PL_def2(ref parameters))
                            {
                                return true;
                            }
                        }
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

        private bool P_choice(ref string parameters)
        {
            if (tokens[index].GetClass() == "ID") 
            {
                parameters += tokens[index].GetValue();

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "Data-Type") 
            {
                parameters += tokens[index].GetValue();

                index++;

                return true;
            }

            return false;
        }

        private bool ref_choice(ref string parameters) 
        {
            if (tokens[index].GetClass() == "ref")
            {
                parameters += "ref ";

                index++;

                return true;
            }

            else 
            {
                if (tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "ID") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool interface_def() 
        {
            if (tokens[index].GetClass() == "interface") 
            {
                string category = "general";

                index++;

                string type = "interface";

                if (tokens[index].GetClass() == "ID") 
                {
                    string name = SemanticAnalyzer.currentClass = tokens[index].GetValue();

                    index++;

                    string parent = "";

                    if (inheritance(ref parent)) 
                    {
                        List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                        if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of interface '" + name + "' at line # " + tokens[index].GetLineNo());
                        }

                        if (tokens[index].GetClass() == "{") 
                        {
                            SemanticAnalyzer.createScope();

                            index++;

                            if (func_prototypes(ref refDT)) 
                            {
                                SemanticAnalyzer.destroyScope();

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

            return false;
        }

        private bool inheritance(ref string parent) 
        {
            if (tokens[index].GetClass() == ":")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    string category = "";

                    string type = SemanticAnalyzer.Lookup_MT(name, out category);

                    if (type == String.Empty) 
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Undeclared Identifier '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (type == "struct") 
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Class cannot be inherited from struct at line # " + tokens[index].GetLineNo());
                    }

                    if (type == "class" && category == "final") 
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Final class cannot be inherited at line # " + tokens[index].GetLineNo());
                    }

                    parent = name;

                    if (inheritance2(ref parent))
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "{") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool inheritance2(ref string parent)
        {
            if (tokens[index].GetClass() == ",")
            {
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    string category = "";

                    string type = SemanticAnalyzer.Lookup_MT(name, out category);

                    if (type == String.Empty)
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Undeclared Identifier '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (type == "struct")
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Class cannot be inherited from struct!");
                    }

                    if (type == "class")
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Class must come first before any interface / Multiple classes cannot be inherited at line # " + tokens[index].GetLineNo());
                    }

                    parent = name;

                    if (inheritance2(ref parent))
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "{")
                {
                    return true;
                }
            }

            return false;
        }

        private bool func_prototypes(ref List<DataTable> link) 
        {
            if (tokens[index].GetClass() == "void" || tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "virtual" || 
                tokens[index].GetClass() == "static" || tokens[index].GetClass() == "override" || tokens[index].GetClass() == "final")
            {
                string typeModifier = "";

                if (func_choice(ref typeModifier)) 
                {
                    string type = "";

                    if (return_type(ref type))
                    {
                        if (tokens[index].GetClass() == "ID")
                        {
                            string name = tokens[index].GetValue();

                            index++;

                            if (tokens[index].GetClass() == "(")
                            {
                                index++;

                                if (PL_def(ref type))
                                {
                                    if (tokens[index].GetClass() == ")")
                                    {
                                        index++;

                                        if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                                        {
                                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                                        }

                                        if (tokens[index].GetClass() == ";")
                                        {
                                            index++;

                                            if (func_prototypes(ref link))
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
            }

            else 
            {
                if (tokens[index].GetClass() == "}") 
                {
                    return true;
                }
            }

            return false;
        }

        private bool func_choice(ref string typeModifier)
        {
            if (tokens[index].GetClass() == "virtual")
            {
                typeModifier = "virtual";

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "override")
            {
                typeModifier = "override";

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "final")
            {
                typeModifier = "final";

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "static")
            {
                typeModifier = "static";

                index++;

                return true;
            }

            else 
            {
                typeModifier = "null";

                if (tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "void")
                {
                    return true;
                }
            }

            return false;
        }

        private bool return_type(ref string type)
        {
            if (tokens[index].GetClass() == "void")
            {
                type = "void";

                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "Data-Type") 
            {
                type = tokens[index].GetValue();

                index++;

                return true;
            }

            return false;
        }

        private bool class_def() 
        {
            if (tokens[index].GetClass() == "class" || tokens[index].GetClass() == "static" || tokens[index].GetClass() == "final" ||
                tokens[index].GetClass() == "abstract") 
            {
                string category = "";

                if (class_choice(ref category)) 
                {
                    if (tokens[index].GetClass() == "class") 
                    {
                        index++;

                        string type = "class";

                        if (tokens[index].GetClass() == "ID") 
                        {
                            string name = SemanticAnalyzer.currentClass = tokens[index].GetValue();

                            index++;

                            string parent = "";

                            if (inheritance(ref parent)) 
                            {
                                List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                                if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT)) 
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of class '" + name + "' at line # " + tokens[index].GetLineNo());
                                }

                                if (tokens[index].GetClass() == "{") 
                                {
                                    index++;

                                    SemanticAnalyzer.createScope();

                                    if (class_body(ref refDT)) 
                                    {
                                        SemanticAnalyzer.destroyScope();

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

            return false;
        }

        private bool class_choice(ref string category)
        {
            if (tokens[index].GetClass() == "static")
            {
                category = "static";
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "abstract")
            {
                category = "abstract";
                index++;

                return true;
            }

            else if (tokens[index].GetClass() == "final")
            {
                category = "final";
                index++;

                return true;
            }

            else 
            {
                if (tokens[index].GetClass() == "class") 
                {
                    category = "general";
                    return true;
                }
            }

            return false;
        }

        private bool class_body(ref List<DataTable> link) 
        {
            if (tokens[index].GetClass() == "void" || tokens[index].GetClass() == "Data-Type" || tokens[index].GetClass() == "virtual" ||
                tokens[index].GetClass() == "static" || tokens[index].GetClass() == "override" || tokens[index].GetClass() == "final")
            {
                string typeModifier = "";

                if (func_choice(ref typeModifier))
                {
                    if (class_body1(typeModifier, link))
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "abstract")
            {
                string typeModifier = "abstract";

                index++;

                string type = "";

                if (return_type(ref type))
                {
                    if (tokens[index].GetClass() == "ID")
                    {
                        string name = tokens[index].GetValue();

                        index++;

                        if (tokens[index].GetClass() == "(")
                        {
                            index++;

                            if (PL_def(ref type))
                            {
                                if (tokens[index].GetClass() == ")")
                                {
                                    index++;

                                    if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link)) 
                                    {
                                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                                    }

                                    if (tokens[index].GetClass() == ";")
                                    {
                                        index++;

                                        if (class_body(ref link))
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

            else if (tokens[index].GetClass() == "interface")
            {
                if (interface_def())
                {
                    if (class_body(ref link))
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "class")
            {
                string category = "general";

                index++;

                string type = "class";

                if (tokens[index].GetClass() == "ID")
                {
                    string name = SemanticAnalyzer.currentClass = tokens[index].GetValue();

                    index++;

                    string parent = "";

                    if (inheritance(ref parent))
                    {
                        List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                        if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of class '" + name + "' at line # " + tokens[index].GetLineNo());
                        }

                        if (tokens[index].GetClass() == "{")
                        {
                            index++;

                            SemanticAnalyzer.createScope();

                            if (class_body(ref refDT))
                            {
                                SemanticAnalyzer.destroyScope();

                                if (tokens[index].GetClass() == "}")
                                {
                                    index++;

                                    if (class_body(ref link))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "const")
            {
                string type = "", name = "";
                string typeModifier = "const";

                if (const_statement(ref name, ref type, link))
                {
                    if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                    }

                    if (class_body(ref link))
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "struct")
            {
                if (struct_def())
                {
                    if (class_body(ref link))
                    {
                        return true;
                    }
                }
            }

            else 
            {
                if (tokens[index].GetClass() == "}") 
                {
                    return true;
                }
            }
            
            return false;
        }

        private bool class_body1(string typeModifier, List<DataTable> refDT)
        {
            if (tokens[index].GetClass() == "Data-Type")
            {
                string returnType = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    string name = tokens[index].GetValue();

                    index++;

                    if (DT(ref returnType, typeModifier, refDT))
                    {
                        if (!SemanticAnalyzer.Insert_DT(name, returnType, typeModifier, refDT)) 
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                        }

                        if (class_body(ref refDT))
                        {
                            return true;
                        }
                    }
                }
            }

            else if (tokens[index].GetClass() == "void") 
            {
                string returnType = "void";

                index++;

                if (tokens[index].GetClass() == "ID") 
                {
                    string name = tokens[index].GetValue();

                    index++;                   

                    if (tokens[index].GetClass() == "(") 
                    {
                        index++;

                        SemanticAnalyzer.createScope();

                        if (PL_def(ref returnType)) 
                        {
                            if (tokens[index].GetClass() == ")") 
                            {
                                if (!SemanticAnalyzer.Insert_DT(name, returnType, typeModifier, refDT))
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "' at line # " + tokens[index].GetLineNo());
                                }

                                index++;

                                if (tokens[index].GetClass() == "{") 
                                {
                                    index++;

                                    if (MST()) 
                                    {
                                        SemanticAnalyzer.destroyScope();

                                        if (tokens[index].GetClass() == "}") 
                                        {
                                            index++;

                                            if (class_body(ref refDT)) 
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
            }

            return false;
        }
    }
}

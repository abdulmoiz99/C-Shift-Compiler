﻿using System;
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
                            string name = tokens[index].GetValue();

                            index++;

                            string parent = "";

                            if (inheritance(ref parent)) 
                            {
                                List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                                if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of class '" + name + "'!");
                                }

                                if (tokens[index].GetClass() == "{") 
                                {
                                    index++;

                                    SemanticAnalyzer.createScope();

                                    //FT Pending

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
                    index++;

                    if (MST())
                    {
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
                    index++;

                    if (tokens[index].GetClass() == "ID") 
                    {
                        index++;

                        if (ID_opt()) 
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
                    index++;

                    if (MST_finally())
                    {
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

        private bool ID_opt() 
        {
            if (tokens[index].GetClass() == "ID")
            {
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

        private bool const_statement(ref string name, ref string type) 
        {
            if (tokens[index].GetClass() == "const") 
            {
                index++;

                if (tokens[index].GetClass() == "Data-Type") 
                {
                    type = tokens[index].GetValue();

                    index++;

                    if (tokens[index].GetClass() == "ID") 
                    {
                        name = tokens[index].GetValue();

                        index++;

                        if (const_initialization()) 
                        {
                            if (const_list()) 
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool const_initialization() 
        {
            if (tokens[index].GetClass() == "=") 
            {
                index++;

                if (OE()) 
                {
                    return true;
                }
            }

            return false;
        }

        private bool const_list() 
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

                    if (const_initialization()) 
                    {
                        if (const_list()) 
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool init() 
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

        private bool Z() 
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

                    if (Z())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "Inc-Dec")
            {
                index++;

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
                index++;

                if (tokens[index].GetClass() == "=")
                {
                    index++;

                    if (tokens[index].GetClass() == "new")
                    {
                        index++;

                        if (constructor())
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

            else if (tokens[index].GetClass() == "=") 
            {
                index++;

                if (OE()) 
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

                    if (Z()) 
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
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (tokens[index].GetClass() == "=")
                    {
                        index++;

                        if (init())
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
                string name = "", type = ""; //Pending
                if (const_statement(ref name, ref type))
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "ID")
            {
                index++;

                if (Z())
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
                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    index++;

                    if (tokens[index].GetClass() == "=")
                    {
                        index++;

                        if (init())
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
                string name = "", type = ""; //Pending
                if (const_statement(ref name, ref type))
                {
                    return true;
                }
            }

            else if (tokens[index].GetClass() == "ID")
            {
                index++;

                if (Z())
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
                    string name = tokens[index].GetValue();

                    index++;

                    string parent = "none";

                    List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                    if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of struct '" + name + "'!");
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

                if (struct_content2(ref name, ref type, ref typeModifier))
                {
                    if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "'!");
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

        private bool struct_content2(ref string name, ref string type, ref string typeModifer)
        {
            if (tokens[index].GetClass() == "const")
            {
                typeModifer = "const";

                if (const_statement(ref name, ref type))
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
                    if (struct_content3(ref name, ref type)) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool struct_content3(ref string name, ref string type)
        {
            if (tokens[index].GetClass() == "Data-Type")
            {
                type = tokens[index].GetValue();

                index++;

                if (tokens[index].GetClass() == "ID")
                {
                    name = tokens[index].GetValue();

                    index++;

                    if (DT(ref type))
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
                        index++;

                        if (PL_def(ref type)) 
                        {
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

        private bool DT(ref string returnType) 
        {
            if (tokens[index].GetClass() == "=" || tokens[index].GetClass() == "," || tokens[index].GetClass() == ";")
            {
                if (initialization())
                {
                    if (list())
                    {
                        return true;
                    }
                }
            }

            else if (tokens[index].GetClass() == "(") 
            {
                index++;

                if (PL_def(ref returnType)) 
                {
                    if (tokens[index].GetClass() == ")") 
                    {
                        index++;

                        if (tokens[index].GetClass() == "{") 
                        {
                            SemanticAnalyzer.createScope();

                            index++;

                            if (MST()) 
                            {
                                if (return_statement()) 
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

        private bool return_statement()
        {
            if (tokens[index].GetClass() == "return") 
            {
                index++;

                if (OE()) 
                {
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
                            index++;

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
                        if (tokens[index].GetClass() == "ID")
                        {
                            index++;

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
                    string name = tokens[index].GetValue();

                    index++;

                    string parent = "";

                    if (inheritance(ref parent)) 
                    {
                        List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                        if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of interface '" + name + "'!");
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
                        SemanticAnalyzer.errors.Add("Semantic Error: Undeclared Identifier '" + name + "'!");
                    }

                    if (type == "struct") 
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Class cannot be inherited from struct!");
                    }

                    if (type == "class" && category == "final") 
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Final class cannot be inherited!");
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
                        SemanticAnalyzer.errors.Add("Semantic Error: Undeclared Identifier '" + name + "'!");
                    }

                    if (type == "struct")
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Class cannot be inherited from struct!");
                    }

                    if (type == "class")
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Class must come first before any interface / Multiple classes cannot be inherited!");
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
                                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "'!");
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
                            string name = tokens[index].GetValue();

                            index++;

                            string parent = "";

                            if (inheritance(ref parent)) 
                            {
                                List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                                if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT)) 
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of class '" + name + "'!");
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
                                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "'!");
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
                    string name = tokens[index].GetValue();

                    index++;

                    string parent = "";

                    if (inheritance(ref parent))
                    {
                        List<DataTable> refDT = SemanticAnalyzer.Create_DT();

                        if (!SemanticAnalyzer.Insert_MT(name, type, category, parent, refDT))
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of class '" + name + "'!");
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

                if (const_statement(ref name, ref type))
                {
                    if (!SemanticAnalyzer.Insert_DT(name, type, typeModifier, link))
                    {
                        SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "'!");
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

                    if (DT(ref returnType))
                    {
                        if (!SemanticAnalyzer.Insert_DT(name, returnType, typeModifier, refDT)) 
                        {
                            SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "'!");
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

                        if (PL_def(ref returnType)) 
                        {
                            if (tokens[index].GetClass() == ")") 
                            {
                                if (!SemanticAnalyzer.Insert_DT(name, returnType, typeModifier, refDT))
                                {
                                    SemanticAnalyzer.errors.Add("Semantic Error: Redeclaration of '" + name + "'!");
                                }

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

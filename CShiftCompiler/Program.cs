﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static void Main()
        {
            
            //tokens list
            List<string> tokens = new List<string>();

            char[] punctuators = { ';', ',', ':', '(', ')', '{', '}', '[', ']' }; //Dot Excluded

            //Reading input files
            var reader = new StreamReader(Application.StartupPath + @"\Input\input1.txt");
            var cr = reader.ReadToEnd().ToCharArray();

            //Reading 
            for (int i = 0; i < cr.Count(); i++)
            {
                //FOR STRING
                if (cr[i] == '"')
                {
                    i = ValidateString(tokens, cr, i);
                }
                //FOR CHARACTERS
                else if (cr[i] == '\'') // check if the character is '
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
                }
                //FOR MULTI LINE COMMENTS                     Invalid Lexeme expected?
                else if (cr[i] == '/' && i + 1 < cr.Length && cr[i + 1] == '*' && i + 2 < cr.Length)
                {
                    i += 2;

                    while (cr[i] != '*' && i + 1 < cr.Length && cr[i + 1] != '/')
                    {
                        i++;
                    }
                }
                //FOR SPACE
                else if (cr[i] == ' ')
                {
                    if (Temp.Length() > 0) tokens.Add(Temp.Empty());
                    if (i + 1 < cr.Length) i++;
                }
                //FOR PUNCTUATORS
                else if (punctuators.Contains(cr[i]))
                {
                    if (Temp.Length() > 0) tokens.Add(Temp.Empty()); //Check
                    tokens.Add(cr[i].ToString());
                    if (i + 1 < cr.Length) i++;                   
                }            
            }

            //To display words
            foreach (var item in tokens)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }

        private static int ValidateString(List<string> tokens, char[] cr, int pointer)
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
                        tokens.Add(Temp.Empty());
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

            } while (cr[pointer] != '"' && cr[pointer] != '\n' );//Break Conditions - \n(New Line), ", end of file
            tokens.Add(Temp.Empty());
            return pointer;
        }
        private static int ValidateCharacter(List<string> tokens, char[] cr, int pointer)
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
            tokens.Add(Temp.Empty());
            return pointer;
        }
    }
}

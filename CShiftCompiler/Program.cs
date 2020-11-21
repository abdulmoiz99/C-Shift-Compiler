using System;
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

            //
            char[] Punctuators = { ';', ',', ':', '.', '(', ')', '{', '}', '[', ']' };

            var temp = string.Empty;

            //Reading input files
            var reader = new StreamReader(Application.StartupPath + @"\Input\input1.txt");
            // string testString = "ksdk sdlsdsdd";
            //var cr = testString.ToCharArray();
            var cr = reader.ReadToEnd().ToCharArray();

            //Reading 
            for (int i = 0; i < cr.Count(); i++)
            {
                //FOR STRING
                // Start Conditions - "
                if (cr[i] == '"')
                {
                    do // For appending first inverted comma
                    {
                        // Include Conditions ("\") - increment for the next character
                        if (cr[i] == '\\')
                        {
                            if (!string.IsNullOrEmpty(cr[i + 1].ToString()))
                            {
                                Temp.Add(cr[i++]);
                            }
                            else
                            {
                                tokens.Add(Temp.Empty());
                                break;
                            }
                        }

                        Temp.Add(cr[i++]);

                        //Adding the last inverted comma
                        if (cr[i] == '"') Temp.Add(cr[i]);

                    } while (cr[i] != '"' && i < cr.Count() - 1 && cr[i] != '\n');//Break Conditions - \n(New Line), ", end of file
                    tokens.Add(Temp.Empty());
                }
                //FOR CHARACTERS

                if (cr[i] == '\'') // checke if the character is '
                {
                }
































                //    Temp.Add(cr[i]);
                //    i++;
                //    while (cr[i] != '"' && i < cr.Count() - 1)
                //    {
                //        if (cr[i] == '\\') //Single Slash ('\')
                //        {
                //            Temp.Add(cr[i]);
                //            i++;
                //            Temp.Add(cr[i]);
                //        }
                //        else if (cr[i] == '\n')
                //        {
                //            tokens.Add(Temp.Empty());
                //            break;
                //        }
                //        else 
                //        {
                //            Temp.Add(cr[i]);
                //        }
                //        i++;
                //    }
                //    if (cr[i] == '"' || cr[i] == '\\') 
                //    {
                //        Temp.Add(cr[i]);
                //    }
                //    tokens.Add(Temp.Empty());
                //}
                //else if (cr[i] == '\'')
                //{

                //}
                //else
                //{

                //}
            }
            foreach (var item in tokens)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}

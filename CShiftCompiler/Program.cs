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
            var reader = new StreamReader(Application.StartupPath + @"\input.txt");
            string testString = "\"asdasdasdasdasd\n";
            var cr = testString.ToCharArray();
            //var cr = reader.ReadToEnd().ToCharArray();

            //Reading 
            for (int i = 0; i < cr.Count(); i++)
            {
                if (cr[i] == '"')
                {
                    Temp.Add(cr[i]);
                    i++;
                    while (cr[i] != '"' )
                    {

                        i++;
                        if (cr[i] == '\n' ||i < cr.Count() - 1)
                        {
                            tokens.Add(Temp.Empty());
                            break;
                        }
                        Temp.Add(cr[i]);
                    }
                    tokens.Add(Temp.Empty());
                }
                else if (cr[i] == '\'')
                {

                }
                else
                {

                }
            }
            foreach (var item in tokens)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}

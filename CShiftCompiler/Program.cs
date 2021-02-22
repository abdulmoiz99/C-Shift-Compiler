using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CShiftCompiler
{
    static class Program
    {
        static void Main()
        {
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();

            List<Token> tokens = lexicalAnalyzer.GenerateTokens(Application.StartupPath + @"\Input\input3.txt");

            foreach (Token token in tokens)
            {
                lexicalAnalyzer.IdentifyClass(token);
            }

            lexicalAnalyzer.SaveTokens(tokens);

            lexicalAnalyzer.GenerateEndToken(tokens);

            Console.ReadKey();
        }
    }
}

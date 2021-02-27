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
            SyntaxAnalyzer syntaxAnalyzer;

            List<Token> tokens = lexicalAnalyzer.GenerateTokens(Application.StartupPath + @"\Input\test.txt");

            foreach (Token token in tokens)
            {
                lexicalAnalyzer.IdentifyClass(token);
            }

            lexicalAnalyzer.SaveTokens(tokens);

            lexicalAnalyzer.GenerateEndToken(tokens);

            Console.WriteLine();

            syntaxAnalyzer = new SyntaxAnalyzer(tokens);

            Console.ReadKey();
        }
    }
}

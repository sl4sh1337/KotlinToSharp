using System;
using System.CodeDom.Compiler;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using KotlinToSharp.Classes;
using Microsoft.CSharp;

namespace KotlinToSharp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                AntlrInputStream input = new AntlrInputStream(File.OpenRead(args[0]));
                KotlinLexer lexer = new KotlinLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                KotlinParser parser = new KotlinParser(tokens);
                parser.ErrorHandler = new ErrorStrat();

                KVisitor visitor = new KVisitor();
                //Console.WriteLine(visitor.Visit(tree));


                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                ICodeCompiler icc = codeProvider.CreateCompiler();
                CompilerParameters parameters = new CompilerParameters();
           
                parameters.GenerateExecutable = true;
                Console.WriteLine(args[0] + ".exe");
                parameters.OutputAssembly = args[0] + ".exe";
                try
                {
                    IParseTree tree = parser.kotlin();
                    string s = visitor.Visit(tree);
                    Console.WriteLine(s);
                    CompilerResults results = icc.CompileAssemblyFromSource(parameters, s);
                    if (results.Errors.HasErrors)
                    {
                        foreach (var VARIABLE in results.Errors)
                        {
                            Console.WriteLine(VARIABLE);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //Console.WriteLine(results.Errors.HasErrors);
            //Console.ReadKey();
        }
    }
}
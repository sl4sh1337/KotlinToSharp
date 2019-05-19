using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace KotlinToSharp.Classes
{
    class ErrorStrat : DefaultErrorStrategy
    {
        public override IToken RecoverInline(Parser recognizer)
        {
            throw new Exception();
        }

        public override void Recover(Parser recognizer, RecognitionException e)
        {
            throw new Exception();
        }

        public override void Sync(Parser recognizer)
        {
        }
    }
    
    internal class KVisitor : KotlinBaseVisitor<string>
    {
        Dictionary<string, string> types = new Dictionary<string, string>() {["Int"] = "int", ["Boolean"] = "bool", ["String"] = "string"};
        Dictionary<string, string> repl = new Dictionary<string, string>() {["<"] = "-1", [">"] = "1", ["=="] = "0", ["!="] = "0", ["<="] = "1", [">="] = "-1"};
        Dictionary<string, Dictionary<string, string>> classes = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> vars = new Dictionary<string, string>();
        
        
        public override string VisitKotlin(KotlinParser.KotlinContext context)
        {
            string s = "using System;";
            foreach (var VARIABLE in context.classdecl())
            {
                s += Visit(VARIABLE);
            }

            //Console.WriteLine(topclasses);
            return s + Visit(context.main());
        }

        public override string VisitMain(KotlinParser.MainContext context)
        {
            return "class Program{public static void Main(string[] args)" + Visit(context.block()) + "}";
        }

        public override string VisitBlock(KotlinParser.BlockContext context)
        {
            return "{" + Visit(context.code()) + "}";
        }

        public override string VisitCode(KotlinParser.CodeContext context)
        {
            
            string s = string.Empty;
            
            foreach (var VARIABLE in context.eq())
            {
                s += Visit(VARIABLE);
            }

            return s;
        }

        public override string VisitEq(KotlinParser.EqContext context)
        {
            return Visit(context.GetChild(0));
        }

        public override string VisitPrint(KotlinParser.PrintContext context)
        {
            return $"Console.Write({Visit(context.GetChild(1))});";
        }

        public override string VisitDecl(KotlinParser.DeclContext context)
        {
            var a = context.TYPE() is null ? Visit(context.VARNAME()[1]) : types[Visit(context.TYPE())];
            if (vars.ContainsKey(Visit(context.VARNAME()[0])))
            {
                throw new Exception($"Line {context.Start.Line}, variable '{Visit(context.VARNAME()[0])}' already exists.");
            }

            vars.Add(Visit(context.VARNAME()[0]), a);

            return $"{a} {Visit(context.VARNAME()[0])};";
        }

        public override string VisitInit(KotlinParser.InitContext context)
        {
            //Console.WriteLine(context.VARNAME() is null);
            var a = context.VARNAME() is null ? Visit(context.fieldname()) : Visit(context.VARNAME());
            if (!(context.VARNAME() is null )&&!vars.ContainsKey(Visit(context.VARNAME())))
            {
                throw new Exception($"Line {context.Start.Line}, variable '{Visit(context.VARNAME())}' doesn't exist.");
            }
            return $"{a}={Visit(context.operation())};";
        }

        public override string VisitDeclinit(KotlinParser.DeclinitContext context)
        {
            string type = String.Empty;
            //Console.WriteLine(context.Parent.RuleIndex == KotlinParser.RULE_classeq);
            switch (context.operation().GetChild<RuleContext>(0).RuleIndex)
            {
                case KotlinParser.RULE_stringoperation: type = "string"; break;
                case KotlinParser.RULE_intoperation: type = "int"; break;
                case KotlinParser.RULE_booloperation: type = "bool"; break;
                case KotlinParser.RULE_classcontruct: type = context.operation().GetChild(0).GetChild(0).GetText(); if(!classes.ContainsKey(type)) throw new Exception($"Line {context.Start.Line}, class '{type}' doesn't exist."); break;
            }
            if (context.Parent.RuleIndex == KotlinParser.RULE_classeq)
            {
                //Console.WriteLine(context.operation().GetChild<RuleContext>(0).RuleIndex);


                if (classes[Visit(context.Parent.Parent.Parent.Parent.GetChild(1))].ContainsKey(Visit(context.VARNAME())))
                {
                    throw new Exception($"Line {context.Start.Line}, field '{Visit(context.VARNAME())}' already exists.");
                }
                classes[Visit(context.Parent.Parent.Parent.Parent.GetChild(1))].Add(Visit(context.VARNAME()), type);
            }
            else
            {
                if (vars.ContainsKey(Visit(context.VARNAME())))
                {
                    throw new Exception($"Line {context.Start.Line}, variable '{Visit(context.VARNAME())}' already exists.");
                }

                vars.Add(Visit(context.VARNAME()), type);
            }
            //context.Parent.RuleIndex == KotlinParser.RULE_classeq;
            return $"{type} {Visit(context.VARNAME())}={Visit(context.operation())};";
        }

        public override string VisitTypeddeclinit(KotlinParser.TypeddeclinitContext context)
        {
            //Console.WriteLine(context.Parent.RuleIndex == KotlinParser.RULE_classeq);
            string type = String.Empty;
            var a = context.TYPE() is null ? Visit(context.VARNAME()[1]) : types[Visit(context.TYPE())];
            switch (context.operation().GetChild<RuleContext>(0).RuleIndex)
            {
                case KotlinParser.RULE_stringoperation: type = "string"; break;
                case KotlinParser.RULE_intoperation: type = "int"; break;
                case KotlinParser.RULE_booloperation: type = "bool"; break;
                case KotlinParser.RULE_classcontruct: type = context.operation().GetChild(0).GetChild(0).GetText(); if(!classes.ContainsKey(type)) throw new Exception($"Line {context.Start.Line}, class '{type}' doesn't exist."); break;
            }
            if (context.Parent.RuleIndex == KotlinParser.RULE_classeq)
            {

                if (classes[Visit(context.Parent.Parent.Parent.Parent.GetChild(1))].ContainsKey(Visit(context.VARNAME()[0])))
                {
                    throw new Exception($"Line {context.Start.Line}, field '{Visit(context.VARNAME()[0])}' already exists.");
                }

                if (type != a)
                {
                    throw new Exception($"Line {context.Start.Line}, types doesn't match.");
                }
                classes[Visit(context.Parent.Parent.Parent.Parent.GetChild(1))].Add(Visit(context.VARNAME()[0]), a);
            }
            else
            {
                if (vars.ContainsKey(Visit(context.VARNAME()[0])))
                {
                    throw new Exception($"Line {context.Start.Line}, variable '{type}' already exists.");
                }
                if (type != a)
                {
                    throw new Exception($"Line {context.Start.Line}, types doesn't match.");
                }
                vars.Add(Visit(context.VARNAME()[0]), type);
            }
            return $"{a} {Visit(context.VARNAME()[0])}={Visit(context.operation())};";
        }

        public override string VisitOperation(KotlinParser.OperationContext context)
        {
            return $"{Visit(context.GetChild(0))}";
        }

        public override string VisitStringoperation(KotlinParser.StringoperationContext context)
        {
            return $"{Visit(context.stringplus())}";
        }

        public override string VisitStringeq(KotlinParser.StringeqContext context)
        {
            if (Visit(context.GetChild(1)) == "!="||Visit(context.GetChild(1)) == "<="||Visit(context.GetChild(1)) == ">=")
            {
                return  $"(({Visit(context.GetChild(0))}).CompareTo({Visit(context.GetChild(2))})!={repl[Visit(context.GetChild(1))]})";
            }
            else return  $"(({Visit(context.GetChild(0))}).CompareTo({Visit(context.GetChild(2))})=={repl[Visit(context.GetChild(1))]})";
        }

        public override string VisitStringplus(KotlinParser.StringplusContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitStringbr(KotlinParser.StringbrContext context)
        {
            return Visit(context.LBRACKET()) + Visit(context.stringplus()) + Visit(context.RBRACKET());
        }

        public override string VisitIntoperation(KotlinParser.IntoperationContext context)
        {
            return Visit(context.GetChild(0));
        }

        public override string VisitIntasstringplus(KotlinParser.IntasstringplusContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitInteq(KotlinParser.InteqContext context)
        {
            
            if (Visit(context.GetChild(1)) == "!="||Visit(context.GetChild(1)) == "<="||Visit(context.GetChild(1)) == ">=")
            {
                return  $"(({Visit(context.GetChild(0))}).CompareTo({Visit(context.GetChild(2))})!={repl[Visit(context.GetChild(1))]})";
            }
            else return  $"(({Visit(context.GetChild(0))}).CompareTo({Visit(context.GetChild(2))})=={repl[Visit(context.GetChild(1))]})";
            //return $"{Visit(context.intadditive()[0])}{Visit(context.GetChild(1))}{Visit(context.intadditive()[1])}";
        }

        public override string VisitIntadditive(KotlinParser.IntadditiveContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitIntmult(KotlinParser.IntmultContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitIntbr(KotlinParser.IntbrContext context)
        {
            return $"{Visit(context.GetChild(0))}{Visit(context.GetChild(1))}{Visit(context.GetChild(2))}";
        }

        public override string VisitBooloperation(KotlinParser.BooloperationContext context)
        {
            return Visit(context.GetChild(0));
        }

        public override string VisitBooleq(KotlinParser.BooleqContext context)
        {
            string s = $"{Visit(context.boolor()[0])}";
            for (int i = 2; i < context.children.Count; i+=2)
            {
                if (Visit(context.GetChild(i - 1)) == "!="||Visit(context.GetChild(1)) == "<="||Visit(context.GetChild(1)) == ">=")
                {
                    s = $"(({s}).CompareTo({Visit(context.GetChild(i))})!={repl[Visit(context.GetChild(i - 1))]})";
                }
                else s = $"(({s}).CompareTo({Visit(context.GetChild(i))})=={repl[Visit(context.GetChild(i - 1))]})";
            }
            return s;
        }

        public override string VisitBoolor(KotlinParser.BoolorContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitBooland(KotlinParser.BoolandContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitBoolxor(KotlinParser.BoolxorContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitBoolneg(KotlinParser.BoolnegContext context)
        {
            return $"{Visit(context.NEGATION())}{Visit(context.GetChild(1))}";
        }

        public override string VisitBoolbr(KotlinParser.BoolbrContext context)
        {
            return $"{Visit(context.LBRACKET())}{Visit(context.GetChild(1))}{Visit(context.RBRACKET())}";
        }

        public override string VisitBoolasstringplus(KotlinParser.BoolasstringplusContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitTernar(KotlinParser.TernarContext context)
        {
            return $"{Visit(context.booloperation()[0])}?{Visit(context.GetChild(4))}:{Visit(context.GetChild(6))}";
        }

        public override string VisitDowhile(KotlinParser.DowhileContext context)
        {
            return $"{Visit(context.DO())}{Visit(context.loopblock())}{Visit(context.WHILE())}({Visit(context.booloperation())});";
        }

        public override string VisitLoopblock(KotlinParser.LoopblockContext context)
        {
            return "{" + $"{Visit(context.loopcode())}" + "}";
        }

        public override string VisitLoopcode(KotlinParser.LoopcodeContext context)
        {
            string s = string.Empty;
            
            foreach (var VARIABLE in context.children)
            {
                s += Visit(VARIABLE);
            }

            return s;
        }

        public override string VisitClassdecl(KotlinParser.ClassdeclContext context)
        {
            if (classes.ContainsKey(Visit(context.VARNAME())))
            {
                throw new Exception($"Line {context.Start.Line}, class '{Visit(context.VARNAME())}' already exists.");
            }
            classes.Add(Visit(context.VARNAME()), new Dictionary<string, string>());
            return $"class {Visit(context.VARNAME())}{Visit(context.classblock())}";
        }

        public override string VisitClassblock(KotlinParser.ClassblockContext context)
        {
            return "{" + $"{Visit(context.classcode())}" +"}";
        }

        public override string VisitClasscode(KotlinParser.ClasscodeContext context)
        {
            string s = string.Empty;
            
            foreach (var VARIABLE in context.classeq())
            {
                s += "public " + Visit(VARIABLE);
            }

            return s;
        }

        public override string VisitClasseq(KotlinParser.ClasseqContext context)
        {
            return $"{Visit(context.GetChild(0))}";
        }

        public override string VisitClasscontruct(KotlinParser.ClasscontructContext context)
        {
            return $"new {Visit(context.VARNAME())}{Visit(context.LBRACKET())}{Visit(context.RBRACKET())}";
        }

        public override string VisitFieldname(KotlinParser.FieldnameContext context)
        {
            string s = Visit(context.GetChild(0));
            for (int i = 2; i < context.children.Count; i+=2)
            {
                s += Visit(context.GetChild(i - 1)) + Visit(context.GetChild(i));
            }
            return s;
        }

        public override string VisitTerminal(ITerminalNode node)
        {
            if ((node.Symbol.Type == KotlinLexer.VARNAME)&&(node.Parent.RuleContext.RuleIndex != KotlinParser.RULE_fieldname))
            {
                var tmp = node.Parent;
                bool flag = false;
                while (tmp.RuleContext.RuleIndex != KotlinParser.RULE_classdecl&&tmp.RuleContext.RuleIndex != KotlinParser.RULE_main&&tmp.RuleContext.RuleIndex != KotlinParser.RULE_kotlin)
                {
                    if (tmp.RuleContext.RuleIndex == KotlinParser.RULE_classcontruct)
                    {
                        break;
                    }
                    if (tmp.RuleContext.RuleIndex == KotlinParser.RULE_operation)
                    {
                        flag = true;
                    }

                    tmp = tmp.Parent;
                }

                if (flag)
                {
                    if (tmp.RuleContext.RuleIndex == KotlinParser.RULE_classdecl && !(classes[Visit(tmp.RuleContext.GetChild(1))].ContainsKey(node.GetText())))
                    {
                        throw new Exception($"Line {node.Symbol.Line}, field '{node.GetText()}' doesn't exists.");
                    }
                    if (tmp.RuleContext.RuleIndex == KotlinParser.RULE_main && !(vars.ContainsKey(node.GetText())))
                    {
                        throw new Exception($"Line {node.Symbol.Line}, variable '{node.GetText()}' doesn't exists.");
                    }
                }
            }
            if ((node.Symbol.Type == KotlinLexer.NL)||(node.Symbol.Type == KotlinLexer.SEMICOLON))
            {
                return String.Empty;
            }

            if (node.Symbol.Type == KotlinLexer.BREAK)
            {
                return "break;";
            }

            if (node.Symbol.Type == KotlinLexer.XOR)
            {
                return "^";
            }
            return node.GetText();
        }

        public override string VisitErrorNode(IErrorNode node)
        {
            return base.VisitErrorNode(node);
        }
    }
}

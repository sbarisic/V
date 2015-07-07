using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Vermin {
	class VerminListener : VerminBaseListener {

		#region var
		public override void EnterOnVarNew(VerminParser.OnVarNewContext context) {
			Asm.NewVar(context.ID().GetText());
		}

		public override void EnterOnVarNewAssign(VerminParser.OnVarNewAssignContext context) {
			Asm.NewVar(context.ID().GetText());
		}

		public override void ExitOnVarNewAssign(VerminParser.OnVarNewAssignContext context) {
			Asm.Store(context.ID().GetText());
		}
		#endregion

		#region const
		public override void EnterOnString(VerminParser.OnStringContext context) {
			string S = context.STRING().ToString();
			S = S.Substring(1, S.Length - 2);
			Asm.LoadStr(S);
		}

		public override void EnterOnNumber(VerminParser.OnNumberContext context) {
			Asm.LoadNum(context.NUMBER().ToString());
		}

		public override void EnterOnNull(VerminParser.OnNullContext context) {
			Asm.LoadNull();
		}
		#endregion

		#region expr
		public override void ExitOnAssign(VerminParser.OnAssignContext context) {
				Asm.Store(context.ID().ToString());
		}

		public override void EnterOnID(VerminParser.OnIDContext context) {
				Asm.Load(context.ID().GetText());
		}

		public override void ExitOnAddSub(VerminParser.OnAddSubContext context) {
			if (context.op.Text == "+")
				Asm.Add();
			else if (context.op.Text == "-")
				Asm.Sub();
		}

		public override void ExitOnMultDiv(VerminParser.OnMultDivContext context) {
			if (context.op.Text == "*")
				Asm.Mult();
			else if (context.op.Text == "/")
				Asm.Div();
		}

		public override void ExitOnIndex(VerminParser.OnIndexContext context) {
			Asm.LoadIndex();
		}

		public override void ExitOnIndexAssign(VerminParser.OnIndexAssignContext context) {
			Asm.StoreIndex();
		}
		#endregion
	}

	static class Asm {
		public static void NewVar(string Name) {
			Console.WriteLine("newvar '{0}'", Name);
		}

		public static void Store(string Name) {
			Console.WriteLine("store '{0}'", Name);
		}

		public static void StoreIndex() {
			Console.WriteLine("storeindex");
		}

		public static void Load(string Name) {
			Console.WriteLine("load '{0}'", Name);
		}

		public static void LoadIndex() {
			Console.WriteLine("loadindex");
		}

		public static void LoadNum(string Num) {
			Console.WriteLine("loadnum {0}", Num);
		}

		public static void LoadStr(string Str) {
			Console.WriteLine("loadstr \"{0}\"", Str);
		}

		public static void LoadNull() {
			Console.WriteLine("loadnull");
		}

		public static void Add() {
			Console.WriteLine("add");
		}

		public static void Sub() {
			Console.WriteLine("sub");
		}

		public static void Mult() {
			Console.WriteLine("mult");
		}

		public static void Div() {
			Console.WriteLine("div");
		}
	}

	public static class Compiler {
		static Compiler() {

		}

		public static void Compile(string Src) {
			try {

				AntlrInputStream Input = new AntlrInputStream(Src);
				VerminLexer Lexer = new VerminLexer(Input);
				CommonTokenStream Tokens = new CommonTokenStream(Lexer);
				VerminParser Parser = new VerminParser(Tokens);

				ParseTreeWalker Walker = new ParseTreeWalker();
				VerminListener Listener = new VerminListener();
				Walker.Walk(Listener, Parser.prog());
			} catch (Exception E) {
				Console.WriteLine(E.Message);
			}
		}
	}
}
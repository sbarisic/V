using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Vermin {
	public class Runtime {
		VerminVisitor V;
		bool Dump;

		public Runtime(string Name, bool Dump = false) {
			this.Dump = Dump;
			V = new VerminVisitor(Name);
		}

		public void Compile(string Src) {
			//try {
			AntlrInputStream Input = new AntlrInputStream(Src);
			VerminLexer Lexer = new VerminLexer(Input);
			CommonTokenStream Tokens = new CommonTokenStream(Lexer);
			VerminParser Parser = new VerminParser(Tokens);
			V.Visit(Parser.prog());

			/*if (Dump)
				V.Dump();
			V.Compile();*/
			/*} catch (Exception E) {
				Console.WriteLine(E.Message);
			}//*/
		}
	}
}
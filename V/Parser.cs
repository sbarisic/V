using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Vermin {
	public enum Keyword : int {
		Int = 1,
		UInt
	}

	public class Parser {
		LexerBehavior LB;
		LexerSettings LS;

		public Parser() {
			LB = LexerBehavior.SkipComments | LexerBehavior.SkipWhiteSpaces | LexerBehavior.Default;
			LS = LexerSettings.Default;
			LS.Options = LexerOptions.StringDoubleQuote | LexerOptions.StringEscaping;

			LS.Keywords = new Dictionary<string, int>();
			string[] KeywordNames = Enum.GetNames(typeof(Keyword));
			for (int i = 0; i < KeywordNames.Length; i++)
				LS.Keywords.Add(KeywordNames[i].ToLower(), (int)(Keyword)Enum.Parse(typeof(Keyword), KeywordNames[i]));

			LS.Symbols = new Dictionary<string, int>();
			LS.Symbols.Add("(", 1);
			LS.Symbols.Add(")", 2);
			LS.Symbols.Add(",", 3);
			LS.Symbols.Add(";", 4);
		}

		public void Parse(TextReader TR) {
			Lexer L = new Lexer(TR, LB, LS);
			Token[] Tokens = L.ToArray();

			for (int i = 0; i < Tokens.Length; i++) {
				Console.WriteLine("{0} - {1} - '{2}'", Tokens[i].Id, Tokens[i].Type, Tokens[i].Value);
			}
		}

		public void Parse(string Src) {
			Parse(new StringReader(Src));
		}
	}
}
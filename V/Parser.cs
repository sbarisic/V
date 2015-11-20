using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Linq.Expressions;
using Mono.Linq.Expressions;

namespace V {
	public enum Keyword : int {
		_TypesStart, // Types begin
		Void,
		Int,
		UInt,
		String,
		Float,
		Double,
		_TypesEnd, // Types end

		_KeywordsStart, // Keywords begin
		Null,
		Return,
		_KeywordsEnd, // Keywords end
	}

	public enum Symbol : int {
		_Null,
		LParen,
		RParen,
		LCurvy,
		RCurvy,
		Comma,
		Semicolon,
		Assign,
	}

	public static class KeywordExtensions {
		public static bool IsBetween(this Keyword K, Keyword Start, Keyword End) {
			return (int)K > (int)Start && (int)K < (int)End;
		}

		public static bool IsType(this Keyword K) {
			return K.IsBetween(Keyword._TypesStart, Keyword._TypesEnd);
		}
	}

	public static class TokenExtensions {
		public static bool IsType(this Token T) {
			return T.Type == TokenType.Keyword && ((Keyword)T.Id).IsType();
		}

		public static Type ToType(this Token T) {
			if (!T.IsType())
				throw new Exception("Token is not a type token");
			Keyword K = (Keyword)T.Id;
			switch (K) {
				case Keyword.Void:
					return typeof(void);
				case Keyword.Int:
					return typeof(int);
				case Keyword.UInt:
					return typeof(uint);
				case Keyword.String:
					return typeof(string);
				case Keyword.Double:
					return typeof(double);
				case Keyword.Float:
					return typeof(float);
				default:
					throw new Exception("Unknown type keyword " + K);
			}
		}

		public static bool IsIdentifier(this Token T) {
			return T.Type == TokenType.Identifier;
		}

		public static bool IsSymbol(this Token T, Symbol S) {
			return T.Type == TokenType.Symbol && ((Symbol)T.Id) == S;
		}

		public static bool IsKeyword(this Token T, Keyword K) {
			return T.Type == TokenType.Keyword && ((Keyword)T.Id) == K;
		}
	}

	public class Parser {
		LexerBehavior LB;
		LexerSettings LS;
		Token[] Tokens;
		int Idx;

		public Parser() {
			LB = LexerBehavior.SkipComments | LexerBehavior.SkipWhiteSpaces | LexerBehavior.Default;
			LS = LexerSettings.Default;
			LS.Options = LexerOptions.StringDoubleQuote | LexerOptions.StringEscaping;

			LS.Keywords = new Dictionary<string, int>();
			string[] KeywordNames = Enum.GetNames(typeof(Keyword));
			for (int i = 0; i < KeywordNames.Length; i++) {
				if (KeywordNames[i].StartsWith("_"))
					continue;
				LS.Keywords.Add(KeywordNames[i].ToLower(), (int)(Keyword)Enum.Parse(typeof(Keyword), KeywordNames[i]));
			}

			LS.Symbols = new Dictionary<string, int>();
			LS.Symbols.Add("(", (int)Symbol.LParen);
			LS.Symbols.Add(")", (int)Symbol.RParen);
			LS.Symbols.Add("{", (int)Symbol.LCurvy);
			LS.Symbols.Add("}", (int)Symbol.RCurvy);
			LS.Symbols.Add(",", (int)Symbol.Comma);
			LS.Symbols.Add(";", (int)Symbol.Semicolon);
			LS.Symbols.Add("=", (int)Symbol.Assign);
		}

		Token Peek(int N = 0) {
			if (Idx + N >= Tokens.Length)
				return Token.Null;
			return Tokens[Idx + N];
		}

		Symbol PeekSymbol(int N = 0) {
			Token T = Peek(N);
			if (T.Type != TokenType.Symbol)
				return Symbol._Null;
			return (Symbol)T.Id;
		}

		Token Next() {
			Token N = Peek();
			Console.WriteLine(N);
			Idx++;
			return N;
		}

		public void Parse(string Src) {
			Parse(new StringReader(Src));
		}

		public void Parse(TextReader TR) {
			Lexer L = new Lexer(TR, LB, LS);
			Tokens = L.ToArray();
			Idx = 0;

			while (Peek() != Token.Null)
				Next();

			/*List<Expression> Program = new List<Expression>();
			while (Peek() != Token.Null)
				Program.Add(ParseAny());

			Console.WriteLine();
			foreach (var E in Program)
				Console.WriteLine(E.ToCSharpCode());*/
		}

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LLVMSharp;

namespace Vermin {
	public class Runtime {
		internal CodeGen Gen;

		public Runtime(string Name) {
			Gen = new CodeGen(Name);
		}

		public void SetFunction<T>(T Fn) where T : class {
			IntPtr FnPtr = Marshal.GetFunctionPointerForDelegate(Fn as Delegate);
		}

		public T GetFunction<T>(string Name) where T : class {
			return Marshal.GetDelegateForFunctionPointer(Gen.GetPointerToFunc(Name), typeof(T)) as T;
		}
	}

	public class Compiler {
		Runtime R;

		public Compiler(Runtime R) {
			this.R = R;
		}

		public void Compile(string Src, bool Dump = false) {
			//try {
			AntlrInputStream Input = new AntlrInputStream(Src);
			VerminLexer Lexer = new VerminLexer(Input);
			CommonTokenStream Tokens = new CommonTokenStream(Lexer);
			VerminParser Parser = new VerminParser(Tokens);
			ParseTreeWalker Walker = new ParseTreeWalker();

			VerminListener Listener = new VerminListener(R.Gen);
			Walker.Walk(Listener, Parser.prog());

			if (Dump)
				R.Gen.Dump();
			R.Gen.Verify();
			R.Gen.Compile();

			/*} catch (Exception E) {
				Console.WriteLine(E.Message);
			}//*/
		}
	}
}
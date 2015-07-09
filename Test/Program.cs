using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Vermin;

namespace Test {
	class Program {
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate int FncDel(int A, int B, int C);

		static Compiler C;

		static void Main(string[] args) {
			Console.Title = "Vermin Test";
			Runtime R = new Runtime("Module");

			C = new Compiler(R);
			C.Compile("func Fnc(A, B, C) { return (A + B) * C }");

			FncDel Fnc = R.GetFunction<FncDel>("Fnc");
			Console.WriteLine("Result: {0}", Fnc(2, 4, 3));

			/*while (true)
				Exec(Prompt("> "));//*/
			//Vermin.Test.Run();

			//Console.WriteLine("Done!");
			Console.ReadLine();
			Environment.Exit(0);
		}

		static string Prompt(string P) {
			string R;
			do {
				Console.Write(P);
				R = Console.ReadLine().Trim();
			} while (R.Length == 0);
			return R;
		}

		static void Exec(string Str) {
			C.Compile(Str);
		}
	}
}
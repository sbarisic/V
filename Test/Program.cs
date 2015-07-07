using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vermin;

namespace Test {
	class Program {
		static string Prompt(string P) {
			string R;
			do {
				Console.Write(P);
				R = Console.ReadLine().Trim();
			} while (R.Length == 0);
			return R;
		}

		static void Main(string[] args) {
			Console.Title = "Vermin Test";
			Console.WriteLine("Vermin v0.0.0\n");

			while (true)
				Exec(Prompt("> "));
		}

		static void Exec(string Str) {
			Compiler.Compile(Str);
		}
	}
}
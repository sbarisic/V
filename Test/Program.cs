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

		static Runtime RTime;

		static void Main(string[] args) {
			Console.Title = "Vermin Test";
			RTime = new Runtime("Module", true);

			RTime.Compile(@"
extern func Fnc() {
	//return (A + B) * C
	return
}
");

			/*FncDel Fnc = RTime.GetFunction<FncDel>("Fnc");
			Console.WriteLine("Result: {0}", Fnc(2, 4, 3));*/

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
	}
}
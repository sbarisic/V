using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using LLVMSharp;

namespace Vermin {
	public static class Test {
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int MDel(int a, int b, int c);

		public static void Run() {
			LLVMBool False = new LLVMBool(0);
			LLVMModuleRef Mod = LLVM.ModuleCreateWithName("Module");

			LLVMTypeRef[] ParamTypes = { LLVM.Int32Type(), LLVM.Int32Type(), LLVM.Int32Type() };
			LLVMTypeRef RetType = LLVM.FunctionType(LLVM.Int32Type(), out ParamTypes[0], (uint)ParamTypes.Length, False);
			LLVMValueRef Sum = LLVM.AddFunction(Mod, "somemethod", RetType);

			LLVMBasicBlockRef Entry = LLVM.AppendBasicBlock(Sum, "entry");

			LLVMBuilderRef Builder = LLVM.CreateBuilder();
			LLVM.PositionBuilderAtEnd(Builder, Entry);

			LLVMValueRef Mul = LLVM.BuildMul(Builder, LLVM.GetParam(Sum, 0), LLVM.GetParam(Sum, 1), "");
			LLVMValueRef Ad = LLVM.BuildAdd(Builder, Mul, LLVM.GetParam(Sum, 2), "");
			LLVM.BuildRet(Builder, Ad);

		

			IntPtr Error;
			LLVM.VerifyModule(Mod, LLVMVerifierFailureAction.LLVMAbortProcessAction, out Error);
			LLVM.DisposeMessage(Error);

			LLVMExecutionEngineRef Engine;

			LLVM.LinkInMCJIT();
			LLVM.InitializeX86Target();
			LLVM.InitializeX86TargetInfo();
			LLVM.InitializeX86TargetMC();
			LLVM.InitializeX86AsmPrinter();

			var Platform = Environment.OSVersion.Platform;
			if (Platform == PlatformID.Win32NT) 
				LLVM.SetTarget(Mod, Marshal.PtrToStringAnsi(LLVM.GetDefaultTargetTriple()) + "-elf");

			var Options = new LLVMMCJITCompilerOptions();
			var OptionsSize = (4 * sizeof(int)) + IntPtr.Size; // LLVMMCJITCompilerOptions has 4 ints and a pointer

			LLVM.InitializeMCJITCompilerOptions(out Options, OptionsSize);
			LLVM.CreateMCJITCompilerForModule(out Engine, Mod, out Options, OptionsSize, out Error);

			MDel SomeMethod = (MDel)Marshal.GetDelegateForFunctionPointer(LLVM.GetPointerToGlobal(Engine, Sum), typeof(MDel));
			Console.WriteLine("SomeMethod(2, 3, 4) => {0}", SomeMethod(2, 3, 4));

			LLVM.DumpModule(Mod);
			LLVM.DisposeBuilder(Builder);
			LLVM.DisposeExecutionEngine(Engine);
		}

	}
}
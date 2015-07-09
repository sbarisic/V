using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

using LLVMSharp;

namespace Vermin {
	class CodeGen {
		LLVMExecutionEngineRef Engine;
		LLVMBool True, False;
		LLVMModuleRef Mod;
		LLVMBuilderRef Builder;
		LLVMTypeRef DynamicType, ArithmType;
		IntPtr Error;

		Dictionary<string, LLVMValueRef> Funcs;
		Stack<LLVMValueRef> FuncOrders;

		Dictionary<string, LLVMValueRef> Locals;
		Stack<LLVMValueRef> LocalStack;

		public CodeGen(string Name) {
			True = new LLVMBool(1);
			False = new LLVMBool(0);
			//DynamicType = LLVM.PointerType(LLVM.VoidType(), 0);
			DynamicType = LLVM.Int32Type();
			ArithmType = LLVM.Int32Type();

			Funcs = new Dictionary<string, LLVMValueRef>();
			FuncOrders = new Stack<LLVMValueRef>();
			Locals = new Dictionary<string, LLVMValueRef>();
			LocalStack = new Stack<LLVMValueRef>();

			Mod = LLVM.ModuleCreateWithName(Name);
			Builder = LLVM.CreateBuilder();
		}

		~CodeGen() {
			LLVM.DisposeBuilder(Builder);
			LLVM.DisposeExecutionEngine(Engine);
		}

		public void Verify() {
			LLVM.VerifyModule(Mod, LLVMVerifierFailureAction.LLVMAbortProcessAction, out Error);
			LLVM.DisposeMessage(Error);
		}

		public void Dump() {
			LLVM.DumpModule(Mod);
		}

		public void BeginFunc(string Name, int Args) {
			LLVMTypeRef[] ParamTypes = null;
			if (Args > 0) {
				ParamTypes = new LLVMTypeRef[Args];
				for (int i = 0; i < Args; i++)
					ParamTypes[i] = DynamicType;
			} else
				ParamTypes = new[] { LLVM.VoidType() };

			LLVMTypeRef FType = LLVM.FunctionType(DynamicType, out ParamTypes[0], (uint)Args, False);

			LLVMValueRef F = LLVM.AddFunction(Mod, Name, FType);
			Funcs.Add(Name, F);
			FuncOrders.Push(F);

			LLVMBasicBlockRef B = LLVM.AppendBasicBlock(F, "entry");
			LLVM.PositionBuilderAtEnd(Builder, B);
		}

		int ArgCnt;

		public void BeginDeclareArg() {
			ArgCnt = 0;
		}

		public void DeclareArg(string Name) {
			LLVMValueRef V = LLVM.GetParam(FuncOrders.Peek(), (uint)ArgCnt++);
			Locals.Add(Name, V);
		}

		public void PushLocal(string Name) {
			LocalStack.Push(Locals[Name]);
		}

		LLVMValueRef PopCast(LLVMTypeRef T) {
			return LLVM.BuildBitCast(Builder, LocalStack.Pop(), T, "");
		}

		void PushCast(LLVMValueRef V) {
			LocalStack.Push(LLVM.BuildBitCast(Builder, V, DynamicType, ""));
		}

		public void Mult() {
			LLVMValueRef B = PopCast(ArithmType);
			LLVMValueRef A = PopCast(ArithmType);
			PushCast(LLVM.BuildMul(Builder, A, B, ""));
		}

		public void Div() {
			LLVMValueRef B = PopCast(ArithmType);
			LLVMValueRef A = PopCast(ArithmType);
			PushCast(LLVM.BuildFDiv(Builder, A, B, ""));
		}

		public void Add() {
			LLVMValueRef B = PopCast(ArithmType);
			LLVMValueRef A = PopCast(ArithmType);
			PushCast(LLVM.BuildAdd(Builder, A, B, ""));
		}

		public void Sub() {
			LLVMValueRef B = PopCast(ArithmType);
			LLVMValueRef A = PopCast(ArithmType);
			PushCast(LLVM.BuildSub(Builder, A, B, ""));
		}

		public void Return(bool HasVal = false) {
			if (!HasVal)
				LLVM.BuildRet(Builder, LLVM.ConstNull(DynamicType));
			else
				LLVM.BuildRet(Builder, LocalStack.Pop());
		}

		public void EndFunc() {
			//Return();
			FuncOrders.Pop();
		}

		public IntPtr GetPointerToFunc(string Name) {
			return LLVM.GetPointerToGlobal(Engine, Funcs[Name]);
		}

		public void Compile() {
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
		}
	}
}
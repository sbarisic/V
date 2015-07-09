using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using LLVMSharp;

namespace Vermin {
	class VerminListener : VerminBaseListener {
		CodeGen Gen;

		public VerminListener(CodeGen CGen) {
			Gen = CGen;
		}

		public override void EnterOnConst(VerminParser.OnConstContext context) {
			base.EnterOnConst(context);
		}

		public override void EnterOnFuncDef(VerminParser.OnFuncDefContext context) {
			var Args = context.args() as VerminParser.OnArgsDefContext;
			int ArgC = 0;
			if (Args != null)
				ArgC = Args.ID().Count;
			Gen.BeginFunc(context.ID().GetText(), ArgC);
		}

		public override void ExitOnFuncDef(VerminParser.OnFuncDefContext context) {
			Gen.EndFunc();
		}

		public override void EnterOnArgsDef(VerminParser.OnArgsDefContext context) {
			var IDs = context.ID().ToArray();
			Gen.BeginDeclareArg();
			for (int i = 0; i < IDs.Length; i++)
				Gen.DeclareArg(IDs[i].GetText());
		}

		public override void EnterOnID(VerminParser.OnIDContext context) {
			Gen.PushLocal(context.ID().GetText());
		}

		public override void EnterOnReturn(VerminParser.OnReturnContext context) {
			Gen.Return();
		}

		public override void ExitOnReturnExpr(VerminParser.OnReturnExprContext context) {
			Gen.Return(true);
		}

		public override void ExitOnMultDiv(VerminParser.OnMultDivContext context) {
			if (context.op.Text == "*")
				Gen.Mult();
			else
				Gen.Div();
		}

		public override void ExitOnAddSub(VerminParser.OnAddSubContext context) {
			if (context.op.Text == "+")
				Gen.Add();
			else
				Gen.Sub();
		}
	}
}
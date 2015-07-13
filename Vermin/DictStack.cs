using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vermin {
	class DictStack<TKey, TValue> {
		Stack<Dict<TKey, TValue>> DStack;

		public TValue this[TKey Key] {
			get {
				Dict<TKey, TValue> Dct = DStack.Peek();
				if (!Dct.ContainsKey(Key)) {
					Dict<TKey, TValue>[] Dicts = DStack.ToArray();
					for (int i = 1; i < Dicts.Length; i++)
						if (Dicts[i].ContainsKey(Key))
							return Dicts[i][Key];
				}
				return DStack.Peek()[Key];
			}
			set {
				DStack.Peek()[Key] = value;
			}
		}

		public DictStack() {
			DStack = new Stack<Dict<TKey, TValue>>();
			PushNew();
		}

		public void PushNew() {
			DStack.Push(new Dict<TKey, TValue>());
		}

		public Dict<TKey, TValue> Pop() {
			return DStack.Pop();
		}
	}
}

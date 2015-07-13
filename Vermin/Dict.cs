using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vermin {
	class Dict<TKey, TVal> {
		Dictionary<TKey, TVal> Dct;

		public Dict() {
			Dct = new Dictionary<TKey, TVal>();
		}

		public TVal this[TKey Key] {
			get {
				if (ContainsKey(Key))
					return Dct[Key];
				return default(TVal);
			}
			set {
				if (value == null) {
					Remove(Key);
					return;
				}

				if (ContainsKey(Key))
					Remove(Key);
				Dct.Add(Key, value);
			}
		}

		public bool ContainsKey(TKey Key) {
			return Dct.ContainsKey(Key);
		}

		public void Remove(TKey Key) {
			Dct.Remove(Key);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WOA3.Model;
using WOA3.Model.Skills;

namespace WOA3.Logic {
	public class Unsubscriber : IDisposable {

		private VisiblityChangeMobCallback callback;

		public Unsubscriber(VisiblityChangeMobCallback callback) {
			this.callback = callback;
		}

		public void Dispose() {
			this.callback.Invoke();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WOA3.Logic {
	interface Scareable {

		ScaredFactor Scared { get; set; }

		void scare(float amount);
	}
}

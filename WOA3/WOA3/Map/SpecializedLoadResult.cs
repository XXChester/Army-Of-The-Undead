using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WOA3.Map {
	public struct SpecializedLoadResult {
		#region Properties
		public Point Start { get; set; }
		public Point End { get; set; }
		public MonsterType TypeOfMob { get; set; }
		#endregion Properties
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WOA3.Model;
using WOA3.Logic.Skills;

namespace WOA3.Logic {
	public struct CombatRequest {
		public Character Source { get; set; }
		public List<Character> Targets { get; set; }
		public Skill Skill { get; set; }
	}
}

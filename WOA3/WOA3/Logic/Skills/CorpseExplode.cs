using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WOA3.Logic.Skills {
	public class CorpseExplode : Skill {
		private const int COOL_DOWN = 1;
		public CorpseExplode(float damage) : base(damage, 1, null) { }
	}
}

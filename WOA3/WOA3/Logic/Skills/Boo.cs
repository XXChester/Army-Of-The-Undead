using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WOA3.Logic.Skills {
	public class Boo : Skill {

		private const float DAMAGE = 1f;
		private const int COOL_DOWN = 30;
		public Boo() : base(DAMAGE, COOL_DOWN) { }
	}
}

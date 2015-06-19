using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;

namespace WOA3.Model.Skills {
	public class Appear : Skill {

		private const float DAMAGE = 0f;
		private const int COOL_DOWN = 1;
		public Appear(VisualCallback visualCallback) : base(DAMAGE, COOL_DOWN, visualCallback) {	}
	}
}

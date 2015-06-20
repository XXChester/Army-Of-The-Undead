using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace WOA3.Logic.Skills {
	public class Disappear : Skill {
		

		private const float DAMAGE = 0f;
		private const int COOL_DOWN = 2000;
		public Disappear( SoundEffect sfx, SkillFinished skillCallback) : base(DAMAGE, COOL_DOWN, sfx, skillCallback) { }
	}
}

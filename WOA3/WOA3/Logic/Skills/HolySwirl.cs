using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace WOA3.Logic.Skills {
	public class HolySwirl : Skill {
		public HolySwirl(SoundEffect sfx, float damage, SkillFinished skillFinished, int cooldown) : base(damage, cooldown, sfx, skillFinished) { }
	}
}

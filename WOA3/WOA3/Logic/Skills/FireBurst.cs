﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace WOA3.Logic.Skills {
	public class FirstBurst : Skill {
		private const int COOL_DOWN = 1600;
		public FirstBurst(SoundEffect sfx, float damage, SkillFinished skillFinished) : base(damage, COOL_DOWN, sfx, skillFinished) { }
	}
}

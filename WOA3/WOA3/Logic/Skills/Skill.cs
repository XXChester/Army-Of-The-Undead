using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WOA3.Logic.Skills {
	public abstract class Skill {

		private float lastUsedAt;

		private readonly float DAMAGE;
		private readonly int COOL_DOWN;


		public Skill(float damage, int coolDown) {
			DAMAGE = damage;
			COOL_DOWN = coolDown * 1000;
			this.lastUsedAt = 0f;
		}

		public SkillResult perform(BoundingSphere boundingSphere) {
			SkillResult result = null;
			if (lastUsedAt == 0) {
				// positive angle is the right side, negative is the left side
				float directionFactor = 1f;

				this.lastUsedAt = COOL_DOWN;
				result = new SkillResult() { Damage = DAMAGE * directionFactor, BoundingSphere = boundingSphere  };
			}
			return result;
		}

		public void update(float elapsed) {
			if (lastUsedAt != 0) {
				lastUsedAt -= elapsed;
				if (lastUsedAt <= 0) {
					lastUsedAt = 0;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;

namespace WOA3.Model.Skills {
	public abstract class Skill {
		private VisualCallback visualCallback;
		private float lastUsedAt;

		private readonly float DAMAGE;
		private readonly int COOL_DOWN;

		public bool CoolDownOver { get { return lastUsedAt == 0; } }

		public Skill(float damage, int coolDown) : this(damage, coolDown, null) { }

		public Skill(float damage, int coolDown, VisualCallback visualCallback) {
			DAMAGE = damage;
			COOL_DOWN = coolDown * 1000;
			this.lastUsedAt = 0f;
			this.visualCallback = visualCallback;
		}

		public virtual SkillResult perform(BoundingSphere boundingSphere) {
			SkillResult result = null;
			if (CoolDownOver) {
				// positive angle is the right side, negative is the left side
				float directionFactor = 1f;

				this.lastUsedAt = COOL_DOWN;
				result = new SkillResult() { Damage = DAMAGE * directionFactor, BoundingSphere = boundingSphere  };
				if (this.visualCallback != null) {
					this.visualCallback.Invoke();
				}
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace WOA3.Logic.Skills {
	public abstract class Skill {
		private SkillFinished skillCallback;
		private float lastUsedAt;
		private SoundEffect sfx;

		private readonly float DAMAGE;
		private readonly int COOL_DOWN;

		public bool CoolDownOver { get { return lastUsedAt == 0; } }

		public Skill(float damage, int coolDown, SoundEffect sfx, SkillFinished skillCallback) {
			DAMAGE = damage;
			//DAMAGE = 0f;
			COOL_DOWN = 2500;
			//COOL_DOWN = coolDown * 1000;
			this.lastUsedAt = 0f;
			this.skillCallback = skillCallback;
			this.sfx = sfx;
		}

		public virtual SkillResult perform(BoundingSphere boundingSphere) {
			SkillResult result = null;
			if (CoolDownOver) {
				// positive angle is the right side, negative is the left side
				float directionFactor = 1f;

				this.lastUsedAt = COOL_DOWN;
				result = new SkillResult() { Damage = DAMAGE * directionFactor, BoundingSphere = boundingSphere  };
				if (this.skillCallback != null) {
					this.skillCallback.Invoke();
				}
				if (this.sfx != null) {
					SoundManager.getInstance().sfxEngine.playSoundEffect(this.sfx);
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

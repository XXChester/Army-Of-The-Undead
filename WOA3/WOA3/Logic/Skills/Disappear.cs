using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WOA3.Logic.Skills {
	public class Disappear : Skill {
		
		private VisualCallback visualCallback;

		private const float DAMAGE = 0f;
		private const int COOL_DOWN = 1;

		public Disappear(VisualCallback visualCallback) : base(DAMAGE, COOL_DOWN) {
			this.visualCallback = visualCallback;
		}

		public override SkillResult perform(BoundingSphere boundingSphere) {
			SkillResult result = base.perform(boundingSphere);
			if (result != null) {
				Debug.log("GOOoooOOOooOD Bye!!");
				if (this.visualCallback != null) {
					this.visualCallback.Invoke();
				}
			}
			return result;
		}
	}
}

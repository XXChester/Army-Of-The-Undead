using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace WOA3.Logic.Behaviours {
	public class Tracking : TargetBehaviour {

		private readonly float SPEED;

		public Vector2 Target { get; set; }

		public Vector2 Position { get; set; }

		public Tracking(Vector2 startingPosition, float speed) {
			this.Position = startingPosition;
			this.Target = this.Position;
			this.SPEED = speed;
		}


		public void update(float elapsed) {
			if (!Target.Equals(Position)) {
				float distance = Vector2.Distance(Position, Target);
				Vector2 direction = Vector2.Normalize(Target - Position);

				Vector2 newPosition = Position + direction * SPEED * elapsed;
				if (Vector2.Distance(Position, newPosition) >= distance) {
					Position = Target;
				} else {
					Position = newPosition;
				}
			}
		}
	}
}

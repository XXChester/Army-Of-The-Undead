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
		private Vector2 target;
		private bool recalculate;
		private Vector2 originalDistance;

		private readonly float SPEED;

		public Vector2 Target {
			get { return target; }
			set {
				this.target = value;
				this.recalculate = true;
				this.originalDistance = Vector2.Subtract(target, Position);
			}
		}

		public Vector2 Position { get; set; }

		public Tracking(Vector2 startingPosition, float speed) {
			this.Position = startingPosition;
			this.target = this.Position;
			this.SPEED = speed;
		}


		public void update(float elapsed) {
			if (this.recalculate) {
				String test = "";
				this.recalculate = false;
			}
			if (!Target.Equals(Position)) {
				float distance = Vector2.Distance(Position, target);
				Vector2 direction = Vector2.Normalize(target - Position);

				Vector2 newPosition = Position + direction * SPEED * elapsed;
				if (Vector2.Distance(Position, newPosition) >= distance) {
					Position = target;
				} else {
					Position = newPosition;
				}
			}
		}
	}
}

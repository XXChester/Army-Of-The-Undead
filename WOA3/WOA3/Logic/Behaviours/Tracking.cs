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
				Vector2 distance = Vector2.Subtract(target, Position);
				float maxSpeedForFrame = (SPEED / 1000) * elapsed;
				//float delta = (this.speed / 1000) * elapsed;
				//NEED TO STOP LERPING!!
				Position = Vector2.Add(Position, distance * maxSpeedForFrame);


				//Vector2 speed = new Vector2(SPEED);
				//Position = Vector2.Add(Position, Vector2.MXA(distance * maxSpeedForFrame, speed * maxSpeedForFrame));

				//Vector2 noLerp = new Vector2(Math.Max(delta, delta *distance.X), Math.Max(delta, delta *distance.Y));
				//Vector2 t = maxSpeedForFrame * originalDistance;
				//Vector2 t2 = maxSpeedForFrame * distance;
				
				
				//Position = Vector2.Add(Position, noLerp);
				//Position = Vector2.Add(Position, originalDistance * maxSpeedForFrame);
				//Vector2 noLerp = new Vector2(Math.Max(maxSpeedForFrame, originalDistance.X * maxSpeedForFrame), 
				//	(Math.Max(maxSpeedForFrame, originalDistance.Y * maxSpeedForFrame)));
				
				/*Vector2 min = Vector2.Min(originalDistance * maxSpeedForFrame, new Vector2(maxSpeedForFrame * SPEED));
				Vector2 max = Vector2.Max(originalDistance * maxSpeedForFrame, new Vector2(maxSpeedForFrame * SPEED));
				Vector2 noLerp = Vector2.Clamp(new Vector2(maxSpeedForFrame), min, max);
				Position = Vector2.Add(Position, noLerp);*/
			}
		}
	}
}

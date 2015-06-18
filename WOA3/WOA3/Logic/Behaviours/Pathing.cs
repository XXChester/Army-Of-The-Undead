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

using WOA3.Engine;


namespace WOA3.Logic.Behaviours {
	public class Pathing : TargetBehaviour {
		private Stack<Vector2> targets;
		

		private readonly float SPEED;


		public Stack<Vector2> Targets {
			get { return targets; }
			set {
				this.targets = value;
				this.Target = targets.Pop();
			}
		}

		public Vector2 Target { get; set; }

		public Vector2 Position { get; set; }

		public Pathing(Vector2 startingPosition, float speed, Stack<Point> points) {
			this.Position = startingPosition;
			this.Target = this.Position;
			this.SPEED = speed;
			Queue<Vector2> targets = new Queue<Vector2>();
			foreach (var point in points) {
				targets.Enqueue(point.toVector2());
			}
			this.targets = new Stack<Vector2>();
			foreach (var vector in targets) {
				Targets.Push(vector);
			}
		}


		public void update(float elapsed) {
			Vector2 direction = Vector2.Subtract(Target, Position);
			// direction is also the distance
			float minimum = 1f;
			if (direction.X < minimum && direction.Y < minimum) {
				if (targets.Count > 0) {
					Target = targets.Pop();
				}
				direction = Vector2.Subtract(Target, Position);
			}

			float maxSpeedForFrame = (SPEED / 1000) * elapsed;		
			Position = Vector2.Add(Position, direction * maxSpeedForFrame);
		}
	}
}

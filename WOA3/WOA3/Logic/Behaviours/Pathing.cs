﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using GWNorthEngine.Input;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Engine;
using WOA3.Logic.AI;


namespace WOA3.Logic.Behaviours {
	public class Pathing : TargetBehaviour {
		private Stack<Vector2> targets;
		private BehaviourFinished callback;

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

		public Pathing(Vector2 startingPosition, float speed, BehaviourFinished callback) {
			this.SPEED = speed;
			this.targets = new Stack<Vector2>();
			this.Position = startingPosition;
			this.callback = callback;
		}
		public Pathing(Vector2 startingPosition, float speed, Stack<Point> points) {
			this.targets = new Stack<Vector2>();
			this.Position = startingPosition;
			this.SPEED = speed;
			initTargets(points);
		}

		public void init(Vector2 startingPosition) {
			init(startingPosition, InputManager.getInstance().MousePosition);
		}

		public void init(Vector2 startingPosition, Vector2 endPosition) {
			this.Position = startingPosition;
			AIManager.getInstance().requestPath(startingPosition.toPoint(), endPosition.toPoint(), delegate(Stack<Point> path) {
				if (this != null) {
					if (path != null) {
						initTargets(path);
					}
				}
			});
		}

		private void initTargets(Stack<Point> points) {
			if (points.Count > 0) {
				this.Targets.Clear();
				Point myPosition = Position.toPoint();
				List<Point> reversedPoints = points.ToList<Point>();
				reversedPoints.Reverse();
				foreach (var point in reversedPoints) {
					if (!point.Equals(myPosition)) {
						Vector2 newPosition = point.toVector2();
						newPosition = Vector2.Subtract(newPosition, new Vector2(0f, Constants.TILE_SIZE / 2));
						Targets.Push(newPosition);
					}
				}
				this.Target = Targets.Pop();
			}
		}

		public void update(float elapsed) {
			/*if (Targets.Count == 0) {
				init(Position);
			}*/
			if (Targets.Count > 0) {
				float distance = Vector2.Distance(Position, Target);
				Vector2 direction = Vector2.Normalize(Target - Position);

				Vector2 newPosition = Position + direction * SPEED * elapsed;
				//float distanceBetweenPositions = Vector2.Distance(Target, Position);
				float distanceBetweenPositions = Vector2.Distance(Target, newPosition);
				if (distanceBetweenPositions >= distance) {
					newPosition = Target;
					Target = targets.Pop();
				}
				if (!float.IsNaN(newPosition.X) && !float.IsNaN(newPosition.Y)) {
					Position = newPosition;
				} else {
					Debug.log("NAN: " + Position + "\tCount: " + Targets.Count);
					Target = Targets.Pop();
				}
			}
			if (Targets.Count == 0) {
				if (this.callback != null) {
					this.callback.Invoke();
				}
			}
		}
	}
}

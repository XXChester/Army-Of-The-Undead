using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;

using WOA3.Engine;
using WOA3.Logic;
using WOA3.Logic.AI;

namespace WOA3.Model {
	public class Map {
		#region Class variables
	//	private readonly int[,] COLLISION_CHECKS;
		#endregion Class variables

		#region Class properties
		public List<Wall> Walls { get; set; }
		#endregion Class properties

		#region Constructor
		public Map(List<Wall> walls) {
			this.Walls = walls;
			/*this.COLLISION_CHECKS = new int[25, 2];
			int min = -1;
			int count = 0;
			for (int y = min; y <= -min; y++) {
				for (int x = min; x <= -min; x++) {
					this.COLLISION_CHECKS[count, 0] = y;
					this.COLLISION_CHECKS[count, 1] = x;
					count++;
				}
			}*/
		}
		#endregion Constructor

		#region Support methods
		public Wall collisionDetected(BoundingBox bbox) {
			Wall collisionWith = null;
			foreach (var wall in Walls) {
				if (bbox.Intersects(wall.BBox)) {
					collisionWith = wall;
					break;
				}
			}


			return collisionWith;
		}

		public void render(SpriteBatch spriteBatch) {
			foreach (var wall in Walls) {
				wall.render(spriteBatch);
			}
#if DEBUG
			if (Debug.showAIMap) {
				GWNorthEngine.AI.AStar.BasePathFinder.TypeOfSpace space;
				for (int y = 0; y <=  AIManager.getInstance().Board.GetUpperBound(0); y++) {
					for (int x = 0; x <=  AIManager.getInstance().Board.GetUpperBound(1); x++) {
						space = AIManager.getInstance().Board[y, x];
						if (space == GWNorthEngine.AI.AStar.BasePathFinder.TypeOfSpace.Unwalkable) {
							DebugUtils.drawRectangle(spriteBatch, new Point(x, y).toRectangle(), Debug.DEBUG_BBOX_Color, Debug.debugChip);
						} else {
							DebugUtils.drawRectangle(spriteBatch, new Point(x, y).toRectangle(), Debug.DEBUG_AI_MAP_WALKABLE, Debug.debugChip);
						}
					}
				}
			}
#endif
			/*if (this.Tiles != null) {
				foreach (var tile in Tiles) {
					if (tile != null) {
						tile.render(spriteBatch);
#if DEBUG
						if (Debug.debugOn) {
							DebugUtils.drawBoundingBox(spriteBatch, tile.BBox, Debug.DEBUG_BBOX_Color, Debug.debugChip);
						}
#endif
					}
				}
			}
			*/
		}
		#endregion Support methods
	}
}

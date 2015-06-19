using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WOA3.Engine {
	public static class Extensions {
		//private static Vector2 OFFSET = new Vector2(Constants.TILE_SIZE / 2f);
		private static Vector2 OFFSET = new Vector2(0f);
		public static Vector2 toVector2(this Point point) {
			return Vector2.Add(OFFSET, Constants.TILE_SIZE * new Vector2(point.X, point.Y));
		}

		public static Point toPoint(this Vector2 vector) {
			float xSize = vector.X -OFFSET.X;
			float ySize = vector.Y -OFFSET.Y;
			int x = (int)(xSize / Constants.TILE_SIZE);
			int y = (int)(ySize / Constants.TILE_SIZE);
			return new Point(x,y);
		}
		public static Rectangle toRectangle(this Point point) {
			Vector2 v = point.toVector2();
			return new Rectangle((int)v.X, (int)v.Y, (int)Constants.TILE_SIZE, (int)Constants.TILE_SIZE);
		}
	}
}

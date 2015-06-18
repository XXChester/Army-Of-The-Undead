using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WOA3.Engine {
	public static class Extensions {
		public static Vector2 toVector2(this Point point) {
			return Constants.TILE_SIZE * new Vector2(point.X, point.Y);
		}

		public static Point toPoint(this Vector2 vector) {
			int x = (int)(vector.X / Constants.TILE_SIZE);
			int y = (int)(vector.Y / Constants.TILE_SIZE);
			return new Point(x,y);
		}
		public static Rectangle toRectangle(this Point point) {
			Vector2 v = point.toVector2();
			return new Rectangle((int)v.X, (int)v.Y, (int)Constants.TILE_SIZE, (int)Constants.TILE_SIZE);
		}
	}
}

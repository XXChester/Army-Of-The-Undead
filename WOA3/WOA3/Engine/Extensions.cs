using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WOA3.Engine {
	public static class Extensions {
		//private static Vector2 OFFSET = new Vector2(Constants.TILE_SIZE / 2f);
		private static Vector2 OFFSET = new Vector2(Constants.TILE_SIZE / 3f * 2);
		//private static Vector2 OFFSET = new Vector2(Constants.TILE_SIZE);
		//private static Vector2 OFFSET = new Vector2(0f);
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
			//return new Rectangle((int)v.X, (int)v.Y, (int)Constants.TILE_SIZE, (int)Constants.TILE_SIZE);

			Vector2 start = Vector2.Subtract(v, OFFSET);
			Vector2 end = Vector2.Add(v, new Vector2(Constants.TILE_SIZE));

			/*float half = Constants.TILE_SIZE / 2f;
			Vector2 start = Vector2.Subtract(v, new Vector2(half));
			Vector2 end = Vector2.Add(v, new Vector2(half));*/
			return new Rectangle((int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
		}

		public static Vector3 toVector3(this Vector2 vector) {
			return new Vector3(vector, 0f);
		}

		public static Vector2 toVector2(this Vector3 vector) {
			return new Vector2(vector.X, vector.Y);
		}
	}
}

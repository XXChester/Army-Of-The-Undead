using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WOA3.Logic {
	public class PositionUtils {
		public static BoundingBox getBBox(Vector2 position) {
			float half = Constants.TILE_SIZE / 2f;
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(half)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(half)), 0f));
		}

		public static BoundingBox getBBox32(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE)), 0f));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WOA3.Logic {
	public class CollisionGenerationUtils {



		public static BoundingBox generateBoundingBoxesForTexture(Texture2D texture, Vector2 position) {
			return getBBox(position);
			/*
			BoundingBox bbox = getBBox(position);
			float halfTile = Constants.TILE_SIZE / 2;
			float quarterTile = Constants.TILE_SIZE / 4;
			Vector2 newPosition = Vector2.Subtract(position, new Vector2(halfTile));
			if (texture.Name.StartsWith("2c")) {
				bbox = getBBox(position);
			} else if (texture.Name.StartsWith("ud")) {
				bbox = new BoundingBox(
						new Vector3(Vector2.Add(newPosition, new Vector2(halfTile, 0f)), 0f),
						new Vector3(Vector2.Add(newPosition, new Vector2(quarterTile, Constants.TILE_SIZE)), 0f));
			} else if (texture.Name.StartsWith("lr")) {
				bbox = new BoundingBox(
						new Vector3(Vector2.Add(newPosition, new Vector2(0f, halfTile)), 0f),
						new Vector3(Vector2.Add(newPosition, new Vector2(Constants.TILE_SIZE, quarterTile)), 0f));
			} else if (texture.Name.StartsWith("c")) {
				bbox = getBBox(position);
			} else {
				throw new Exception("Cannot load BBox for: " + texture.Name);
			}
			return bbox;*/
		}

		public static BoundingBox getBBox(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE / 2)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE / 2)), 0f));
		}

		public static BoundingBox getBBoxHalf(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE / 4)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE / 4)), 0f));
		}

		public static BoundingBox getBBox64(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE )), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE)), 0f));
		}

		public static BoundingBox getCharacterBBox(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Add(position, new Vector2(-(Constants.TILE_SIZE /3), Constants.TILE_SIZE / 4)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE / 3, Constants.TILE_SIZE / 2)), 0f));
		}

		public static Rectangle getButtonRectangle(Vector2 origin, Vector2 position) {
			float originY = origin.Y / 2 + 4f;
			return new Rectangle((int)(position.X - origin.X), (int)(position.Y - originY), 256, 70);
		}
	}
}
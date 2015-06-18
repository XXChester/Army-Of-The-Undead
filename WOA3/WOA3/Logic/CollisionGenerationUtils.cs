using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WOA3.Logic  {
	public class CollisionGenerationUtils {
		
		public const string NAME_SIMPLE = "Simple";
		public const string NAME_QUAD = "Quad";
		public const string NAME_DOUBLE_RIGHT = "DoubleRight";
		public const string NAME_DOUBLE_TOP = "DoubleTop";
		public const string NAME_DOUBLE_LEFT = "DoubleLeft";
		public const string NAME_DOUBLE_BOTTOM = "DoubleBottom";
		
		public static BoundingBox generateBoundingBoxesForTexture(Texture2D texture, Vector2 position) {
			BoundingBox bbox = new BoundingBox();
			float halfTile = Constants.TILE_SIZE / 2;
			switch (texture.Name) {
				case NAME_SIMPLE:
				case NAME_QUAD:
					bbox = getBBox(position);
					//bbox = new BoundingBox(new Vector3(position.X, position.Y, 0f),
				//		new Vector3(position.X + Constants.TILE_SIZE , position.Y + Constants.TILE_SIZE , 0f));
					break;
				case NAME_DOUBLE_BOTTOM:
					bbox = new BoundingBox(new Vector3(position.X, position.Y + halfTile, 0f),
						new Vector3(position.X + Constants.TILE_SIZE , position.Y + Constants.TILE_SIZE , 0f));
					break;
				case NAME_DOUBLE_LEFT:
					bbox = new BoundingBox(new Vector3(position.X, position.Y, 0f),
						new Vector3(position.X + halfTile, position.Y + Constants.TILE_SIZE , 0f));
					break;
				case NAME_DOUBLE_RIGHT:
					bbox = new BoundingBox(new Vector3(position.X + halfTile, position.Y, 0f),
						new Vector3(position.X + Constants.TILE_SIZE , position.Y + Constants.TILE_SIZE , 0f));
					break;
				case NAME_DOUBLE_TOP:
					bbox = new BoundingBox(new Vector3(position.X, position.Y, 0f),
						new Vector3(position.X + Constants.TILE_SIZE , position.Y + halfTile, 0f));
					break;
			}
			return bbox;
		}

		public static BoundingBox getBBox(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE/2)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE/2)), 0f));
		}

		public static BoundingBox getBBoxHalf(Vector2 position) {
			return new BoundingBox(
				new Vector3(Vector2.Subtract(position, new Vector2(Constants.TILE_SIZE /4)), 0f),
				new Vector3(Vector2.Add(position, new Vector2(Constants.TILE_SIZE /4)), 0f));
		}

	
	}
}

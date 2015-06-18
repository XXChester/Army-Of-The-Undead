using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace WOA3.Map {
	/*public delegate bool MapCollisionChecker(BoundingBox bbox, Vector2 objectsPosition);
	public delegate bool LauncherCollisionChecker(BoundingBox bbox, Vector2 objectsPositions);
	public delegate void FishCounterIncrementer(int amount);
	public delegate void PlayerHealthModifier(float damage, HitType hitType);
	public delegate void ScoreIncrementer(int amount);
	public delegate void DisplayUpdater(Vector2 moveBy);*/
#if DEBUG
	public delegate void EditorCreator(MapEditor.MappingState type, Vector2 position);
	public delegate void EditorDeleter(Point point);
#endif
}

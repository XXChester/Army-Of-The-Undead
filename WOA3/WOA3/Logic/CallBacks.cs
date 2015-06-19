using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using WOA3.Model;

namespace WOA3.Logic {
	public delegate void VisualCallback();
	public delegate void DisappearCallback();
	public delegate void VisiblityChangeMobCallback();
	public delegate void BehaviourFinished();
	public delegate void SkillFinished();
	public delegate void OnDeath(Character character);
	public delegate List<Character> CharactersInRange(BoundingSphere range);
	public delegate bool CollisionCheck(Vector2 newPosition);
}

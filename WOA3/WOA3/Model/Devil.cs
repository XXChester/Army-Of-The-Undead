using System;
using System.Collections.Generic;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.AI.AStar.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using WOA3.Engine;
using WOA3.Logic;
using WOA3.Model.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;

namespace WOA3.Model {
	public class Devil : Mob {
		#region Class variables
	
		#endregion Class variables

		#region Class propeties
		
		#endregion Class properties

		#region Constructor
		public Devil(ContentManager content, Vector2 position, CharactersInRange charactersInRange, OnDeath onDeath, CollisionCheck collisionCheck)
			:base(content, position, charactersInRange, onDeath, collisionCheck, "Monster1") {
		}
		#endregion Constructor

		#region Support methods
		protected override void initSkills() {
			this.skills.Add(new HolySwirl(2f));
		}
		#endregion Support methods
	}
}

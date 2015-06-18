using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace WOA3.Logic.Behaviours {
	public class LostTarget : Tracking {

		public LostTarget(Vector2 startingPosition, Vector2 lastKnownPosition, float speed, BehaviourFinished callback)
		:base(startingPosition, speed, callback){
			this.Target = lastKnownPosition;
		}
	}
}

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
	public class IdleBehaviour : TargetBehaviour{

		public Vector2 Target { get; set; }

		public Vector2 Position { get; set; }


		public IdleBehaviour(Vector2 startingPosition) {
			this.Position = startingPosition;
			this.Target = this.Position;
		}

		public void update(float elapsed) {
			
		}
	}
}

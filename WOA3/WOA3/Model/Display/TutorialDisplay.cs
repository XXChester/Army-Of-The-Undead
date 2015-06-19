using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.StateMachine;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model.Scenarios;

namespace WOA3.Model.Display {
	public class TutorialDisplay : GameDisplay {
		#region Class variables
		private TutorialScenario activeScenario;
		private Queue<TutorialScenario> activeScenarios;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public TutorialDisplay(GraphicsDevice graphics, ContentManager content, GameStateMachine gameStateMachine) :base(graphics, content, "Tutorial") {
			Constants.ALLOW_MOB_ATTACKS = false;
			Constants.ALLOW_PLAYER_ATTACKS = false;
			Ghost ghost = this.allGhosts[0];
			Mob mob = this.mobs[0];
			mob.Inactive = true;

			this.activeScenarios = new Queue<TutorialScenario>();
			this.activeScenarios.Enqueue(new TutorialScenario(content, "Unitselect", ghost, mob));
			this.activeScenarios.Enqueue(new MovementTutorial(content, "Movement", ghost, mob));
			this.activeScenarios.Enqueue(new EnemySpawnTutorial(content, "EnemySpawn", ghost, mob));
			this.activeScenarios.Enqueue(new EvadeTutorial(content, "Evade", ghost, mob));
			this.activeScenarios.Enqueue(new KillingTutorial(content, "Killing", ghost, mob, this.allGhosts));
			this.activeScenarios.Enqueue(new ArmyTutorial(content, "Army", ghost, mob, allGhosts, gameStateMachine));

			this.activeScenario = this.activeScenarios.Dequeue();
			
		}
		#endregion Constructor

		#region Support methods

		public override void update(float elapsed) {
			base.update(elapsed);

			if (this.activeScenario != null) {
				this.activeScenario.update(elapsed);
				if (this.activeScenario.Completed) {
					if (this.activeScenarios.Count > 0) {
						this.activeScenario = this.activeScenarios.Dequeue();
					} else {
						//DO SOMETHING!!
					}
				}
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			if (this.activeScenario != null) {
				this.activeScenario.render(spriteBatch);
			}
		}
		#endregion Support methods
	}
}

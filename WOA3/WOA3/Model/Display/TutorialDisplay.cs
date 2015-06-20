using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Logic;
using WOA3.Logic.Skills;
using WOA3.Logic.AI;
using WOA3.Logic.GameStateMachine;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model.Scenarios;

namespace WOA3.Model.Display {
	public class TutorialDisplay : GameDisplay {
		#region Class variables
		private BaseTutorialScenario activeScenario;
		private Queue<BaseTutorialScenario> activeScenarios;
		private int scenario;
		private List<Button> buttons;

		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public TutorialDisplay(GraphicsDevice graphics, ContentManager content) :base(graphics, content, "Tutorial") {
			GameStateMachine.getInstance().LevelContext = null;
			float xBuffer = 256;
			float yBuffer = 128;
			float leftSideX = Constants.RESOLUTION_X  - xBuffer;
			float y = Constants.RESOLUTION_Y - yBuffer;


			VisualCallback setPrevipousState = delegate() {
				GameStateMachine.getInstance().goToPreviousState();
			};
			VisualCallback resetState = delegate() {
				init(this.scenario);
			};

			string[] buttonNames = { "Menu", "Reset" };
			TexturedEffectButton menu = ModelGenerationUtil.createButton(content, new Vector2(xBuffer, y), buttonNames[0]);
			TexturedEffectButton restart = ModelGenerationUtil.createButton(content, new Vector2(leftSideX, y), buttonNames[1]);

			this.buttons = new List<Button>();
			this.buttons.Add(new Button(content, menu, setPrevipousState));
			this.buttons.Add(new Button(content, restart, resetState));

			init(0);
		}
		#endregion Constructor

		#region Support methods
		
		private void init(int active) {
			base.init(true);
			Ghost ghost = this.allGhosts[0];
			Mob mob = this.mobs[0];
			mob.Inactive = true;

			this.activeScenarios = new Queue<BaseTutorialScenario>();
			this.activeScenarios.Enqueue(new SelectionTutorial(content, "Unitselect", ghost, mob));
			this.activeScenarios.Enqueue(new MovementTutorial(content, "Movement", ghost, mob));
			this.activeScenarios.Enqueue(new EnemySpawnTutorial(content, "Invisible", ghost, mob));
			this.activeScenarios.Enqueue(new EvadeTutorial(content, "Evade", ghost, mob));
			this.activeScenarios.Enqueue(new KillingTutorial(content, "Killing", ghost, mob, this.allGhosts));
			this.activeScenarios.Enqueue(new ArmyTutorial(content, "Army", ghost, mob, allGhosts, GameStateMachine.getInstance()));
			for (int i = 0; i <= active; i++) {
				// the last scenario requires 2 ghosts
				if (0 == this.activeScenarios.Count - 1) {
					break;
				}
				this.activeScenario = this.activeScenarios.Dequeue();
				this.activeScenario.init();
			}

			this.scenario = active;
		}

		protected override bool winConditionAchieved() {
			return false;
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			foreach (var button in this.buttons) {
				button.update(elapsed);
			}

			if (this.activeScenario != null) {
				this.activeScenario.update(elapsed);
				if (this.activeScenario.Completed) {
					if (this.activeScenarios.Count > 0) {
						this.activeScenario = this.activeScenarios.Dequeue();
						this.activeScenario.init();
						this.scenario ++;
					}
				}
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			foreach (var button in this.buttons) {
				button.render(spriteBatch);
#if DEBUG
				if (Debug.debugOn) {
					DebugUtils.drawRectangle(spriteBatch, button.TexturedButton.PickableArea, Debug.DEBUG_BBOX_Color, Debug.debugChip);
				}
#endif
			}
			if (this.activeScenario != null) {
				this.activeScenario.render(spriteBatch);
			}
		}
		#endregion Support methods
	}
}

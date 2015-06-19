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
using WOA3.Logic.StateMachine;
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model.Scenarios;

namespace WOA3.Model.Display {
	public class TutorialDisplay : GameDisplay {
		#region Class variables
		private BaseTutorialScenario activeScenario;
		private Queue<BaseTutorialScenario> activeScenarios;
		private int scenario;
		private List<TexturedEffectButton> buttons;

		private readonly string[] BUTTON_NAMES = { "Reset", "Menu" };
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public TutorialDisplay(GraphicsDevice graphics, ContentManager content, GameStateMachine gameStateMachine) :base(graphics, content, "Tutorial", gameStateMachine) {			
			float xBuffer = 256;
			float yBuffer = 128;
			float leftSideX = Constants.RESOLUTION_X  - xBuffer;
			float y = Constants.RESOLUTION_Y - yBuffer;

			this.gameStateMachine = gameStateMachine;
			this.buttons = new List<TexturedEffectButton>();
			this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(leftSideX,y), BUTTON_NAMES[0]));
			this.buttons.Add(ModelGenerationUtil.createButton(content, new Vector2(xBuffer, y), BUTTON_NAMES[1]));

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
			this.activeScenarios.Enqueue(new EnemySpawnTutorial(content, "EnemySpawn", ghost, mob));
			this.activeScenarios.Enqueue(new EvadeTutorial(content, "Evade", ghost, mob));
			this.activeScenarios.Enqueue(new KillingTutorial(content, "Killing", ghost, mob, this.allGhosts));
			this.activeScenarios.Enqueue(new ArmyTutorial(content, "Army", ghost, mob, allGhosts, this.gameStateMachine));
			for (int i = 0; i <= active; i++) {
				// the last scenario requires 2 ghosts
				if (0 == this.activeScenarios.Count - 1) {
					break;
				}
				this.activeScenario = this.activeScenarios.Dequeue();
			}

			this.scenario = active;
		}

		public override void update(float elapsed) {
			base.update(elapsed);
			foreach (TexturedEffectButton button in this.buttons) {
				button.update(elapsed);
				button.processActorsMovement(InputManager.getInstance().MousePosition);
			}
			if (InputManager.getInstance().wasLeftButtonPressed()) {
				foreach (TexturedEffectButton button in this.buttons) {
					if (button.isActorOver(InputManager.getInstance().MousePosition)) {
						// we clicked a button
						if (button.Texture.Name.Equals(BUTTON_NAMES[0])) {
							init(this.scenario);
						} else if (button.Texture.Name.Equals(BUTTON_NAMES[1])) {
							this.gameStateMachine.goToPreviousState();
						}
						break;
					}
				}
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
			foreach (TexturedEffectButton button in this.buttons) {
				button.render(spriteBatch);
#if DEBUG
				if (Debug.debugOn) {
					DebugUtils.drawRectangle(spriteBatch, button.PickableArea, Debug.DEBUG_BBOX_Color, Debug.debugChip);
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

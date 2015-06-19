using System;
using System.Collections.Generic;

using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
using GWNorthEngine.Logic;
using GWNorthEngine.Logic.Params;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using WOA3.Engine;
using WOA3.Logic;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;
using WOA3.Logic.Skills;

namespace WOA3.Model {
	public class Ghost : Character {
		private enum State { Visisble, Invisible };
		#region Class variables
		private Tracking seeking;
		private StaticDrawable2D selectedImg;
		private FadeEffect fadeEffect, selectedFadeEffect;
		private State state;
		private GhostObservationHandler observerHandler;

		private Dictionary<Keys, Skill> aggressiveSkills;
		private Dictionary<Keys, Skill> passiveskills;

		private const float SPEED = .5f;//.2f;
		#endregion Class variables

		#region Class propeties
		public bool Selected { get; set; }
		#endregion Class properties

		#region Constructor
		public Ghost(ContentManager content, Vector2 position, GhostObservationHandler observerHandler, CharactersInRange charactersInRange)
			: base(content, position, SPEED, charactersInRange) {
			
			this.observerHandler = observerHandler;
			Texture2D texture = LoadingUtils.load<Texture2D>(content, "Ghost");

			StaticDrawable2DParams characterParms = new StaticDrawable2DParams {
				Position = position,
				Texture = texture,
				Origin = new Vector2(Constants.TILE_SIZE/2)
			};

			base.init(new StaticDrawable2D(characterParms));

			Texture2D radiusTexture =  LoadingUtils.load<Texture2D>(content, "Ring");
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Position = position;
			parms.LightColour = Color.LimeGreen;
			parms.Texture = radiusTexture;
			parms.Origin = new Vector2(Constants.TILE_SIZE/2, -(Constants.TILE_SIZE / 4));
			
			this.selectedImg = new StaticDrawable2D(parms);
			this.seeking = new Tracking(position, SPEED);
			this.fadeEffect = createEffect(base.LightColour);
			this.addEffect(fadeEffect);
			this.selectedFadeEffect = createEffect(this.selectedImg.LightColour);

			initSkills();
		}
		#endregion Constructor

		#region Support methods
		private void initSkills() {
			SkillFinished appear = delegate() {
				int alpha = 255;
				resetFadeEffect(this.fadeEffect, alpha, FadeEffect.FadeState.PartialIn);
				resetFadeEffect(this.selectedFadeEffect, alpha, FadeEffect.FadeState.PartialIn);
				this.state = State.Visisble;
			};
			SkillFinished disappear = delegate() {
				int alpha = 75;
				resetFadeEffect(this.fadeEffect, alpha, FadeEffect.FadeState.PartialOut);
				resetFadeEffect(this.selectedFadeEffect, alpha, FadeEffect.FadeState.PartialOut);
				this.state = State.Invisible;
				this.observerHandler.notifyGhostChange(this);
			};

			SkillFinished shriek = delegate() {
				int alpha = 255;
				resetFadeEffect(this.fadeEffect, alpha, FadeEffect.FadeState.PartialIn);
				resetFadeEffect(this.selectedFadeEffect, alpha, FadeEffect.FadeState.PartialIn);
				this.state = State.Visisble;

				float effectLife;
				Base2DSpriteDrawable shockWave = ModelGenerationUtil.generateWaveEffect(content, base.Position, Color.Orange, out effectLife);
				VisualEffect effect = new VisualEffect(shockWave, effectLife);
				EffectsManager.getInstance().Visuals.Add(effect);
			};

			SkillFinished boo = delegate() {
				int alpha = 255;
				resetFadeEffect(this.fadeEffect, alpha, FadeEffect.FadeState.PartialIn);
				resetFadeEffect(this.selectedFadeEffect, alpha, FadeEffect.FadeState.PartialIn);
				this.state = State.Visisble;


				float effectLife;
				Base2DSpriteDrawable shockWave = ModelGenerationUtil.generateWaveEffect(content, base.Position, Color.Green, out effectLife);
				VisualEffect effect = new VisualEffect(shockWave, effectLife);
				EffectsManager.getInstance().Visuals.Add(effect);
			};

			this.aggressiveSkills = new Dictionary<Keys, Skill>();
			this.aggressiveSkills.Add(Keys.D1, new Boo(boo));
			this.aggressiveSkills.Add(Keys.D2, new Shriek(shriek));

			this.passiveskills = new Dictionary<Keys, Skill>();
			this.passiveskills.Add(Keys.D3, new Appear(appear));
			this.passiveskills.Add(Keys.D4, new Disappear(disappear));
		}

		private void resetFadeEffect(FadeEffect effect, int alphaAmount, FadeEffect.FadeState state) {
			effect.AlphaAmount = alphaAmount;
			effect.State = state;
			effect.reset();
		}

		private FadeEffect createEffect(Color color) {
			PartialFadeEffectParams fadeParms = new PartialFadeEffectParams() {
				OriginalColour = color,
				TotalTransitionTime = 500f,
				State = FadeEffect.FadeState.PartialIn,
			};
			return new FadeEffect(fadeParms);
		}

		public bool isVisible() {
			return State.Visisble.Equals(this.state);
		}

		public override List<SkillResult> performSkills() {
			List<SkillResult> results = new List<SkillResult>();
			if (Selected) {
				List<Character> charactersInRange = this.CharactersInRange.Invoke(this.Range);
				if (Constants.ALLOW_PLAYER_ATTACKS) {
					foreach (var skill in aggressiveSkills) {
						if (InputManager.getInstance().wasKeyPressed(skill.Key)) {
							CombatManager.getInstance().CombatRequests.Add(new CombatRequest() {
								Skill = skill.Value,
								Source = this,
								Targets = charactersInRange
							});
						}
					}
				}
				foreach (var skill in passiveskills) {
					if (InputManager.getInstance().wasKeyPressed(skill.Key)) {
						CombatManager.getInstance().CombatRequests.Add(new CombatRequest() {
							Skill = skill.Value,
							Source = this,
							Targets = charactersInRange
						});
					}
				}
			}
			return results;
		}

		public override Skill die() {
			return null;
		}
		
		public override void update(float elapsed) {
			base.update(elapsed);
			this.seeking.update(elapsed);
			base.Position = this.seeking.Position;
			this.selectedImg.update(elapsed);
			this.selectedImg.Position = base.Position;

			if (Selected) {
				// update our skills
				foreach (var skill in aggressiveSkills) {
					skill.Value.update(elapsed);
				}
				foreach (var skill in passiveskills) {
					skill.Value.update(elapsed);
				}

				if (InputManager.getInstance().wasRightButtonPressed()) {
					this.seeking.Target = GWNorthEngine.Input.InputManager.getInstance().MousePosition;
				}
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			if (Selected) {
				this.selectedImg.render(spriteBatch);
			}
			base.render(spriteBatch);
		}
		#endregion Support methods
	}
}

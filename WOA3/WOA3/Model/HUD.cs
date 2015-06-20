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
using WOA3.Engine;
using WOA3.Map;
using WOA3.Model.Display;

namespace WOA3.Model {
	public class HUD : IRenderable {

		private int previousSelectedAmount;
		private ContentManager content;
		private List<HUDIcon> icons;
		private List<HUDSelected> selected;
		private ChangeSelected changeSelected;

		private const int SELECTED_MAX_IN_ROW = 2;
		private const float SPACE = 16f;

		public HUD(ContentManager content, ChangeSelected changeSelected) {
			this.content = content;
			this.icons = new List<HUDIcon>();
			this.selected = new List<HUDSelected>();
			this.changeSelected = changeSelected;


			Vector2 hudPosition = new Vector2(Constants.RESOLUTION_X/3, Constants.RESOLUTION_Y - 96);

			string[] skillIcons = { "Boo", "Blast", "Appear", "Hide" };
			for (int i = 0; i < skillIcons.Length; i++) {
				Vector2 position = Vector2.Add(hudPosition, new Vector2((HUDIcon.ICON_SIZE * i) + (SPACE * (i + 1) + HUDIcon.ICON_SIZE), HUDIcon.ICON_SIZE / 1.5f));
				icons.Add(new HUDIcon(content, position, skillIcons[i]));
			}
		}


		public void update(float elapsed) {
			foreach (var icon in icons) {
				if (icon != null) {
					icon.update(elapsed);
				}
			}
			for (int i = 0; i < selected.Count; i++) {
				if (InputManager.getInstance().wasLeftButtonPressed()) {
					if (PickingUtils.pickVector(InputManager.getInstance().MousePosition, selected[i].BBox)) {
						this.changeSelected.Invoke(i);
						break;
					}
				}
			}
		}

		private class CoolDownDictionary<TKey, TValue> : Dictionary<TKey, TValue> {

			public CoolDownDictionary(int capacity) : base(capacity) {
			}

			public new void Add(TKey key, TValue value) {
				if (this.ContainsKey(key)) {
					this.Remove(key);
				}
				base.Add(key, value);
			}
		}

		public void updateSkills(List<Ghost> ghosts) {
			int count = ghosts.Count;
			if (count > 0) {
				CoolDownDictionary<int, float> coolDowns = new CoolDownDictionary<int, float>(this.icons.Count);
				for (int i = 0; i < this.icons.Count; i++) {
					coolDowns[i] = 100;
				}
				int drawnSelected = 0;
				foreach (Ghost ghost in ghosts) {
					for (int i = 0; i < ghost.Skills.Count; i++) {
						Skill skill = ghost.Skills[i];
						float percent = coolDowns[i];
						float completed = skill.CooldownComplete * 100;
						// get the lowest cooldown complete for the ghosts
						if (percent > completed) {
							coolDowns.Add(i, completed);
						}
					}

					if (count != previousSelectedAmount) {
						if (drawnSelected == 0) {
							selected.Clear();
						}
						float y = Constants.RESOLUTION_Y - 90f;
						float x =Constants.RESOLUTION_X / 4 * 3;
						float offset = 16f;
						if (drawnSelected % SELECTED_MAX_IN_ROW == 0) {
							y += 32f;
						}
						Vector2 position = new Vector2(x + (offset * drawnSelected), y);
						this.selected.Add(new HUDSelected(content, position));
						drawnSelected++;
					}
				}

				for (int i = 0; i < icons.Count; i++) {
					icons[i].setCooldown(coolDowns[i]);
				}
			}
			if (count != previousSelectedAmount) {
				previousSelectedAmount = count;
			}
			if (count == 0) {
				selected.Clear();
			}
		}

		public void render(SpriteBatch spriteBatch) {
			foreach (var icon in icons) {
				if (icon != null) {
					icon.render(spriteBatch);
				}
			}
			foreach (var ghost in selected) {
				ghost.render(spriteBatch);
			}
		}

		public void dispose() {
			throw new NotImplementedException();
		}
	}
}

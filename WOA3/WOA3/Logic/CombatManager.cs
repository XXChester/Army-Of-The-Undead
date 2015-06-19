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
using WOA3.Model;
using WOA3.Logic.AI;
using WOA3.Logic.Behaviours;
using WOA3.Logic.Skills;

namespace WOA3.Logic {
	public class CombatManager {
		// singleton variable
		private static CombatManager instance = new CombatManager();

		#region Class variables
		private List<CombatRequest> combatRequests;
		#endregion Class variables

		#region Class propeties
		public List<CombatRequest> CombatRequests { get { return this.combatRequests; } }
		#endregion Class properties

		#region Constructor
		public CombatManager() {
			
		}
		#endregion Constructor

		#region Support methods
		public static CombatManager getInstance() {
			return instance;
		}

		private CombatRequest getFirst() {
			CombatRequest result = new CombatRequest();
			foreach (var request in combatRequests) {
				result = request;
				combatRequests.Remove(result);
				break;
			}
			return result;
		}

		public void init() {
			this.combatRequests = new List<CombatRequest>();
		}

		private void removeRequests(Character source) {
			foreach (var request in this.combatRequests) {
				if (request.Source.Equals(source)) {
					this.combatRequests.Remove(request);
				}
			}
		}

		public void update(float elapsed) {
			if (this.combatRequests.Count > 0) {
				CombatRequest request = getFirst();
				if (request.Skill != null) {
					SkillResult skillResult = request.Skill.perform(request.Source.Range);
					if (skillResult != null) {
						foreach (var target in request.Targets) {
							if (target != null) {
								if (request.Source != null) {
									// can we see the target?


									target.damage(skillResult.Damage);
									// did we kill the target?
									if (target.AmIDead) {
										removeRequests(target);
										// if we had a death effect it needs to go to the front of the list of actions and apply against all targets in the area
										Skill deathEffect = target.die();
										List<Character> charactersInRange = target.CharactersInRange.Invoke(target.Range);
										this.combatRequests.Insert(0, new CombatRequest() {
											Skill = deathEffect,
											Source = target,
											Targets = charactersInRange
										});
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion Support methods
	}
}

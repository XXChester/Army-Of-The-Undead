using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using WOA3.Logic.Behaviours;

namespace WOA3.Logic {
	public class ScaredFactor {

		public struct ScaredState {
			public float UpperBound { get; set; }
			public float LowerBound { get; set; }

			public bool withinBounds(float value) {
				return value < UpperBound && value >= LowerBound;
			}
		}

		private float factor;
		private ScaredState myState;
		private Dictionary<String, ScaredState> states;

		private const String KEY_COURAGEOUS = "Courageous";
		private const String KEY_STANDARD = "Standard";
		private const String KEY_PETRIFIED = "Petrified";
		private const String KEY_DEAD = "Dead";

		public String Text { get { return this.factor.ToString(); } }
		public float Factor { get { return this.factor; } }
		

		public ScaredFactor(float health) {
			states = new Dictionary<string, ScaredState>();
			states.Add(KEY_DEAD, new ScaredState() { LowerBound = float.MinValue, UpperBound = 0.00001f});
			states.Add(KEY_PETRIFIED, new ScaredState() { LowerBound = 0.00001f, UpperBound = 2.5f });
			states.Add(KEY_STANDARD, new ScaredState() { LowerBound = 2.5f, UpperBound = 5f});
			states.Add(KEY_COURAGEOUS, new ScaredState() { LowerBound = 5f,  UpperBound = float.MaxValue });

			this.myState = states[KEY_STANDARD];
			this.factor = health;
		}

		public bool amIState(String key) {
			return states[key].Equals(myState);
		}

		public bool amIPetrified() {
			return amIState(KEY_PETRIFIED);
		}

		public bool amIDead() {
			return amIState(KEY_DEAD);
		}

		public void scare(float scare) {
			float newFactorIfSuccessful = factor - scare;

			Nullable<ScaredState> newState = null;
			foreach (KeyValuePair<String, ScaredState> entry in states) {
				ScaredState state = states[entry.Key];
				if (state.withinBounds(newFactorIfSuccessful)) {
					newState = state;
					break;
				}
			}

			if (newState != null) {
				// did we scare far enough into dead or petrified?
				//if (states[KEY_PETRIFIED].Equals(newState) || states[KEY_DEAD].Equals(newState)) {
					factor = newFactorIfSuccessful;
				/*} else {
					factor += 1;
				}*/
				myState = (ScaredState)newState;
			}
		}
	}
}

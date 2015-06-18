using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WOA3.Model.Display;

namespace WOA3.Logic.StateMachine {
	public interface State {
		void goToNextState();

		IRenderable getCurrentScreen();
		
		void goToPreviousState();

		void setStates();
	}
}

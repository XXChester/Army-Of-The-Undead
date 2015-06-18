using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WOA3.Model;

namespace WOA3.Logic {
	public class GhostObservationHandler : IObservable<Ghost> {

		public List<Ghost> Ghosts { get; set; }
		public List<IObserver<Ghost>> Observers { get; set; }


		public void notifyGhostChange(Ghost ghost) {
			if (Observers != null) {
				foreach (var observer in Observers) {
					observer.OnNext(ghost);
				}
			}
		}

		public IDisposable Subscribe(IObserver<Ghost> observer, Ghost ghost) {
			IDisposable result = Subscribe(observer);
			observer.OnNext(ghost);
			return result;
		}

		public IDisposable Subscribe(IObserver<Ghost> observer) {
			if (this.Observers == null) {
				this.Observers = new List<IObserver<Ghost>>();
			}
			if (!this.Observers.Contains(observer)) {
				this.Observers.Add(observer);
			}
			return new Unsubscriber(delegate() {
				this.Observers.Remove(observer);
			});
		}
	}
}

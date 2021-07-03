using System;

namespace Warrior_App.Model {
	public class GoalOccurrence {
		public int ID { get; set; }
		public DateTime Date { get; set; }
		public Goal Goal { get; set; }
	}
}
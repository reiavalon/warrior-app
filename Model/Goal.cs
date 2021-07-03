using System;

namespace Warrior_App.Model {
	public class Goal {
		public int ID { get; set; }
		public int GoalNumber { get; set; }
		public string GoalTitle { get; set; }
		public string GoalDescription { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public bool IsActive { get; set; }
	}
}
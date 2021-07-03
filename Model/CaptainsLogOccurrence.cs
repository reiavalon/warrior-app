using System;

namespace Warrior_App.Model {
	public class CaptainsLogOccurrence {
		public int ID { get; set; }
		public DateTime Date { get; set; }
		public string Anwser { get; set; }
		public CaptainsLog CaptainsLog { get; set; }
	}
}
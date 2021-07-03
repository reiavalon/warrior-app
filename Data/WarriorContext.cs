using Microsoft.EntityFrameworkCore;
using Warrior_App.Model;

namespace Warrior_App.Data {
	public class WarriorContext : DbContext {
		public WarriorContext (DbContextOptions<WarriorContext> options) : base(options) {}

		public DbSet<CaptainsLog> CaptainsLogs { get; set; }
		public DbSet<CaptainsLogOccurrence> CaptainsLogOccurrences { get; set; }
		public DbSet<Goal> Goals { get; set; }
		public DbSet<GoalOccurrence> GoalOccurrences { get; set; }
		public DbSet<LostBattle> LostBattles { get; set; }
	}
}
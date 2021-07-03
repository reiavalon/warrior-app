using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Warrior_App.Data;

namespace Warrior_App.Model
{
	public static class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new WarriorContext(
				serviceProvider.GetRequiredService<
					DbContextOptions<WarriorContext>>()))
			{
				if (context.CaptainsLogs.Any()) { }
				else {
					context.CaptainsLogs.AddRange(
						new CaptainsLog {
							QuestionNumber = 1,
							Question = "Why are you fighting? Why don’t you just give up?"
						},
						new CaptainsLog {
							QuestionNumber = 2,
							Question = "How did you win your most recent difficult battle? What have you been doing right when you win?"
						},
						new CaptainsLog {
							QuestionNumber = 3,
							Question = "Are you keeping the daily MAN/GRL power journal? What day are you on now?"
						},
						new CaptainsLog {
							QuestionNumber = 4,
							Question = "What are you doing for your MAN/GRL goals? " +
								"Do you have meaningful rituals in place? Are they sufficient? " +
            					"What is your Flag Pole for drilling to help you achieve your goals? " + 
            					"Have you drilled them at least 3 to 5 times today?"
						},
						new CaptainsLog {
							QuestionNumber = 5,
							Question = "When you lost, what technique did the enemy use to defeat you? " +
								"Is there a pattern? If you could replay the event, what could you have " + 
								"done to beat him? What drills can you do to make sure you win next time " +
								"if he tries something similar?"
						},
						new CaptainsLog {
							QuestionNumber = 6,
							Question = "Prophesy: What strategy will the adversary try in the next week? " + 
            				"What do you need to do to be ready for such an attack?"
						}
					);
					context.SaveChanges();
				}

				if (context.Goals.Any()) { }
				else {
					context.Goals.AddRange(
						new Goal {
							GoalNumber = 1,
							GoalTitle = "M",
							DateCreated = DateTime.Now,
							DateModified = DateTime.Now,
							IsActive = true
						},
						new Goal {
							GoalNumber = 2,
							GoalTitle = "A",
							DateCreated = DateTime.Now,
							DateModified = DateTime.Now,
							IsActive = true
						},
						new Goal {
							GoalNumber = 3,
							GoalTitle = "N",
							DateCreated = DateTime.Now,
							DateModified = DateTime.Now,
							IsActive = true
						},
						new Goal {
							GoalNumber = 4,
							GoalTitle = "P",
							DateCreated = DateTime.Now,
							DateModified = DateTime.Now,
							IsActive = true
						},
						new Goal {
							GoalNumber = 5,
							GoalTitle = "W",
							DateCreated = DateTime.Now,
							DateModified = DateTime.Now,
							IsActive = true
						},
						new Goal {
							GoalNumber = 6,
							GoalTitle = "R",
							DateCreated = DateTime.Now,
							DateModified = DateTime.Now,
							IsActive = true
						}
					);
					context.SaveChanges();
				}
			}
		}
	}
}
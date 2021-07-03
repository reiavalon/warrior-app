using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warrior_App.Model;
using Warrior_App.Data;
using System.Linq;

namespace Warrior_App.Controller {
	public class LostBattleInput
	{
		public DateTime date { get; set; }
	}

	public class GoalInput
	{
		public DateTime date { get; set; }
		public int goalNumber { get; set; }
	}

	public class QuestionInput
	{
		public DateTime date { get; set; }
		public int captainsLogNumber { get; set; }
		public string anwser { get; set; }
	}

	public class WarriorController : Microsoft.AspNetCore.Mvc.Controller
	{
		private readonly WarriorContext _context;

		public WarriorController(WarriorContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult> BattleView()
        {
			return View();
        }

		[HttpGet]
		public async Task<ActionResult> CaptainsLogView()
        {
			return View();
        }

		// GET: warrior/LostBattles
		[HttpGet]
		public async Task<ActionResult<IEnumerable<DateTime>>> LostBattles()
		{
			return await _context.LostBattles.Select(elem => elem.Date).ToListAsync();
		}

		// GET: warrior/Goals
		[HttpGet]
		public async Task<ActionResult<object>> Goals()
        {
			Dictionary<DateTime, List<int>> output = new Dictionary<DateTime, List<int>>();
			var goalOccurrences = await _context.GoalOccurrences.Select(elem => new
			{
				date = elem.Date,
				goalNumber = elem.Goal.GoalNumber
			}).ToListAsync();

			foreach(var goal in goalOccurrences)
            {
				if(!output.ContainsKey(goal.date))
                {
					output.Add(goal.date, new List<int>());
                }
				output[goal.date].Add(goal.goalNumber);
            }

			return output.Select(elem => new { date = elem.Key, goals = elem.Value }).ToList();
        }

		// GET: warrior/CaptainsLogSummary
		[HttpGet]
		public async Task<ActionResult<object>> CaptainsLogSummary()
        {
			Dictionary<DateTime, int> output = new Dictionary<DateTime, int>();
			var captainsLogOccurrences = await _context.CaptainsLogOccurrences.Select(elem => new
			{
				date = elem.Date,
				captainsLogNumber = elem.CaptainsLog.QuestionNumber
			}).ToListAsync();
			
			foreach(var captainsLog in captainsLogOccurrences)
            {
				if(!output.ContainsKey(captainsLog.date))
                {
					output.Add(captainsLog.date, 0);
                }
				output[captainsLog.date]++;
            }

			return output.Select(elem => new { date = elem.Key, captainsLogCount = elem.Value }).ToList();
        }

		// GET: warrior/CaptainsLog
		[HttpGet]
		public async Task<ActionResult<object>> CaptainsLog(DateTime date)
        {
			Dictionary<int, string> output = new Dictionary<int, string>();
			var captainsLogOccurrences = await _context.CaptainsLogOccurrences
				.Where(elem => elem.Date.Date == date.Date)
				.Select(elem => new
				{
					captainsLogNumber = elem.CaptainsLog.QuestionNumber,
					anwser = elem.Anwser
				}).ToListAsync();
			foreach(var captainsLog in captainsLogOccurrences)
            {
				output.Add(captainsLog.captainsLogNumber, captainsLog.anwser);
            }

			return output.Select(elem => new { questionNumber = elem.Key, anwser = elem.Value }).ToList();
        }

		// POST: warrior/AddLostBattle
		[HttpPost]
		public async Task<ActionResult> AddLostBattle([FromBody] LostBattleInput input)
		{
			if (await _context.LostBattles.Where(elem => elem.Date == input.date.Date).CountAsync() == 0)
			{
				await _context.LostBattles.AddAsync(new LostBattle { Date = input.date.Date });
				await _context.SaveChangesAsync();
			}
			return Ok();
		}

		// POST: warrior/RemoveLostBattle
		[HttpPost]
		public async Task<ActionResult> RemoveLostBattle([FromBody] LostBattleInput input)
		{
			LostBattle lostBattle = await _context.LostBattles.Where(elem => elem.Date == input.date.Date).FirstOrDefaultAsync();
			if(lostBattle != null)
			{
				_context.Remove(lostBattle);
				await _context.SaveChangesAsync();
			}
			return Ok();
		}

		private IQueryable<GoalOccurrence> GoalQuery([FromBody] GoalInput input)
        {
			return _context.GoalOccurrences
				.Where(elem => elem.Date.Date == input.date.Date && elem.Goal.GoalNumber == input.goalNumber);
		}

		// POST: warrior/AddGoal
		[HttpPost]
		public async Task<ActionResult> AddGoal([FromBody] GoalInput input)
		{
			if (await GoalQuery(input).CountAsync() == 0)
            {
				Goal goal = await _context.Goals.Where(elem => elem.GoalNumber == input.goalNumber).FirstOrDefaultAsync();
				_context.GoalOccurrences.Add(new GoalOccurrence
				{
					Date = input.date,
					Goal = goal
				});
				await _context.SaveChangesAsync();
            }
			return Ok();
		}

		// POST: warrior/RemoveGoal
		[HttpPost]
		public async Task<ActionResult> RemoveGoal([FromBody] GoalInput input)
		{
			GoalOccurrence goalOccurrence = await GoalQuery(input).FirstOrDefaultAsync();
			if(goalOccurrence != null)
            {
				_context.GoalOccurrences.Remove(goalOccurrence);
				await _context.SaveChangesAsync();
            }
			return Ok();
		}

		// POST: warrior/UpdateCaptainsLog
		[HttpPost]
		public async Task<ActionResult> UpdateCaptainsLog([FromBody] QuestionInput input)
		{
			CaptainsLogOccurrence captainsLogOccurrence = await _context.CaptainsLogOccurrences
				.Where(elem => elem.CaptainsLog.QuestionNumber == input.captainsLogNumber && elem.Date.Date == input.date)
				.FirstOrDefaultAsync();
			if(captainsLogOccurrence == null)
            {
				if (!String.IsNullOrWhiteSpace(input.anwser))
				{
					_context.CaptainsLogOccurrences.Add(
						new CaptainsLogOccurrence
						{
							Date = input.date.Date,
							Anwser = input.anwser,
							CaptainsLog = await _context.CaptainsLogs.Where(elem => elem.QuestionNumber == input.captainsLogNumber).FirstOrDefaultAsync()
						}
					);
					await _context.SaveChangesAsync();
				}
            }
			else
            {
				if(String.IsNullOrWhiteSpace(input.anwser))
                {
					_context.CaptainsLogOccurrences.Remove(captainsLogOccurrence);

                }
				else
                {
					captainsLogOccurrence.Anwser = input.anwser;
                }
				await _context.SaveChangesAsync();
            }
			return Ok();
		}
	}
}
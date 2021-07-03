import express from 'express';
import MySqlite3 from './sqlite3Wrapper';

const router = express.Router();

let warriorDb = new MySqlite3('./Warrior.db');

function select(inputArray: any[], modFunc: (elem: any) => any)
{
	let output = [];
	for (let elem of inputArray) { output.push(modFunc(elem)); }
	return output;
}

function dateMod(input: string): string {
	return input.replace(' 00:00:00', '');
	//return new Date(input).toLocaleDateString();
}

function getInt(input: any)
{
    let output = input ? Number.parseInt(input) : null;
    return output !== Number.NaN ? output : null;
}

function toDateStr(req_param: any): string
{
	let month = getInt(req_param.month);
	let day = getInt(req_param.day);
	let year = getInt(req_param.year);

	let dateStr = null;

	if(month !== null && day !== null && year !== null)
	{
		function zeroPad(input: number, pad: number): string {
			return String(input).padStart(pad, '0');
		}
		dateStr = `${zeroPad(year,4)}-${zeroPad(month,2)}-${zeroPad(day,2)} 00:00:00`;
	}

	return dateStr;
}

interface LostBattle { LostBattles: string; }

router.get('/LostBattles', async (req, res) => {
	await warriorDb.query<LostBattle>(res, 'SELECT * FROM [LostBattles]', [], 
	(inputArray) => select(inputArray, elem => elem.Date));
});

interface GoalSummary { Date: string; GoalNumber: number; }

router.get('/GoalSummary', async (req, res) => {
	await warriorDb.query<GoalSummary>(res,
	`SELECT [Date],[GoalNumber] 
	FROM [GoalOccurrences] o 
	JOIN [Goals] g ON o.[GoalID]=g.[ID];`,
	[],
	(queryResult) => {
		let output: {[key: string]: number[]} = {};
		for(let elem of queryResult)
		{
			if(!output[elem.Date]) { output[elem.Date] = []; }
			output[elem.Date].push(elem.GoalNumber);
		}
		return output;
	});
});

interface CaptainsLogSummary { Date: string; QuestionNumber: number; }

router.get('/CaptainsLogSummary', async (req, res) => {
	await warriorDb.query<CaptainsLogSummary>(res,
	`SELECT [Date], [QuestionNumber]
	FROM [CaptainsLogOccurrences] o
	JOIN [CaptainsLogs] l ON o.CaptainsLogID=l.ID;`,
	[],
	(queryResult) => {
		let output: {[key: string]: number[]} = {};
		for(let elem of queryResult)
		{
			if(!output[elem.Date]) { output[elem.Date] = []; }
			output[elem.Date].push(elem.QuestionNumber);
		}
		return output;
	});
});

interface CaptainsLog {
	QuestionNumber: number;
	Anwser: string;
}

router.get('/CaptainsLog', async (req, res) => {
	let dateStr = toDateStr(req.query);

	if(dateStr !== null)
	{
		await warriorDb.query<CaptainsLog>(res,
		`SELECT [QuestionNumber],[Anwser]
		FROM [CaptainsLogOccurrences] o
		JOIN [CaptainsLogs] l ON o.CaptainsLogID = l.ID
		WHERE date(o.Date)=date(?);`,
		[dateStr]);
	}
	else
	{
		res.status(400).send();
	}
	
});

router.post('/AddLostBattle', async (req, res) => {
	let dateStr = toDateStr(req.body);
	
	if(dateStr !== null)
	{
		await warriorDb.run(res,
			`REPLACE INTO [LostBattles] (Date)
			VALUES (datetime(?));`,
			[dateStr]);
	}
	else
	{
		res.status(400).send();
	}
});

router.post('/RemoveLostBattle', async (req, res) => {
	let dateStr = toDateStr(req.body);

	await warriorDb.run(res,
		`DELETE FROM [LostBattles]
		WHERE date([Date])=date(?);`,
		[dateStr]);
});

interface GoalID {
	ID: number;
}

router.post('/AddGoal', async (req, res) => {
	let dateStr = toDateStr(req.body);
	let goalNumber = getInt(req.body.goalNumber);

	if(dateStr !== null && goalNumber !== null)
	{
		await warriorDb.run(res, 
			`INSERT INTO [GoalOccurrences] ([Date], [GoalID])
			SELECT 
				datetime(?) AS [Date], 
				(SELECT [ID] AS [GoalID] FROM [Goals] WHERE [GoalNumber]==?) AS [GoalID]
			WHERE NOT EXISTS(
				SELECT 1 FROM [GoalOccurrences] 
				WHERE date([Date])==date(?) 
				AND [GoalID] IN (SELECT [ID] FROM [Goals] WHERE [GoalNumber]==?)
			);`, [dateStr, goalNumber, dateStr, goalNumber])
	}
	else
	{
		res.status(400).send();
	}

	
});

router.post('/RemoveGoal', async (req, res) => {
	let dateStr = toDateStr(req.body);
	let goalNumber = getInt(req.body.goalNumber);

	if(dateStr !== null && goalNumber !== null)
	{
		await warriorDb.run(res,
			`DELETE FROM [GoalOccurrences]
			WHERE date([Date])==date(?) 
			AND [GoalID] IN (SELECT [ID] FROM [Goals] WHERE [GoalNumber]==?);`,
			[dateStr, goalNumber]);
	}
	else
	{
		res.status(400).send();
	}
});

router.post('/UpdateCaptainsLog', async (req, res) => {
	let dateStr = toDateStr(req.body);
	let captainsLogNumber = getInt(req.body.captainsLogNumber);
	let anwser: string = req.body.anwser;

	if(dateStr !== null && captainsLogNumber !== null)
	{
		if(anwser && anwser.trim() !== '')
		{
			await warriorDb.run(res,
				`REPLACE INTO [CaptainsLogOccurrences] ([Date], [Anwser], [CaptainsLogID])
				SELECT 
					datetime(?) AS [Date], 
					? AS [Anwser], (
						SELECT [ID] 
						FROM [CaptainsLogs] 
						WHERE [QuestionNumber] == ?
					) AS [CaptainsLogID];`,
					[dateStr, anwser, captainsLogNumber]);
		}
		else
		{
			await warriorDb.run(res,
				`DELETE FROM [CaptainsLogOccurrences]
				WHERE date([Date])==date(?) AND
					[CaptainsLogID] IN 	(
						SELECT [ID] 
						FROM [CaptainsLogs] 
						WHERE [QuestionNumber] == ?
					);`,
					[dateStr, captainsLogNumber]);
		}
	}
	else
	{
		res.status(400).send();
	}
});

export default router;
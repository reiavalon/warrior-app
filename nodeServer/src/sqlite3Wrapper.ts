import sqlite3 from 'sqlite3';

export default class MySqlite3
{
    private dbConnectionString: string;
    db: sqlite3.Database = null;

    constructor(dbConnectionString: string)
    {
        this.dbConnectionString = dbConnectionString;
    }

    private async openDatabase(): Promise<void>
    {
        return new Promise((resolve, reject) => {
            this.db = new sqlite3.Database(this.dbConnectionString, (err) => {
                if (err) {
                    reject(err.message);
                }
                else {
                    resolve();
                }
            })
        })
    }

    private async closeDatabase(): Promise<void>
    {
        return new Promise((resolve, reject) => {
            this.db.close((err) => {
                if (err) {
                    reject(err.message);
                }
                else {
                    resolve();
                }
            });
        })
    }

    private async _query(sql: string, params: any[] = []): Promise<any[]>
    {
        return new Promise((resolve, reject) => {
            this.db.all(sql, params, (err, rows) => {
                if (err) {
                    reject(err.message);
                }
                else {
                    resolve(rows);
                }
            });
        })
	}
	
	async query<T>(res: any = null, sql: string, params: any[] = [], modData: (input: T[]) => any = null): Promise<T[]>
	{
		try
		{
			await this.openDatabase();
			let rows = await this._query(sql, params);
			await this.closeDatabase();

			if(modData) { rows = modData(rows); }
			if(res) { res.status(200).send(rows); }

			return rows;
		}
		catch(e)
		{
			if(res) { res.status(500).send(e); }
			return null;
		}
	}

    private async _run(sql: string, params: any[] = []): Promise<void>
    {
        return new Promise((resolve, reject) => {
            this.db.run(sql, params, (err) => {
                if (err) {
                    reject(err.message);
                }
                else {
                    resolve();
                }
            })
        })
	}

	async run(res: any = null, sql: string, params: any[] = []): Promise<boolean>
	{
		try
		{
			await this.openDatabase();
			await this._run(sql, params);
			await this.closeDatabase();

			if(res) { res.status(200).send(); }

			return true;
		}
		catch(e)
		{
			if(res) { res.status(500).send(e); }
			
			return false;
		}
	}
}
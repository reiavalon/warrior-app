import express from "express";
import warrior from './warrior';
import bodyParser from 'body-parser';
import cors from 'cors';

import sqlite3Wrapper from './sqlite3Wrapper';

//const express = require('express');
const app = express();
const port = 5000;

app.use(cors());

app.use(express.static('./src/static'));
app.use(bodyParser.json());
app.use('/warrior', warrior);

app.listen(port, () => {
    console.log(`Example app listening at http://localhost:${port}`);
});
# Web Calls

## GET

warrior/LostBattles
warrior/Goals
warrior/CaptainsLogSummary
warrior/CaptainsLog?date=2021-07-03

## POST

warrior/AddLostBattle
    { date: '2021-07-03' }
warrior/RemoveLostBattle
    { date: '2021-07-03' }
warrior/AddGoal
    { date: '2021-07-03', goalNumber: 1 }
warrior/RemoveGoal
    { date: '2021-07-03', goalNumber: 1 }
warrior/UpdateCaptainsLog
    { date: '2021-07-03', captainsLogNumber: 1, anwser: 'asdf' }
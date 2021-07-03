<script lang="ts">
    import { onMount } from 'svelte';
    import Calendar from '../Components/Calendar.svelte';
    import { DateToIsoProper, DayDifference } from '../Utilities/DateMethods';
    import { getJsonFromUrl, postJsonToUrl } from '../Utilities/HttpMethods';
import { arrayToMap } from '../Utilities/UtilityFunctions';

    interface GoalsElem {
        date: string;
        goals: number[];
    }

    interface GoalsVMElem {
        goalLetter: string;
        isSelected: boolean;
    }

    interface GoalsCountElem {
        date: string;
        goalsCount: number;
    }

    let selectedDate: Date;
    let lostBattles: string[] = [];

    $: selectedDateISO = DateToIsoProper(selectedDate);
    $: isSelectedDateLostBattle = (lostBattles?.indexOf(selectedDateISO) ?? -1) !== -1;
    $: daysSinceLostBattle = DayDifference(latestLostBattle, new Date());

    let latestLostBattle = null;
    $: {
        let filteredLostBattles = lostBattles.map(elem => new Date(elem)).filter(elem => elem < new Date());
        
        if(filteredLostBattles.length > 0) {
            latestLostBattle = filteredLostBattles.reduce((a, b) => { return a > b ? a : b; });
        }
        else {
            latestLostBattle = null;
        }
    }

    let manPwrGoalsThisWeek = 0;

    let goalVM: {[key: string]: GoalsVMElem[]} = {};

    function getEmptyGoals(): GoalsVMElem[] {
        return [
            { goalLetter: 'M', isSelected: false },
            { goalLetter: 'A', isSelected: false },
            { goalLetter: 'N', isSelected: false },
            { goalLetter: 'P', isSelected: false },
            { goalLetter: 'W', isSelected: false },
            { goalLetter: 'R', isSelected: false },
        ]
    }

    let goalArray = getEmptyGoals();
    $: {
        if(!goalVM[selectedDateISO]) { goalVM[selectedDateISO] = getEmptyGoals(); }
        goalArray = goalVM[selectedDateISO];
    }

    function isDateFullGoals(date: Date): boolean {
        let output = false;
        if(goalVM[DateToIsoProper(date)]) {
            output = goalVM[DateToIsoProper(date)]?.filter(elem => elem.isSelected)?.length === goalVM[DateToIsoProper(date)]?.length
        }
        return output;
    }

    function getConsecutiveManPwrDays() {
        let consecutiveCount = 0;
        let curDate = new Date();

        if(isDateFullGoals(curDate)) { consecutiveCount += 1; }
        curDate.setDate(curDate.getDate() - 1);
        while(isDateFullGoals(curDate)) {
            consecutiveCount += 1;
            curDate.setDate(curDate.getDate() - 1);
        }
        return consecutiveCount;
    }

    let consecutiveManPwrDays: number = 0;
    let yellowDates: string[] = [];
    let greenDates: string[] = [];
    $: {
        // Added this so changes in goalArray will trigger a response
        goalArray;

        consecutiveManPwrDays = getConsecutiveManPwrDays();

        yellowDates = [];
        greenDates = [];

        for (let [date, value] of Object.entries(goalVM)) {
            let goalsAchieved = value.filter(elem => elem.isSelected).length;

            if(goalsAchieved === value.length) {
                greenDates.push(date);
            }
            else if(goalsAchieved > 0) {
                yellowDates.push(date);
            }
        }
    }

    let manPwrCountWeek: number = 0;
    $: {
        goalArray;

        manPwrCountWeek = 0;

        let endDateISO = DateToIsoProper(new Date());
        let curDate = new Date();
        curDate.setDate(curDate.getDate() - 7);

        while(DateToIsoProper(curDate) < endDateISO) {
            let currentGoals: GoalsVMElem[] = goalVM[DateToIsoProper(curDate)];
            manPwrCountWeek += (currentGoals?.filter(elem => elem.isSelected)?.length) ?? 0;
            curDate.setDate(curDate.getDate() + 1);
        }
    }

    onMount(async () => {
        lostBattles = await getJsonFromUrl('/warrior/LostBattles');
        let goals: GoalsElem[] = await getJsonFromUrl('warrior/Goals');
        goalVM = arrayToMap(goals, elem => elem.date, elem => {
            let output = getEmptyGoals();
            for(let goalNumber of elem.goals) {
                output[goalNumber - 1].isSelected = true;
            }
            return output;
        });
    });

    async function removeLostBattle() {
        let index = lostBattles.indexOf(selectedDateISO);
        lostBattles = [...lostBattles.slice(0, index), ...lostBattles.slice(index + 1)];
        await postJsonToUrl('/warrior/RemoveLostBattle', { date: selectedDateISO });
    }

    async function addLostBattle() {
        lostBattles = [...lostBattles, selectedDateISO];
        await postJsonToUrl('/warrior/AddLostBattle', { date: selectedDateISO });
    }

    async function goalClicked(goalNumber: number) {
        goalArray[goalNumber].isSelected = !goalArray[goalNumber].isSelected;
        let postURL = goalArray[goalNumber].isSelected ? '/warrior/AddGoal' : 'warrior/RemoveGoal';
        await postJsonToUrl(postURL, { date: selectedDateISO, goalNumber: goalNumber + 1 });
    }
</script>

<div>
    {#if lostBattles}
        <div class="header">{daysSinceLostBattle} / {consecutiveManPwrDays} / {manPwrCountWeek}</div>
        <div class="calendar_container">
            <Calendar bind:selectedDate={selectedDate} redDates={lostBattles} yellowDates={yellowDates} greenDates={greenDates}></Calendar>
        </div>

        <div class="manpwr_container">
            <div class="container">
                {#each goalArray as goal, i}
                    <div class={goal.isSelected ? "goalLetter selected" : "goalLetter"} on:click={() => { goalClicked(i) }}>{goal.goalLetter}</div>
                {/each}
            </div>
        </div>

        {#if isSelectedDateLostBattle}
            <button class="button" on:click="{removeLostBattle}">Remove Lost Battle</button>
        {:else}
            <button class="button" on:click="{addLostBattle}">Add Lost Battle</button>
        {/if}
    {/if}
</div>

<style>
    .container {
        display: grid;
        grid-template-columns: 150px 150px 150px;
        padding: 10px;
    }
    .manpwr_container {
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .selected {
        background: black;
        color: white;
    }
    .calendar_container {
        display: flex;
        justify-content: center;
    }
    .header {
        font-weight: bold;
        font-size: 20px;
        padding: 20px;
    }
    .button {
        border: none;
        color: white;
        padding: 15px 32px;
        text-align: center;
        margin: 4px 2px;
        font-size: 16px;
        border: 2px solid transparent;
        background: gray;
        border-radius: 10px;
        width: 210px;
    }
    
    .goalLetter {
        padding: 20px;
        cursor: pointer;
    }
</style>
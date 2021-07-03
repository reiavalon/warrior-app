<script type="ts">
    import { createEventDispatcher } from "svelte";
    import { DateToIsoProper } from "../Utilities/DateMethods";

    const dispatch = createEventDispatcher();

    let dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    let monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    export let selectedDate: Date = new Date();
    export let redDates: string[] = [];
    export let yellowDates: string[] = [];
    export let greenDates: string[] = [];

    dispatch('dateChanged', { date: selectedDate });

    let currentMonth: number = selectedDate.getMonth();
    let currentYear: number = selectedDate.getFullYear();

    let dateArray: Date[][] = [];

    function getDateFromDayOffset(input: Date, offset: number) {
        let output = new Date(input);
        output.setDate(output.getDate() + offset);
        return output;
    }

    function isDateWithinCurrentRange(input: Date) {
        return (input.getMonth() == currentMonth) && (input.getFullYear() == currentYear);
    }

    function areDatesEqual(input1: Date, input2: Date) {
        return DateToIsoProper(input1) === DateToIsoProper(input2);
    }

    function monthOffset(offset: number) {
        let tempDate = new Date(currentYear, currentMonth, 1);
        tempDate.setMonth(tempDate.getMonth() + offset);
        currentMonth = tempDate.getMonth();
        currentYear = tempDate.getFullYear();
    }

    function getDateClass(input: Date, selectedDate: Date, redDates: string[], yellowDates: string[], greenDates: string[]) {
        let output = "day";
        if(areDatesEqual(input, new Date())) { output += ' currentDate'; }
        if(areDatesEqual(input, selectedDate)) { output += ' selectedDate'; }
        if(!isDateWithinCurrentRange(input)) { output += ' notRangeDate'; }
        if(redDates.indexOf(DateToIsoProper(input)) !== -1) { output += ' redDate'; }
        if(yellowDates.indexOf(DateToIsoProper(input)) !== -1) { output += ' yellowDate'; }
        if(greenDates.indexOf(DateToIsoProper(input)) !== -1) { output += ' greenDate'; }
        return output;
    }

    $: selectedDateString = selectedDate.toDateString();
    $: currentMonthString = monthNames[currentMonth];
    $: currentYearString = selectedDate.getFullYear();

    $: {
        dateArray = [];
        let curWeek: number = 0;
        let curDate = new Date(currentYear, currentMonth, 1);

        if(curDate.getDay() !== 0) {
            // Pre-padding
            for(let i = 0; i < curDate.getDay(); i++)
            {
                if(!dateArray[curWeek])
                {
                    dateArray[curWeek] = [];
                }
                dateArray[curWeek].push(getDateFromDayOffset(curDate, -1 * (curDate.getDay() - i)));
            }
        }
        else {
            // Necessary for the following logic
            curWeek = -1;
        }

        // Filling out dates
        while((curDate.getMonth() == currentMonth)
            && (curDate.getFullYear() == currentYear))
        {
            if(curDate.getDay() == 0)              // The current day is Sunday
            {
                curWeek += 1;
            }

            if(!dateArray[curWeek])
            {
                dateArray[curWeek] = [];
            }
            dateArray[curWeek].push(curDate);
            curDate = getDateFromDayOffset(curDate, 1);
        }

        // Post-padding
        while((curDate.getDay() != 0))
        {
            if(!dateArray[curWeek])
            {
                dateArray[curWeek] = [];
            }
            dateArray[curWeek].push(curDate);
            curDate = getDateFromDayOffset(curDate, 1);
        }
    }

    function onDateSelected(newDate: Date) {
        if(!areDatesEqual(selectedDate, newDate)) {
            selectedDate = newDate;
            dispatch('dateChanged', { date: selectedDate });
        }
    }
</script>

<div>
    <div class="calendar">
        <div class="currentDateTitleContainer">
            <div class="monthChangeButton" on:click={() => monthOffset(-1)}> ◀ </div>
            <div class="currentDateTitle">{currentMonthString} {currentYearString}</div>
            <div class="monthChangeButton" on:click={() => monthOffset(1)}> ▶ </div>
        </div>
        <div class="week week-label">
            {#each dayNames as dayName}
                <div class="day dayName">{dayName}</div>
            {/each}
        </div>
        {#each dateArray as week}
        <div class="week">
            {#each week as day}
                <div class={ getDateClass(day, selectedDate, redDates, yellowDates, greenDates) } on:click={() => { onDateSelected(day); } }>
                    {day.getDate()}
                </div>
            {/each}
        </div>
        {/each}
    </div>
</div>

<style>
    .calendar {
        border: 1px solid gray;
        font-size: 16px;
        width: 290px;
    }
    .week {
        display: flex;
        justify-content: center;
    }
    .day {
        width: 30px;
        height: 30px;
        display: flex;
        align-items: center;
        justify-content: center;
        border: 2px solid transparent;
        margin: 2px;
        border-radius: 32px;
        cursor: pointer;
    }

    .dayName {
        cursor: default;
    }
    .notRangeDate {
        color: gray;
    }
    .currentDate {
        background: gray;
        color: white;
    }
    .selectedDate {
        border: 2px solid blue;
        background: blue;
        border-radius: 32px;
        color: white;
    }
    .selectedDateTitle {
        padding: 20px 0px;
        background: gray;
        color: white;
    }
    .currentDateTitleContainer {
        padding: 20px 0px;
        display: flex;
    }
    .currentDateTitle {
        flex: 1;
    }

    .monthChangeButton {
        padding: 0px 20px;
        cursor: pointer;
    }

    .week-label {
        font-weight: bold;
    }

    .greenDate {
        background: green;
        color: white;
    }

    .yellowDate {
        background: goldenrod;
        color: white;
    }

    .redDate {
        background: red;
        color: white;
    }
</style>
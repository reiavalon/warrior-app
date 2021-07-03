<script lang="ts">
    import { onMount } from 'svelte';
    import Calendar from '../Components/Calendar.svelte';
    import { DateToIsoProper, DayDifference } from '../Utilities/DateMethods';
    import { getJsonFromUrl, postJsonToUrl } from '../Utilities/HttpMethods';

    let selectedDate: Date;
    let lostBattles: string[];

    $: selectedDateString = selectedDate?.toDateString();
    $: isSelectedDateLostBattle = (lostBattles?.indexOf(selectedDateString) ?? -1) !== -1;
    $: latestLostBattle = lostBattles?.map(elem => new Date(elem)).filter(elem => elem < new Date()).reduce((a, b) => { return a > b ? a : b; })
    $: daysSinceLostBattle = DayDifference(latestLostBattle, new Date());

    onMount(async () => {
        lostBattles = (await getJsonFromUrl('/warrior/LostBattles')).map(lostBattle => new Date(lostBattle).toDateString());
    });

    async function removeLostBattle() {
        let index = lostBattles.indexOf(selectedDateString);
        lostBattles = [...lostBattles.slice(0, index), ...lostBattles.slice(index + 1)];
        await postJsonToUrl('/warrior/RemoveLostBattle', { date: DateToIsoProper(selectedDate) });
    }

    async function addLostBattle() {
        lostBattles = [...lostBattles, selectedDateString];
        await postJsonToUrl('/warrior/AddLostBattle', { date: DateToIsoProper(selectedDate) });
    }
</script>

{#if lostBattles}
    <div>{daysSinceLostBattle}</div>
    <Calendar bind:selectedDate={selectedDate} redDates={lostBattles}></Calendar>

    {#if isSelectedDateLostBattle}
        <button on:click="{removeLostBattle}">Remove Lost Battle</button>
    {:else}
        <button on:click="{addLostBattle}">Add Lost Battle</button>
    {/if}
{/if}
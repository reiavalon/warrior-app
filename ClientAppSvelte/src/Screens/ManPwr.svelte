<script lang="ts">
    async function getJsonFromUrl(url: string) {
        let result = await fetch(url);
        return await result.json();
    }

    let lostBattles = [];

    (async () => {
        lostBattles = (await getJsonFromUrl('/warrior/LostBattles')).map(lostBattle => lostBattle.split('T')[0]);
    })();
</script>

<div>
    {#each lostBattles as lostBattle}
        <div>{lostBattle}</div>
    {/each}
</div>
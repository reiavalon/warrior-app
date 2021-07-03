<script lang="ts">
	import ManPwr from './Screens/ManPwr.svelte';
	import CaptainsLog from './Screens/CaptainsLog.svelte';

	let i = 0;

	let showLinks = false;

	function linkClicked(newValue: number) {
		i = newValue;
		showLinks = false;
	}

	let showSnackbar: boolean = false;
	let snackbarMessage: string = '';

	function showSnackbarMessage(message: string) {
		snackbarMessage = message;
		showSnackbar = true;
		setTimeout(() => { showSnackbar = false; }, 3000);
	}

	document.addEventListener('show-toaster', (elem) => showSnackbarMessage((<any>elem).detail));
</script>

<div class="topnav">
	<div class="container">
		<div class="icon containerElem" on:click={() => { showLinks = !showLinks }}>
			<div class="bar1"></div>
			<div class="bar2"></div>
			<div class="bar3"></div>
		</div>
		<div class="appTitle containerElem">Eternal Warrior App</div>
	</div>
	<div style="display:{showLinks ? "flex" : "none"}; position: absolute; z-index: 1;" >
		<div class="links">
			<div on:click={() => linkClicked(0)}>MAN PWR</div>
			<div on:click={() => linkClicked(1)}>Captain's Log</div>
		</div>
		<div style="flex:1; background: white;"></div>
	</div>
	
</div>

<main>
	<div>
		{#if i === 0}
			<ManPwr></ManPwr>
		{:else if i === 1}
			<CaptainsLog></CaptainsLog>
		{/if}
	</div>
</main>

<div class="snackbar {showSnackbar ? "show" : ""}">{snackbarMessage}</div>

<style>
	main {
		text-align: center;
		padding: 1em;
		margin: 0 auto;
	}

	.topnav {
		background-color: black;
	}

	.topnav .links {
		background-color: black;
	}

	.topnav .links div {
		padding: 15px;
		font-size: 17px;
		color: white;
	}

	.topnav .links div:hover {
		background: white;
		color: black;
	}

	.topnav .container {
		display: flex;
		color: white;
		font-size: 17px;
	}

	.topnav .container .containerElem {
		padding: 15px;
	}

	.topnav div.icon {
		background: black;
	}

	.topnav .icon:hover {
		background-color: #ddd;
		color: black;
	}

	.snackbar {
		visibility: hidden;
		min-width: 250px;
		margin-left: -125px;
		background-color: #333;
		color: #fff;
		text-align: center;
		border-radius: 2px;
		padding: 16px;
		position: fixed;
		z-index: 1;
		left: 50%;
		bottom: 0px;
		font-size: 17px;
		opacity: 0;
		transition: opacity 0.5s ease-in, visibility ease-in 0.5s, bottom ease-in 0.5s;
	}

	.snackbar.show {
		visibility: visible;
		opacity: 1;
		bottom: 30px;
		transition: opacity 0.5s ease-in, visibility ease-in 0.5s, bottom ease-in 0.5s;
	}

	.icon .bar1, .icon .bar2, .icon .bar3 {
		width: 35px;
		height: 5px;
		background-color: white;
		margin: 6px 0;
		padding: 0px;
	}

	
	.icon:hover .bar1, .icon:hover .bar2, .icon:hover .bar3 {
		background-color: black;
	}

	.icon .change .bar1 {
		-webkit-transform: rotate(-45deg) translate(-9px, 6px);
		transform: rotate(-45deg) translate(-9px, 6px);
	}

	.icon .change .bar2 {opacity: 0;}

	.icon .change .bar3 {
		-webkit-transform: rotate(45deg) translate(-8px, -8px);
		transform: rotate(45deg) translate(-8px, -8px);
	}

	.appTitle {
		font-size: 24px;
	}
</style>

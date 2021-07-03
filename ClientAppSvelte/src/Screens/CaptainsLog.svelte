<script lang="ts">
    import { onMount } from 'svelte';
    import Calendar from '../Components/Calendar.svelte';
import { DateToIsoProper } from '../Utilities/DateMethods';
    import { getJsonFromUrl, postJsonToUrl } from '../Utilities/HttpMethods';
    
    let selectedDate: Date = new Date();
    $: selectedDateISO = DateToIsoProper(selectedDate);

    let questionArray = [
        'Why are you fighting? Why don’t you just give up?',
        'How did you win your most recent difficult battle? ' + 
            'What have you been doing right when you win?',
        'Are you keeping the daily MAN/GRL power journal? ' + 
            'What day are you on now?',
        'What are you doing for your MAN/GRL goals? ' + 
            'Do you have meaningful rituals in place? Are they sufficient? ' +
            'What is your Flag Pole for drilling to help you achieve your goals? ' + 
            'Have you drilled them at least 3 to 5 times today?',
        'When you lost, what technique did the enemy use to defeat you? ' +
            'Is there a pattern? If you could replay the event, what could you have ' + 
            'done to beat him? What drills can you do to make sure you win next time ' +
            'if he tries something similar?',
        'Prophesy: What strategy will Satan try in the next week? ' + 
            'What do you need to do to be ready for such an attack?'
    ];

    interface AnwsersFromUrlElem {
        anwser: string;
        questionNumber: number;
    }

    interface CaptainsLogSummaryElem {
        date: string;
        captainsLogCount: number;
    }

    function generateClipboardContent() {
        let output: string[] = [];
        for(let i = 0; i < anwsers.length; i++) {
            if(anwsers[i]) {
                output.push(`${i + 1} - ${anwsers[i]}`);
                output.push('');
            }
        }
        return output.join('\r\n');
    }

    function copyToClipboardClicked() {
        var el = document.createElement('textarea');
        document.body.appendChild(el);
        el.value = generateClipboardContent()
        el.select();
        document.execCommand('copy');
        document.body.removeChild(el);

        document.dispatchEvent(new CustomEvent('show-toaster', { detail: 'Data copied to clipboard' }));
    }

    async function saveClicked(i: number) {
        isEditable[i] = false;
        // anwsers[i] is already saved
        await postJsonToUrl('warrior/UpdateCaptainsLog', {
            date: selectedDateISO, captainsLogNumber: i + 1, anwser: anwsers[i]
        });
        captainsLogSummary = await getJsonFromUrl('warrior/CaptainsLogSummary');
    }

    let isEditable = [false, false, false, false, false, false];
    let anwsers = ['','','','','',''];

    async function onDateChanged(newDate: Date) {
        let anwsersFromUrl: AnwsersFromUrlElem[] = await getJsonFromUrl(`warrior/CaptainsLog?date=${DateToIsoProper(newDate)}`);
        isEditable = [false, false, false, false, false, false];

        anwsers = ['','','','','',''];
        for(let anwserUrl of anwsersFromUrl) {
            anwsers[anwserUrl.questionNumber - 1] = anwserUrl.anwser;
        }
    }

    let captainsLogSummary: CaptainsLogSummaryElem[] = [];
    onMount(async () => {
        await onDateChanged(selectedDate);
        captainsLogSummary = await getJsonFromUrl('warrior/CaptainsLogSummary');
    });

    async function dateChanged(event) {
        await onDateChanged(event.detail.date);
    }

    $: yellowDates = captainsLogSummary?.filter(elem => elem.captainsLogCount > 0 && elem.captainsLogCount < questionArray.length)?.map(elem => elem.date) ?? [];
    $: greenDates = captainsLogSummary?.filter(elem => elem.captainsLogCount === questionArray.length)?.map(elem => elem.date) ?? [];
</script>

<div>
    <div class="header">Captain's Log</div>
    <div class="calendar_container">
        <Calendar bind:selectedDate={selectedDate} on:dateChanged={dateChanged} yellowDates={yellowDates} greenDates={greenDates}></Calendar>
    </div>
    <button class="button" on:click={copyToClipboardClicked}>Copy Anwsers To Clipboard</button>

    {#each questionArray as question, i}
        <div class="captainsLogComponent">
            <div style="display:flex;">
                <div class="question" style="flex:1;">{ question }</div>
                <div>
                    {#if isEditable[i] }
                        <button class="button" on:click={() => { saveClicked(i) }}>Save</button>
                    {:else}
                        <button class="button" on:click={() => { isEditable[i] = true; } }>Edit</button>
                    {/if}
                </div>
            </div>
            {#if isEditable[i]}
                <textarea class="anwser" rows="4" bind:value={ anwsers[i] }></textarea>
            {:else}
                <div class="anwser">{anwsers[i] ? anwsers[i] : "-"}</div>
            {/if}
        </div>
    {/each}
</div>

<style>
    .header {
        font-weight: bold;
        font-size: 20px;
        padding: 20px;
    }
    .calendar_container {
      display: flex;
      justify-content: center;
    }
    .button {
      border: none;
      color: white;
      text-align: center;
      padding: 5px;
      border: 2px solid transparent;
      background: gray;
      border-radius: 14px;
      margin: 5px;
    }
    .captainsLogComponent {
        display: flex;
        flex-direction: column;
        text-align: left;
        padding: 10px;
    }
    .question {
        font-weight: bold;
    }
    .anwser {
        margin: 10px 0px;
    }
</style>
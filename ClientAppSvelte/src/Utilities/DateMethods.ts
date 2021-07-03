export function DateToIsoProper(date: Date) {
    return date ? `${date.getFullYear().toString().padStart(4, "0")}-${(date.getMonth() + 1).toString().padStart(2, "0")}-${date.getDate().toString().padStart(2, "0")}T00:00:00` : ``;
}

export function GetJustDate(date: Date) {
    let output = null;
    if(date) {
        output = new Date(date);
        output.setHours(0);
        output.setMinutes(0);
        output.setSeconds(0);
        output.setMilliseconds(0);
    }
    return output;
}

export function DayDifference(date1: Date, date2: Date) {
    return Math.round(Math.abs(GetJustDate(date1)?.valueOf() - GetJustDate(date2)?.valueOf()) / (1000*60*60*24));
}
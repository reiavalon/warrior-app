export function arrayToMap<T, U>(input: T[], keyFunc: (elem: T) => string, valueFunc: (elem: T) => U) {
    let output: {[key: string]: U} = {};
    for(let elem of input) {
        output[keyFunc(elem)] = valueFunc(elem);
    }
    return output;
}
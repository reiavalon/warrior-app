export async function getJsonFromUrl(url: string) {
    let result = await fetch(url);
    return await result.json();
}

export async function postJsonToUrl(url: string, data: object) {
    return await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
}
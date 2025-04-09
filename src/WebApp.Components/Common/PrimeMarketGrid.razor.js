import perspective from "https://cdn.jsdelivr.net/npm/@finos/perspective/dist/cdn/perspective.js";

export async function loadJson(schema, data, view, config) {
    const worker = await perspective.worker();
    const table = await worker.table(schema, { index: "instrument" });
    const settings = JSON.parse(config);

    await table.update(data);
    await view.load(table);
    await view.restore(settings);

    console.log("Connecting to the SignalR hub...");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/prime")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };

    connection.onclose(async () => {
        await start();
    });

    start()
        .then(function () {
            getSnapshot(connection, table);
            startStreaming(connection, table);
            console.log("Streaming started...");
        });
}

function getSnapshot(connection, table) {
    connection.invoke("GetAllQuotes").then(function (quotes) {
        for (let i = 0; i < quotes.length; i++) {
            displayQuote(table, quotes[i]);
        }
    });
}

function startStreaming(connection, table) {

    connection.stream("StreamMarketData").subscribe({
        close: false,
        next: (quote) => displayQuote(table, quote),
        error: function (err) {
            logger.log(err);
        }
    });

    connection.stream("StreamSubscribeStatus").subscribe({
        close: false,
        next: (status) => console.log("Subscribe status received: " + status.body.instrument),
        error: function (err) {
            logger.log(err);
        }
    });
}

function displayQuote(table, quote) {
    console.log("Quote received: " + quote.instrument)

    let json = {
        instrument: [quote.instrument],
        dateTime: [quote.dateTime],
        bid: [quote.bid],
        ask: [quote.ask],
        mid: [quote.mid],
        last: [quote.last],
        open: [quote.open],
        high: [quote.high],
        low: [quote.low],
        close: [quote.close],
        netChange: [quote.netChange],
        percentChange: [quote.percentChange],
        openInterest: [quote.openInterest],
        tradesToday: [quote.tradesToday],
        volume: [quote.volume],
        prevVol: [quote.prevVol],
        bidSize: [quote.bidSize],
        askSize: [quote.askSize],
        lastSize: [quote.lastSize],
    };

    table.update(json);
}


export async function dispose() {

    if (view) {
        await view.delete();
    }

    if (table) {
        await table.delete();
    }
}

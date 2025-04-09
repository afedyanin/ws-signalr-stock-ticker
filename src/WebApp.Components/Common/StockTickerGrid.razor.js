import perspective from "https://cdn.jsdelivr.net/npm/@finos/perspective/dist/cdn/perspective.js";

export async function loadJson(schema, data, view, config) {
    const worker = await perspective.worker();
    const table = await worker.table(schema, { index: "symbol" });
    const settings = JSON.parse(config);

    await table.update(data);
    await view.load(table);
    await view.restore(settings);

    console.log("Connecting to the SignalR hub...");

    const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/stocks")
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
          console.log("Streaming started...")
          startStreaming(connection, table);
    });
}

function startStreaming(connection, table) {

  connection.stream("StreamStocks").subscribe({
    close: false,
    next: (stock) => displayStock(table, stock),
    error: function (err) {
      logger.log(err);
    }
  });
}

function displayStock(table, stock) {

  let json = {
    symbol: [stock.symbol],
    dayOpen: [stock.dayOpen],
    dayLow: [stock.dayLow],
    dayHigh: [stock.dayHigh],
    lastChange: [stock.lastChange],
    change: [stock.change],
    percentChange: [stock.percentChange],
    price: [stock.price]
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

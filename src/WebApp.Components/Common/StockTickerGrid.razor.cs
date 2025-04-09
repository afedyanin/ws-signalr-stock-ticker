using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace WebApp.Components.Common;
public partial class StockTickerGrid : IAsyncDisposable
{
    private IJSObjectReference? _jsModule;

    private ElementReference perspectiveViewer;

    [Parameter]
    public string TableName { get; set; } = "Table";

    [Parameter]
    public string Height { get; set; } = "800px";

    [Parameter]
    public string ConfigEndpoint { get; set; } = string.Empty;

    [Parameter]
    public string DataEndpoint { get; set; } = string.Empty;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private HttpClient Http { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var data = await Http.GetFromJsonAsync<Dictionary<string, object>[]>(DataEndpoint);
            var config = await Http.GetStringAsync(ConfigEndpoint);

            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/WebApp.Components/Common/StockTickerGrid.razor.js");
            await _jsModule.InvokeVoidAsync("loadJson", GetJsonSchema(), data, perspectiveViewer, config);
        }
    }

    private Dictionary<string, string> GetJsonSchema()
        => new Dictionary<string, string>
        {
          { "symbol", "string" },
          { "price", "float" },
          { "lastChange", "float" },
          { "change", "float" },
          { "percentChange", "float" },
          { "dayOpen", "float" },
          { "dayLow", "float" },
          { "dayHigh", "float" },
        };

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsModule != null)
            {
                // await _jsModule.InvokeVoidAsync("dispose");
                await _jsModule.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // Client disconnected.
        }
    }
}

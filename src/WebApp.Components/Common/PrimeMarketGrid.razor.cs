using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebApp.Components.Common;

public partial class PrimeMarketGrid : IAsyncDisposable
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
            // var data = await Http.GetFromJsonAsync<Dictionary<string, object>[]>(DataEndpoint);
            var data = Array.Empty<Dictionary<string, object>>();
            var config = await Http.GetStringAsync(ConfigEndpoint);

            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/WebApp.Components/Common/PrimeMarketGrid.razor.js");
            await _jsModule.InvokeVoidAsync("loadJson", GetJsonSchema(), data, perspectiveViewer, config);
        }
    }

    private Dictionary<string, string> GetJsonSchema()
        => new Dictionary<string, string>
        {
          { "instrument", "string" },
          { "dateTime", "datetime" },
          { "bid", "float" },
          { "ask", "float" },
          { "mid", "float" },
          { "last", "float" },
          { "open", "float" },
          { "high", "float" },
          { "low", "float" },
          { "close", "float" },
          { "netChange", "float" },
          { "percentChange", "float" },
          { "openInterest", "float" },
          { "tradesToday", "float" },
          { "volume", "float" },
          { "prevVol", "float" },
          { "bidSize", "float" },
          { "askSize", "float" },
          { "lastSize", "float" },
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

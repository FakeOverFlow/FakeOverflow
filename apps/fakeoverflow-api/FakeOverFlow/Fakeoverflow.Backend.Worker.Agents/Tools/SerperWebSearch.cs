using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fakeoverflow.Backend.Worker.Agents.Tools;

public class SerperWebSearch
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("https://google.serper.dev")
    };
    
    [Description("Search for a query on the web and returns multiple data including attributes, description, and image.")]
    public static Task<string> SearchSerper([Description("The query to be searched online")]string query) => SearcherSerperInternal(query);

    private static async Task<string> SearcherSerperInternal(string query)
    {
        List<SearcherSerperQuery> queryArray =
        [
            new SearcherSerperQuery()
            {
                Query = query
            }
        ];
        var request = new HttpRequestMessage(HttpMethod.Post, "https://google.serper.dev/search");
        request.Headers.Add("X-API-KEY", "448a6baf2d4bb2271e3dcb26534889501abac62d"); //TODO
        var content = new StringContent(JsonSerializer.Serialize(queryArray), null, "application/json");
        request.Content = content;
        var response = await Client.SendAsync(request);
        if(response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
        
        
        return "Failed to search";
    }

    private class SearcherSerperQuery
    {
        [JsonPropertyName("q")]
        public string Query { get; set; } = null!;
    }
}
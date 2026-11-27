using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Forester.Domain.Entities;

namespace Forester.Application.Helper;

public class PredictionClient(HttpClient client)
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = null };

    public async Task<int> PredictAsync(UserInput userInput)
    {
        var response = await client.PostAsJsonAsync("predict/single", userInput, Options);
        int modelPredictionResult = await ParsePredictionResponse(response);
        return modelPredictionResult;
    }

    public async Task<BatchPredictionResponse> PredictBatchAsync(List<UserInput> userInputs)
    {
        var payload = new {instances=userInputs};
        var response = await client.PostAsJsonAsync("predict/batch", payload, Options);
        var result = await response.Content.ReadFromJsonAsync<BatchPredictionResponse>();
        return result;
    }

    private async Task<int> ParsePredictionResponse(HttpResponseMessage response)
    {
        var result = Regex.Match(await response.Content.ReadAsStringAsync(), @"\d+").Value;
        return int.Parse(result);
    }

    public sealed class SinglePredictionResponse
    {
        public int prediction { get; set; }
    }

    public sealed class BatchPredictionResponse
    {
        public List<int> predictions { get; set; } = new();
    }
}

using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YourNamespace.Services;

public interface IAiService
{
    Task<List<string>> GetAvailableModelsAsync();
    Task<string> SelectRandomModelAsync();
    Task<string> GenerateAsync(string prompt, string? model = null);
    Task<IAsyncEnumerable<string>> GenerateStreamAsync(string prompt, string? model = null);
    string? CurrentModel { get; }
}

public class AiService : IAiService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _currentModel;
    private readonly Random _random = new();

    public string? CurrentModel => _currentModel;

    public AiService(string baseUrl = "http://localhost:11434")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromMinutes(5)
        };
    }

    public AiService(HttpClient httpClient, string baseUrl = "http://localhost:11434")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<List<string>> GetAvailableModelsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaModelsResponse>();
            return result?.Models?.Select(m => m.Name).ToList() ?? new List<string>();
        }
        catch (HttpRequestException ex)
        {
            throw new AiServiceException("Impossibile connettersi a Ollama. Assicurati che sia in esecuzione.", ex);
        }
    }

    public async Task<string> SelectRandomModelAsync()
    {
        var models = await GetAvailableModelsAsync();
        
        if (models.Count == 0)
            throw new AiServiceException("Nessun modello disponibile in Ollama. Scarica un modello con 'ollama pull <nome_modello>'");

        _currentModel = models[_random.Next(models.Count)];
        return _currentModel;
    }

    public async Task<string> GenerateAsync(string prompt, string? model = null)
    {
        var modelToUse = model ?? _currentModel ?? await SelectRandomModelAsync();

        var request = new OllamaGenerateRequest
        {
            Model = modelToUse,
            Prompt = prompt,
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();
        return result?.Response ?? string.Empty;
    }

    public async Task<IAsyncEnumerable<string>> GenerateStreamAsync(string prompt, string? model = null)
    {
        var modelToUse = model ?? _currentModel ?? await SelectRandomModelAsync();

        var request = new OllamaGenerateRequest
        {
            Model = modelToUse,
            Prompt = prompt,
            Stream = true
        };

        return StreamResponseAsync(request);
    }

    private async IAsyncEnumerable<string> StreamResponseAsync(OllamaGenerateRequest Request)
    {
        var json = JsonSerializer.Serialize(Request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/generate") { Content = content };
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) continue;

            var chunk = JsonSerializer.Deserialize<OllamaGenerateResponse>(line);
            if (chunk?.Response != null)
                yield return chunk.Response;

            if (chunk?.Done == true)
                break;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// DTO Classes
public class OllamaModelsResponse
{
    [JsonPropertyName("models")]
    public List<OllamaModel>? Models { get; set; }
}

public class OllamaModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("modified_at")]
    public DateTime ModifiedAt { get; set; }
}

public class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("options")]
    public OllamaOptions? Options { get; set; }
}

public class OllamaOptions
{
    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }

    [JsonPropertyName("num_predict")]
    public int? NumPredict { get; set; }
}

public class OllamaGenerateResponse
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("response")]
    public string? Response { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("total_duration")]
    public long? TotalDuration { get; set; }
}

// Custom Exception
public class AiServiceException : Exception
{
    public AiServiceException(string message) : base(message) { }
    public AiServiceException(string message, Exception inner) : base(message, inner) { }
}
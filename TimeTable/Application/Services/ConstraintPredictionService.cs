using System.Text;
using System.Text.Json;
using Domain.Common;
using Domain.Entities;

namespace Application.Services;

public class ConstraintPredictionService
{
    private readonly HttpClient _httpClient;

    public ConstraintPredictionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<ConstraintType>> PredictConstraintAsync(string naturalLanguageInput)
    {
        var url = "http://localhost:5001/predict"; // Flask API endpoint

        var requestBody = new
        {
            text = naturalLanguageInput
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PredictionResponse>(responseContent);

                if (Enum.TryParse(result.ConstraintType, true, out ConstraintType constraint))
                {
                    return Result<ConstraintType>.Success(constraint);
                }

                return Result<ConstraintType>.Failure("Invalid constraint type received.");
            }

            return Result<ConstraintType>.Failure($"API call failed with status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return Result<ConstraintType>.Failure($"An error occurred: {ex.Message}");
        }
    }

    private class PredictionResponse
    {
        public string ConstraintType { get; set; }
    }
}
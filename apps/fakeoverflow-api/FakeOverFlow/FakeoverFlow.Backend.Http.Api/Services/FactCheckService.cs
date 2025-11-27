using System.Diagnostics;
using System.Text.Json;
using FakeoverFlow.Backend.Http.Api.Abstracts.DTO;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;

namespace FakeoverFlow.Backend.Http.Api.Services;

public class FactCheckerService(ILogger<FactCheckerService> logger, IConfiguration configuration)
    : IFactCheckerService
{
    private readonly string _pythonScriptPath = configuration["FactChecker:ScriptPath"] ?? "/app/scripts/fact_checker.py";
    private readonly string _pythonExecutable = configuration["FactChecker:PythonExecutable"] ?? "python3";

    private readonly string _openAiApiKey = configuration["FactChecker:OpenAIApiKey"];
    private readonly string _modelName = configuration["FactChecker:ModelName"] ?? "gpt-4o-mini";
    private readonly string _apiBaseUrl = configuration["FactChecker:ApiBaseUrl"] ?? "http://localhost:1234";
    private readonly string _serpApiKey = configuration["FactChecker:SerperApiKey"];

    public async Task<FactCheckResult> CheckFactAsync(
        string title, 
        string content, 
        string tags = "", 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var arguments =
                $"\"{_pythonScriptPath}\" " +
                $"\"{EscapeArgument(title)}\" " +
                $"\"{EscapeArgument(content)}\" " +
                $"\"{EscapeArgument(tags)}\" " +
                $"\"{EscapeArgument(_openAiApiKey)}\" " +
                $"\"{EscapeArgument(_modelName)}\" " +
                $"\"{EscapeArgument(_apiBaseUrl)}\" " +
                $"\"{EscapeArgument(_serpApiKey)}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = _pythonExecutable,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };

            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(cancellationToken);

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode != 0)
            {
                logger.LogError(
                    "FactChecker Python script failed with exit code {ExitCode}. Error: {Error}",
                    process.ExitCode, error);

                return new FactCheckResult
                {
                    Success = false,
                    Error = $"Fact checking script error: {error}"
                };
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                logger.LogError("FactChecker Python script returned empty output");
                return new FactCheckResult
                {
                    Success = false,
                    Error = "No output from fact checker script"
                };
            }

            // Parse JSON returned by Python
            var result = JsonSerializer.Deserialize<FactCheckResult>(output,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new FactCheckResult
            {
                Success = false,
                Error = "Failed to parse fact checker JSON"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while running fact checker");
            return new FactCheckResult
            {
                Success = false,
                Error = $"Exception: {ex.Message}"
            };
        }
    }

    private static string EscapeArgument(string argument)
    {
        return argument.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}

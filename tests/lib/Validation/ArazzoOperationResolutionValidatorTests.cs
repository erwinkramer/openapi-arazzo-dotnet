using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace BinkyLabs.OpenApi.Arazzo.Tests.Validation;

public class ArazzoOperationResolutionValidatorTests
{
    [Fact]
    public async Task LoadFromUrlAsync_WithLocalSourceDescription_ResolvesValidOperationReferences()
    {
        var result = await LoadSampleAsync("valid.arazzo.yaml");

        Assert.NotNull(result.Document);
        Assert.DoesNotContain(result.Diagnostic?.Errors ?? [], IsOperationResolutionError);
    }

    [Fact]
    public async Task LoadFromUrlAsync_WithLocalSourceDescription_ReportsUnresolvedOperationReferences()
    {
        var result = await LoadSampleAsync("invalid.arazzo.yaml");
        var errors = result.Diagnostic?.Errors ?? [];

        Assert.Contains(errors, error => error.Message.Contains("operationId 'missingOperation' does not resolve", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Message.Contains("operationPath '{$sourceDescriptions.petstore.url}#/paths/~1pets/post' does not resolve", StringComparison.Ordinal));
    }

    [Fact]
    public async Task LoadFromUrlAsync_WithMultipleLoadedSourceDescriptions_ReportsAmbiguousOperationId()
    {
        var result = await LoadSampleAsync("ambiguous.arazzo.yaml");
        var errors = result.Diagnostic?.Errors ?? [];

        Assert.Contains(errors, error => error.Message.Contains("operationId 'listPets' is ambiguous", StringComparison.Ordinal));
        Assert.DoesNotContain(errors, error => error.Message.Contains("operationId '$sourceDescriptions.inventory.listInventory'", StringComparison.Ordinal));
    }

    private static async Task<BinkyLabs.OpenApi.Arazzo.ReadResult> LoadSampleAsync(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var settings = new ArazzoReaderSettings();
        settings.OpenApiSettings.LoadExternalRefs = true;
        settings.OpenApiSettings.RuleSet = ValidationRuleSet.GetDefaultRuleSet();

        return await ArazzoModelFactory.LoadFormUrlAsync(
            Path.Combine(GetSamplesDirectory(), Path.Join("OperationResolution", safeFileName)),
            settings,
            TestContext.Current.CancellationToken);
    }

    private static string GetSamplesDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var samplesDirectory = Path.Combine(directory.FullName, Path.Join("tests", "lib", "Samples"));
            if (Directory.Exists(samplesDirectory))
            {
                return samplesDirectory;
            }

            directory = directory.Parent;
        }

        return Path.Join(AppContext.BaseDirectory, "Samples");
    }

    private static bool IsOperationResolutionError(OpenApiError error)
    {
        return error.Message.Contains("operationId", StringComparison.Ordinal) ||
            error.Message.Contains("operationPath", StringComparison.Ordinal);
    }
}
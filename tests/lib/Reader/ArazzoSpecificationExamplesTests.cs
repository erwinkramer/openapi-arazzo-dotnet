// Licensed under the MIT license.

using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace BinkyLabs.OpenApi.Arazzo.Tests.Reader;

public class ArazzoSpecificationExamplesTests
{
    private static readonly string SampleRootDirectory = Path.Combine(AppContext.BaseDirectory, "Samples");

    [Fact]
    public async Task LoadFormUrlAsync_CopiedArazzoSampleDatasets_ParseWithoutErrors()
    {
        var ct = TestContext.Current.CancellationToken;
        var settings = new ArazzoReaderSettings();
        settings.OpenApiSettings.RuleSet = ValidationRuleSet.GetEmptyRuleSet();

        Assert.True(Directory.Exists(SampleRootDirectory), $"Sample root directory was not found: {SampleRootDirectory}");

        var failures = new List<string>();

        foreach (var samplePath in Directory.EnumerateFiles(SampleRootDirectory, "*.arazzo.yaml", SearchOption.AllDirectories)
                     .Where(static path => new FileInfo(path).Length > 0)
                     .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            var result = await ArazzoModelFactory.LoadFormUrlAsync(samplePath, settings, ct);
            var errors = result.Diagnostic?.Errors ?? [];
            var displayPath = Path.GetRelativePath(SampleRootDirectory, samplePath);

            if (result.Document is null || errors.Count != 0)
            {
                var errorReport = errors.Count == 0
                    ? "no diagnostics were reported"
                    : string.Join(Environment.NewLine, errors.Select(static error => $"{error.Pointer}: {error.Message}"));

                failures.Add(
                    $"""
                    {displayPath}
                    Document parsed: {result.Document is not null}
                    Errors:
                    {errorReport}
                    """);
            }
        }

        Assert.True(failures.Count == 0, $"One or more copied Arazzo sample datasets failed to parse:{Environment.NewLine}{Environment.NewLine}{string.Join($"{Environment.NewLine}{Environment.NewLine}", failures)}");
    }
}
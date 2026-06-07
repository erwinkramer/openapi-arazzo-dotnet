// Licensed under the MIT license.

using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace BinkyLabs.OpenApi.Arazzo.Tests.Reader;

public class ArazzoSpecificationExamplesTests
{
    private static readonly string SampleDirectory = Path.Combine(AppContext.BaseDirectory, "Samples", "ArazzoSpecification", "1.0.0");

    [Fact]
    public async Task LoadFormUrlAsync_CopiedArazzoSpecificationExamples_ParseWithoutErrors()
    {
        var ct = TestContext.Current.CancellationToken;
        var settings = new ArazzoReaderSettings();
        settings.OpenApiSettings.RuleSet = ValidationRuleSet.GetEmptyRuleSet();

        Assert.True(Directory.Exists(SampleDirectory), $"Sample directory was not found: {SampleDirectory}");

        var failures = new List<string>();

        foreach (var samplePath in Directory.EnumerateFiles(SampleDirectory, "*.arazzo.yaml", SearchOption.TopDirectoryOnly).OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            var result = await ArazzoModelFactory.LoadFormUrlAsync(samplePath, settings, ct);
            var errors = result.Diagnostic?.Errors ?? [];

            if (result.Document is null || errors.Count != 0)
            {
                var errorReport = errors.Count == 0
                    ? "no diagnostics were reported"
                    : string.Join(Environment.NewLine, errors.Select(static error => $"{error.Pointer}: {error.Message}"));

                failures.Add(
                    $"""
                    {Path.GetFileName(samplePath)}
                    Document parsed: {result.Document is not null}
                    Errors:
                    {errorReport}
                    """);
            }
        }
        Assert.True(failures.Count == 0, $"One or more copied Arazzo Specification examples failed to parse:{Environment.NewLine}{Environment.NewLine}{string.Join($"{Environment.NewLine}{Environment.NewLine}", failures)}");
    }
}
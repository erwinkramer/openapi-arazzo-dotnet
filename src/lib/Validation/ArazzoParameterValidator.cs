using Microsoft.OpenApi;

namespace BinkyLabs.OpenApi.Arazzo.Validation;

internal static class ArazzoParameterValidator
{
    internal static void ValidateSerializationParameters(
        IEnumerable<IArazzoParameter>? parameters,
        string elementName,
        bool requiresLocation,
        string locationRequirementReason)
    {
        foreach (var error in Validate(parameters, elementName, requiresLocation, locationRequirementReason))
        {
            throw new ArazzoSerializationException(error);
        }
    }

    internal static IEnumerable<string> Validate(
        IEnumerable<IArazzoParameter>? parameters,
        string elementName,
        bool requiresLocation,
        string locationRequirementReason)
    {
        if (parameters is null)
        {
            yield break;
        }

        var parameterKeys = new HashSet<(string Name, ParameterLocation? In)>();
        foreach (var parameter in parameters)
        {
            var parameterName = parameter.Name;
            if (string.IsNullOrEmpty(parameterName))
            {
                continue;
            }

            if (!parameterKeys.Add((parameterName, parameter.In)))
            {
                yield return $"{elementName} contains duplicate parameter '{parameterName}' in '{GetParameterLocationDisplayName(parameter.In)}'.";
            }

            if (requiresLocation && !parameter.In.HasValue)
            {
                yield return $"{elementName} parameter '{parameterName}' must specify 'in' {locationRequirementReason}.";
            }
        }
    }

    private static string GetParameterLocationDisplayName(ParameterLocation? location) =>
        location.HasValue ? location.Value.GetDisplayName() : "<unspecified>";
}
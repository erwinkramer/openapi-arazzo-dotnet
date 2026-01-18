// Licensed under the MIT license.

namespace BinkyLabs.OpenApi.Arazzo.Reader
{
    internal class PatternFieldMap<T> : Dictionary<Func<string, bool>, Action<T, string, ParseNode>>
    {
    }
}
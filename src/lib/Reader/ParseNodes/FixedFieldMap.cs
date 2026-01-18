// Licensed under the MIT license.

namespace BinkyLabs.OpenApi.Arazzo.Reader
{
    internal class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode>>
    {
    }
}
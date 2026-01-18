
// Licensed under the MIT license.

using BinkyLabs.OpenApi.Arazzo.Reader;

namespace BinkyLabs.OpenApi.Arazzo;

/// <summary>
/// Defines an exception indicating OpenAPI Reader encountered an issue while reading.
/// </summary>
[Serializable]
public class ArazzoReaderException : ArazzoException
{
    /// <summary>
    /// Initializes the <see cref="ArazzoReaderException"/> class.
    /// </summary>
    public ArazzoReaderException() { }

    /// <summary>
    /// Initializes the <see cref="ArazzoReaderException"/> class with a custom message.
    /// </summary>
    /// <param name="message">Plain text error message for this exception.</param>
    public ArazzoReaderException(string message) : base(message) { }

    /// <summary>
    /// Initializes the <see cref="ArazzoReaderException"/> class with a custom message.
    /// </summary>
    /// <param name="message">Plain text error message for this exception.</param>
    /// <param name="context">Context of current parsing process.</param>
    public ArazzoReaderException(string message, ParsingContext context) : base(message)
    {
        Pointer = context.GetLocation();
    }

    /// <summary>
    /// Initializes the <see cref="ArazzoReaderException"/> class with a custom message and inner exception.
    /// </summary>
    /// <param name="message">Plain text error message for this exception.</param>
    /// <param name="innerException">Inner exception that caused this exception to be thrown.</param>
    public ArazzoReaderException(string message, Exception innerException) : base(message, innerException) { }
}
namespace System.CommandLine.Wrapper;

/// <summary>
/// These represent known exceptions that have user-friendly messages. These will be logged and the message will be displayed to the user. Any other exception type will be hidden from the non-verbose log.
/// </summary>
public class CLIException : Exception
{
    /// <summary>
    /// Creates a new instance of the CLIException class.
    /// </summary>
    public CLIException()
    {
    }

    /// <summary>
    /// Creates a new instance of the CLIException class with the specified message.
    /// </summary>
    /// <param name="message">The user-friendly message that describes the error.</param>
    public CLIException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new instance of the CLIException class with the specified message and inner exception.
    /// </summary>
    /// <param name="message">The user-friendly message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. This will be logged in the verbose log.</param>
    public CLIException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

using System.CommandLine.Wrapper.Extensions;
using System.CommandLine.Wrapper.Services;
using System.Linq;
using System.Text;

namespace System.CommandLine.Wrapper.Commands;

/// <summary>
/// Inherit from this class to define the arguments for a command. Override the Validate method to add custom validation logic.
/// </summary>
public abstract class CommandArgs
{
    /// <summary>
    /// Determines whether the CLI Logger should output verbose information.
    /// </summary>
    public bool Verbose { get; set; }

    /// <summary>
    /// Override this function to add custom validation logic in your CommandArgs class.
    /// </summary>
    /// <param name="log">The CLI Logger instance.</param>
    public virtual void Validate(CLILogger log)
    { }

    /// <summary>
    /// Logs all properties of the CommandArgs class.
    /// </summary>
    /// <param name="log">The CLI Logger instance.</param>
    public void Log(CLILogger log)
    {
        if (log is null)
        {
            throw new ArgumentNullException(nameof(log));
        }

        log.Verbose = Verbose;

        foreach (var property in GetType().GetProperties())
        {
            var logName = GetLogName(property.Name);

            if (property.PropertyType == typeof(bool))
            {
                if ((bool)property.GetValue(this))
                {
                    log.LogInformation($"{logName}: true");
                }
            }
            else
            {
                var propValue = property.GetValue(this);

                if (propValue.HasValue() && propValue.ToString().HasValue())
                {
                    log.LogInformation($"{logName}: {propValue}");
                }
            }
        }
    }

    private string GetLogName(string propertyName)
    {
        var result = new StringBuilder();

        foreach (var c in propertyName)
        {
            if (char.IsLower(c))
            {
                result.Append(char.ToUpper(c));
            }
            else
            {
                result.Append($" {c}");
            }
        }

        return result.ToString().Trim();
    }

    /// <summary>
    /// Iterates over all the properties of the CommandArgs class and registers any properties with the SecretAttribute to the CLI Logger.
    /// </summary>
    /// <param name="log">The CLILogger instance</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RegisterSecrets(CLILogger log)
    {
        if (log is null)
        {
            throw new ArgumentNullException(nameof(log));
        }

        foreach (var property in GetType().GetProperties()
                                          .Where(p => p.HasCustomAttribute<SecretAttribute>()))
        {
            log.RegisterSecret((string)property.GetValue(this));
        }
    }
}

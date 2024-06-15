using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Xunit;

namespace SimpleCommander.Services.Tests;

public class CLILoggerTests
{
    private string _logOutput;
    private string _verboseLogOutput;
    private string _consoleOutput;
    private string _consoleError;

    private readonly CLILogger _cliLogger;

    public CLILoggerTests()
    {
        _cliLogger = new CLILogger(CaptureLogOutput, CaptureVerboseLogOutput, CaptureConsoleOutput, CaptureConsoleError);
    }

    [Fact]
    public void Secrets_Should_Be_Masked_From_Logs_And_Console()
    {
        var secret = "purplemonkeydishwasher";
        var urlEncodedSecret = Uri.EscapeDataString(secret);

        _cliLogger.RegisterSecret(secret);

        _cliLogger.Verbose = false;
        _cliLogger.LogInformation($"Don't tell anybody that {secret} is my password");
        _cliLogger.LogInformation($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password");
        _cliLogger.LogVerbose($"Don't tell anybody that {secret} is my password");
        _cliLogger.LogVerbose($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password");
        _cliLogger.LogWarning($"Don't tell anybody that {secret} is my password");
        _cliLogger.LogWarning($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password");
        _cliLogger.LogSuccess($"Don't tell anybody that {secret} is my password");
        _cliLogger.LogSuccess($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password");
        _cliLogger.LogError($"Don't tell anybody that {secret} is my password");
        _cliLogger.LogError($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password");
        _cliLogger.LogError(new CLIException($"Don't tell anybody that {secret} is my password"));
        _cliLogger.LogError(new CLIException($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password"));
        _cliLogger.LogError(new InvalidOperationException($"Don't tell anybody that {secret} is my password"));
        _cliLogger.LogError(new InvalidOperationException($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password"));

        _cliLogger.Verbose = true;
        _cliLogger.LogVerbose($"Don't tell anybody that {secret} is my password");
        _cliLogger.LogVerbose($"Don't tell anyone that {urlEncodedSecret} is my URL encoded password");

        _consoleOutput.Should().NotContain(secret);
        _logOutput.Should().NotContain(secret);
        _verboseLogOutput.Should().NotContain(secret);
        _consoleError.Should().NotContain(secret);

        _consoleOutput.Should().NotContain(urlEncodedSecret);
        _logOutput.Should().NotContain(urlEncodedSecret);
        _verboseLogOutput.Should().NotContain(urlEncodedSecret);
        _consoleError.Should().NotContain(urlEncodedSecret);
    }

    [Theory]
    [InlineData("https://files.github.acmeinc.com/foo?token=foobar")]
    [InlineData("HTTPS://FILES.GITHUB.ACMEINC.COM/FOO?TOKEN=FOOBAR")]
    public void Ghes_Archive_Url_Tokens_Should_Be_Replaced_In_Logs_And_Console(string archiveUrl)
    {
        _cliLogger.Verbose = false;
        _cliLogger.LogInformation($"Archive URL: {archiveUrl}");
        _cliLogger.LogVerbose($"Archive URL: {archiveUrl}");
        _cliLogger.LogWarning($"Archive URL: {archiveUrl}");
        _cliLogger.LogSuccess($"Archive URL: {archiveUrl}");
        _cliLogger.LogError($"Archive URL: {archiveUrl}");
        _cliLogger.LogError(new CLIException($"Archive URL: {archiveUrl}"));
        _cliLogger.LogError(new InvalidOperationException($"Archive URL: {archiveUrl}"));

        _cliLogger.Verbose = true;
        _cliLogger.LogVerbose($"Archive URL: {archiveUrl}");

        _consoleOutput.Should().NotContain(archiveUrl);
        _logOutput.Should().NotContain(archiveUrl);
        _verboseLogOutput.Should().NotContain(archiveUrl);
        _consoleError.Should().NotContain(archiveUrl);

        _consoleOutput.ToLower().Should().Contain("?token=***");
    }

    [Theory]
    [InlineData("https://example-s3-bucket-name.s3.amazonaws.com/uuid-uuid-uuid.tar.gz?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AAAAAAAAAAAAAAAAAAAAAAA&X-Amz-Date=20231025T104425Z&X-Amz-Expires=172800&X-Amz-Signature=AAAAAAAAAAAAAAAAAAAAAAA&X-Amz-SignedHeaders=host&actor_id=1&key_id=0&repo_id=0&response-content-disposition=filename%3Duuid-uuid-uuid.tar.gz&response-content-type=application%2Fx-gzip")]
    [InlineData("HTTPS://EXAMPLE-S3-BUCKET-NAME.S3.AMAZONAWS.COM/UUID-UUID-UUID.TAR.GZ?X-AMZ-ALGORITHM=AWS4-HMAC-SHA256&X-AMZ-CREDENTIAL=AAAAAAAAAAAAAAAAAAAAAAA&X-AMZ-DATE=20231025T104425Z&X-AMZ-EXPIRES=172800&X-AMZ-SIGNATURE=AAAAAAAAAAAAAAAAAAAAAAA&X-AMZ-SIGNEDHEADERS=HOST&ACTOR_ID=1&KEY_ID=0&REPO_ID=0&RESPONSE-CONTENT-DISPOSITION=FILENAME%3DUUID-UUID-UUID.TAR.GZ&RESPONSE-CONTENT-TYPE=APPLICATION%2FX-GZIP")]
    public void Aws_Url_X_Aws_Credential_Parameters_Should_Be_Replaced_In_Logs_And_Console(string awsUrl)
    {
        _cliLogger.Verbose = false;
        _cliLogger.LogInformation($"Archive (metadata) download url: {awsUrl}");
        _cliLogger.LogVerbose($"Archive (metadata) download url: {awsUrl}");
        _cliLogger.LogWarning($"Archive (metadata) download url: {awsUrl}");
        _cliLogger.LogSuccess($"Archive (metadata) download url: {awsUrl}");
        _cliLogger.LogError($"Archive (metadata) download url: {awsUrl}");
        _cliLogger.LogError(new CLIException($"Archive (metadata) download url: {awsUrl}"));
        _cliLogger.LogError(new InvalidOperationException($"Archive (metadata) download url: {awsUrl}"));

        _cliLogger.Verbose = true;
        _cliLogger.LogVerbose($"Archive (metadata) download url: {awsUrl}");

        _consoleOutput.Should().NotContain(awsUrl);
        _logOutput.Should().NotContain(awsUrl);
        _verboseLogOutput.Should().NotContain(awsUrl);
        _consoleError.Should().NotContain(awsUrl);

        _consoleOutput.ToLower().Should().Contain("&x-amz-credential=***");
    }

    [Fact]
    public void LogError_For_CLIException_Should_Log_Exception_Message_In_Non_Verbose_Mode()
    {
        // Arrange
        const string userFriendlyMessage = "A user friendly message";
        const string exceptionDetails = "exception details";
        var cliException = new CLIException(userFriendlyMessage,
            new ArgumentNullException("arg", exceptionDetails));

        // Act
        _cliLogger.LogError(cliException);

        // Assert
        _consoleOutput.Should().BeNull();

        _consoleError.Trim().Should().EndWith($"[ERROR] {userFriendlyMessage}");
        _consoleError.Should().NotContain(exceptionDetails);

        _logOutput.Trim().Should().EndWith($"[ERROR] {userFriendlyMessage}");
        _logOutput.Should().NotContain(exceptionDetails);

        _verboseLogOutput.Trim().Should().EndWith($"[ERROR] {cliException}");
    }

    [Fact]
    public void LogError_For_Unexpected_Exception_Should_Log_Generic_Error_Message_In_Non_Verbose_Mode()
    {
        // Arrange
        const string genericErrorMessage = "An unexpected error happened. Please see the logs for details.";
        const string userEnemyMessage = "Some user enemy error message!";
        const string exceptionDetails = "exception details";
        var unexpectedException = new InvalidOperationException(userEnemyMessage,
            new ArgumentNullException("arg", exceptionDetails));

        // Act
        _cliLogger.LogError(unexpectedException);

        // Assert
        _consoleOutput.Should().BeNull();

        _consoleError.Trim().Should().EndWith($"[ERROR] {genericErrorMessage}");
        _consoleError.Should().NotContain(userEnemyMessage);
        _consoleError.Should().NotContain(exceptionDetails);

        _logOutput.Trim().Should().EndWith($"[ERROR] {genericErrorMessage}");
        _logOutput.Should().NotContain(userEnemyMessage);
        _logOutput.Should().NotContain(exceptionDetails);

        _verboseLogOutput.Trim().Should().EndWith($"[ERROR] {unexpectedException}");
        _verboseLogOutput.Should().NotContain(genericErrorMessage);
    }

    [Fact]
    public void LogError_For_Any_Exception_Should_Always_Log_Entire_Exception_In_Verbose_Mode()
    {
        // Arrange
        const string genericErrorMessage = "An unexpected error happened. Please see the logs for details.";

        _cliLogger.Verbose = true;

        var unexpectedException =
            new InvalidOperationException("Some user enemy error message!", new ArgumentNullException("arg"));

        // Act
        _cliLogger.LogError(unexpectedException);

        // Assert
        _consoleOutput.Should().BeNull();

        _consoleError.Trim().Should().EndWith($"[ERROR] {unexpectedException}");
        _consoleError.Should().NotContain(genericErrorMessage);

        _logOutput.Trim().Should().EndWith($"[ERROR] {unexpectedException}");
        _logOutput.Should().NotContain(genericErrorMessage);

        _verboseLogOutput.Trim().Should().EndWith($"[ERROR] {unexpectedException}");
        _verboseLogOutput.Should().NotContain(genericErrorMessage);
    }

    [Fact]
    public void LogError_With_Message_Should_Write_To_Console_Error()
    {
        // Act
        _cliLogger.LogError("message");

        // Assert
        _consoleOutput.Should().BeNull();
        _consoleError.Should().NotBeNull();
    }

    [Fact]
    public void LogError_With_Exception_Should_Write_To_Console_Error()
    {
        // Act
        _cliLogger.LogError(new ArgumentNullException("arg"));

        // Assert
        _consoleOutput.Should().BeNull();
        _consoleError.Should().NotBeNull();
    }

    [Fact]
    public void LogInformation_Should_Write_To_Console_Out()
    {
        // Act
        _cliLogger.LogInformation("message");

        // Assert
        _consoleOutput.Should().NotBeNull();
        _consoleError.Should().BeNull();
    }

    [Fact]
    public void LogWarning_Should_Write_To_Console_Out()
    {
        // Act
        _cliLogger.LogWarning("message");

        // Assert
        _consoleOutput.Should().NotBeNull();
        _consoleError.Should().BeNull();
    }

    [Fact]
    public void LogVerbose_Should_Write_To_Console_Out_In_Verbose_Mode()
    {
        // Arrange
        _cliLogger.Verbose = true;

        // Act
        _cliLogger.LogVerbose("message");

        // Assert
        _consoleOutput.Should().NotBeNull();
        _consoleError.Should().BeNull();
    }

    [Fact]
    public void Verbose_Log_Should_Capture_Http_Status_Code()
    {
        // Arrange
        _cliLogger.Verbose = true;
        var ex = new HttpRequestException(null, null, HttpStatusCode.BadGateway); // HTTP 502

        // Act
        _cliLogger.LogError(ex);

        // Assert
        _verboseLogOutput.Trim().Should().Contain("502");
    }

    private void CaptureLogOutput(string msg) => _logOutput += msg;

    private void CaptureVerboseLogOutput(string msg) => _verboseLogOutput += msg;

    private void CaptureConsoleOutput(string msg) => _consoleOutput += msg;

    private void CaptureConsoleError(string msg) => _consoleError += msg;
}

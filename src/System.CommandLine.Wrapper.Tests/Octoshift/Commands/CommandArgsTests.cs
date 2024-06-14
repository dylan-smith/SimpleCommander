using System.CommandLine.Wrapper.Services;
using System.CommandLine.Wrapper.Tests;
using FluentAssertions;
using Moq;
using Xunit;

namespace System.CommandLine.Wrapper.Commands.Tests;

public class CommandArgsTests
{
    private class TestCommandArgs : CommandArgs
    {
        public string MultiWordArg { get; set; }
        [Secret]
        public string ThisIsASecret { get; set; }
        public string EmptyArg { get; set; }
        public bool BooleanTrue { get; set; }
        public bool BooleanFalse { get; set; }
    }

    private readonly TestCommandArgs _args = new();

    private string _logOutput = string.Empty;
    private int _logCallCount;
    private const string _secretValue = "password";
    private const string _multiWordArgValue = "foo";
    private readonly Mock<CLILogger> _mockCLILogger = TestHelpers.CreateMock<CLILogger>();

    public CommandArgsTests()
    {
        _args.MultiWordArg = _multiWordArgValue;
        _args.ThisIsASecret = _secretValue;
        _args.BooleanTrue = true;
        _args.BooleanFalse = false;

        _mockCLILogger.Setup(m => m.LogInformation(It.IsAny<string>())).Callback<string>(s =>
        {
            _logOutput += $"{s}\n";
            _logCallCount++;
        });
    }

    [Fact]
    public void Logs_Each_Property_Separately()
    {
        _args.Log(_mockCLILogger.Object);

        _logCallCount.Should().Be(3);
    }

    [Fact]
    public void Logs_Pascal_Case_To_Multiple_Words()
    {
        _args.Log(_mockCLILogger.Object);

        _logOutput.Should().Contain($"MULTI WORD ARG: {_multiWordArgValue}");
    }

    [Fact]
    public void Does_Not_Log_Empty_Args()
    {
        _args.Log(_mockCLILogger.Object);

        _logOutput.Should().NotContain("EMPTY ARG");
    }

    [Fact]
    public void Logs_Flags_That_Are_Set()
    {
        _args.Log(_mockCLILogger.Object);

        _logOutput.Should().Contain("BOOLEAN TRUE: true");
    }

    [Fact]
    public void Does_Not_Log_Flags_That_Are_Not_Set()
    {
        _args.Log(_mockCLILogger.Object);

        _logOutput.Should().NotContain("BOOLEAN FALSE");
    }

    [Fact]
    public void Registers_Secrets()
    {
        _args.RegisterSecrets(_mockCLILogger.Object);

        _mockCLILogger.Verify(x => x.RegisterSecret(_secretValue));
    }

    [Fact]
    public void Sets_Verbose_When_True()
    {
        _args.Verbose = true;
        _args.Log(_mockCLILogger.Object);

        _mockCLILogger.VerifySet(x => x.Verbose = true);
    }

    [Fact]
    public void Sets_Verbose_When_False()
    {
        _args.Verbose = false;
        _args.Log(_mockCLILogger.Object);

        _mockCLILogger.VerifySet(x => x.Verbose = false);
    }
}

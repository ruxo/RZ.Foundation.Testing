using Microsoft.Extensions.Logging;

namespace RZ.Foundation.Testing;

public class TestLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        output.WriteLine(exception is null
                             ? $"[{logLevel} {eventId}] {formatter(state, exception)}"
                             : $"[{logLevel} {eventId}] {formatter(state, exception)}\n{exception}");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) where TState: notnull => throw new NotImplementedException();
}
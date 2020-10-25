namespace Spritely.Features.Test
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;

    public static class LoggerExtensions
    {
        public static void LogInformation<T>(this MockLogger<T> logger, Func<string, bool> predicate)
            => logger.Log(LogLevel.Information, Arg.Is<EventId>(0), Arg.Is<object>(o => predicate(o.ToString())), null);
    }

    public static class Logger
    {
        public static MockLogger<T> For<T>() => Substitute.For<MockLogger<T>>();
    }
    
    // Derived from solution at: https://github.com/nsubstitute/NSubstitute/issues/597
    public abstract class MockLogger<T> : ILogger<T>
    {
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => Log(logLevel, eventId, state, exception);

        public abstract void Log(LogLevel loglevel, EventId eventId, object state, Exception exception);
        public abstract void Log(LogLevel logLevel, string message);
        public virtual bool IsEnabled(LogLevel logLevel) => true;
        public abstract IDisposable BeginScope<TState>(TState state);
    }
}

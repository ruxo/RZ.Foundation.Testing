using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace RZ.Foundation.Testing;

[PublicAPI]
public static partial class DebugTo
{
    public class SimpleTrace(Action<string> writeLine) : TraceListener
    {
        StringBuilder messageComposer = new();

        public override void Write(string? message) {
            messageComposer.Append(message ?? string.Empty);
        }

        public override void WriteLine(string? message) {
            var sb = Interlocked.Exchange(ref messageComposer, new());
            if (sb.Length > 0)
                writeLine(sb.ToString());

            writeLine(message ?? string.Empty);
        }
    }

    sealed class Disposable<T>(T obj, Action<T> disposer) : IDisposable
    {
        public void Dispose() => disposer(obj);
    }
}
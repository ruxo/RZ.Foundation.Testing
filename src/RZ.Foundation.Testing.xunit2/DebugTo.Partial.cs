using System.Diagnostics;

namespace RZ.Foundation.Testing;

public static partial class DebugTo
{
    /// <summary>
    /// A utility function to redirect <see cref="Trace"/> output to xUnit test output.
    /// </summary>
    /// <returns></returns>
    public static IDisposable XUnit(ITestOutputHelper output) {
        var listener = new SimpleTrace(output.WriteLine);
        Trace.Listeners.Add(listener);
        return new Disposable<SimpleTrace>(listener, l => Trace.Listeners.Remove(l));
    }
}
using System.Diagnostics;
using Xunit;

namespace RZ.Foundation.Testing;

public static partial class DebugTo
{
    /// <summary>
    /// A utility function to redirect <see cref="Trace"/> output to xUnit test output.
    /// </summary>
    /// <returns></returns>
    public static IDisposable XUnit() {
        var listener = new SimpleTrace(TestContext.Current.TestOutputHelper!.WriteLine);
        Trace.Listeners.Add(listener);
        return new Disposable<SimpleTrace>(listener, l => Trace.Listeners.Remove(l));
    }
}
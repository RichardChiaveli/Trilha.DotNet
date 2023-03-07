namespace Trilha.DotNet.Shared.Components;

public class TimeElapsedComponent
{
    private Stopwatch _stopwatch = null!;
    private readonly Dictionary<string, long> _log = new();

    public TimeElapsedComponent Start()
    {
        _stopwatch = Stopwatch.StartNew();
        return this;
    }

    public TimeElapsedComponent End(string method)
    {
        _stopwatch.Stop();
        _log.Add(method, _stopwatch.ElapsedMilliseconds);

        return this;
    }

    public override string ToString() => string.Join(",", _log.Select(i => $"{i.Key}: {i.Value} em ms"));
}

using System.Collections.Concurrent;

namespace Tuvi.Api.Push;

public record PushRecord(string DeviceToken, string Title, string Body, DateTimeOffset At);

/// <summary>Lưu vòng (ring buffer) các push gần đây trong bộ nhớ để xem lại khi test local.</summary>
public class PushLog
{
    private const int MaxItems = 200;
    private readonly ConcurrentQueue<PushRecord> _items = new();

    public void Add(PushRecord r)
    {
        _items.Enqueue(r);
        while (_items.Count > MaxItems && _items.TryDequeue(out _)) { }
    }

    public IReadOnlyList<PushRecord> Recent() => _items.Reverse().ToArray();
}

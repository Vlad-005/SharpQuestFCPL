namespace Capstone.Core.Models;

public sealed class RecentlyUsed<T>
{
    private readonly List<T> _values = [];

    public RecentlyUsed(int capacity)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        Capacity = capacity;
    }

    public int Capacity { get; }

    public IReadOnlyList<T> Values => _values.ToArray();

    public void Push(T value)
    {
        var existingIndex = _values.FindIndex(existing => EqualityComparer<T>.Default.Equals(existing, value));
        if (existingIndex >= 0)
            _values.RemoveAt(existingIndex);

        _values.Insert(0, value);
        if (_values.Count > Capacity)
            _values.RemoveAt(_values.Count - 1);
    }
}
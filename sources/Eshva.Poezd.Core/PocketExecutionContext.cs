using System;
using System.Collections.Concurrent;


namespace Eshva.Poezd.Core
{
  public class PocketExecutionContext : IPocket
  {
    public bool TryGet<TValue>(out TValue value) => TryGet(typeof(TValue).FullName, out value);

    /// <exception cref="System.InvalidCastException">
    /// ��������� �������� ����� ���, �������� �� ����������.
    /// </exception>
    public bool TryGet<TValue>(string key, out TValue value)
    {
      if (_things.TryGetValue(key, out var found))
      {
        value = (TValue)found;
        return true;
      }

      value = default;
      return false;
    }

    /// <exception cref="ArgumentNullException">
    /// �������� �� ������.
    /// </exception>
    public void Set<TValue>(TValue value) => Set(typeof(TValue).FullName, value);

    /// <exception cref="ArgumentNullException">
    /// �� ����� ���� ��� ��������, ���� ���� ������������ ����� ������ ������ ��� ������ ���������� �������.
    /// </exception>
    public void Set<TValue>(string key, TValue value)
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new ArgumentNullException(nameof(key));
      }

      if (Equals(value, default(TValue)))
      {
        throw new ArgumentNullException(nameof(value));
      }

      _things[key] = value;
    }

    public bool TryRemove<TValue>() => TryRemove(typeof(TValue).FullName);

    /// <exception cref="ArgumentNullException">
    /// ���� ������������ ����� ������ ������ ��� ������ ���������� �������.
    /// </exception>
    public bool TryRemove(string key)
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new ArgumentNullException(nameof(key));
      }

      return _things.TryRemove(key, out _);
    }

    private readonly ConcurrentDictionary<string, object> _things = new ConcurrentDictionary<string, object>();
  }
}
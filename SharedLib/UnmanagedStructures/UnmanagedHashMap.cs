namespace FE3.VoxelRenderer.Utils.UnmanagedStructures;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public unsafe sealed class UnmanagedHashMap<TKey, TValue> : IDisposable
    where TKey : unmanaged
    where TValue : unmanaged
{
    private Entry* _entries;
    private int _capacity;
    private int _mask;
    private int Count { get; set; }

    private const byte Empty = 0;
    private const byte Occupied = 1;
    private const byte Tombstone = 2;

    private struct Entry
    {
        public TKey Key;
        public TValue Value;
        public byte State;
    }

    public UnmanagedHashMap(int capacity = 16)
    {
        if (capacity < 4) capacity = 4;
        capacity = NextPow2(capacity);

        _capacity = capacity;
        _mask = capacity - 1;

        _entries = (Entry*)Marshal.AllocHGlobal(sizeof(Entry) * _capacity);
        Unsafe.InitBlockUnaligned(_entries, 0, (uint)(sizeof(Entry) * _capacity));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(in TKey key, out TValue value)
    {
        int idx = Hash(key) & _mask;

        for (int i = 0; i < _capacity; i++)
        {
            ref Entry e = ref _entries[idx];

            if (e.State == Empty)
                break;

            if (e.State == Occupied && KeyEquals(e.Key, key))
            {
                value = e.Value;
                return true;
            }

            idx = (idx + 1) & _mask;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAdd(in TKey key, in TValue value)
    {
        if ((Count + 1) * 2 >= _capacity)
            Resize(_capacity << 1);

        int idx = Hash(key) & _mask;
        int tombstone = -1;

        for (int i = 0; i < _capacity; i++)
        {
            ref Entry e = ref _entries[idx];

            if (e.State == Empty)
            {
                if (tombstone != -1)
                    idx = tombstone;

                _entries[idx].Key = key;
                _entries[idx].Value = value;
                _entries[idx].State = Occupied;
                Count++;
                return true;
            }

            if (e.State == Tombstone && tombstone == -1)
                tombstone = idx;

            else if (e.State == Occupied && KeyEquals(e.Key, key))
                return false;

            idx = (idx + 1) & _mask;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(in TKey key)
    {
        int idx = Hash(key) & _mask;

        for (int i = 0; i < _capacity; i++)
        {
            ref Entry e = ref _entries[idx];

            if (e.State == Empty)
                break;

            if (e.State == Occupied && KeyEquals(e.Key, key))
            {
                e.State = Tombstone;
                Count--;
                return true;
            }

            idx = (idx + 1) & _mask;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Unsafe.InitBlockUnaligned(_entries, 0, (uint)(sizeof(Entry) * _capacity));
        Count = 0;
    }

    private void Resize(int newCap)
    {
        newCap = NextPow2(newCap);

        Entry* old = _entries;
        int oldCap = _capacity;

        _entries = (Entry*)Marshal.AllocHGlobal(sizeof(Entry) * newCap);
        Unsafe.InitBlockUnaligned(_entries, 0, (uint)(sizeof(Entry) * newCap));

        _capacity = newCap;
        _mask = newCap - 1;
        Count = 0;

        for (int i = 0; i < oldCap; i++)
        {
            ref Entry e = ref old[i];
            if (e.State == Occupied)
                TryAdd(e.Key, e.Value);
        }

        Marshal.FreeHGlobal((IntPtr)old);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Hash(in TKey key)
    {
        return key.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool KeyEquals(in TKey a, in TKey b)
    {
        return Unsafe.As<TKey, long>(ref Unsafe.AsRef(in a)) ==
               Unsafe.As<TKey, long>(ref Unsafe.AsRef(in b));
    }

    private static int NextPow2(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        return v + 1;
    }

    public void Dispose()
    {
        if (_entries == null) return;
        Marshal.FreeHGlobal((IntPtr)_entries);
        _entries = null;
        _capacity = _mask = Count = 0;
    }
}

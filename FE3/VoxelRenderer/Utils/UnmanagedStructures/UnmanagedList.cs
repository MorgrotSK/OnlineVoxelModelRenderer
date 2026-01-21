namespace FE3.VoxelRenderer.Utils.UnmanagedStructures;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public unsafe sealed class UnmanagedList<T> : IDisposable where T : unmanaged
{
    private T* _buf; 
    private int _cap;
    public int Count { get; private set; }

    public UnmanagedList(int cap = 4)
    {
        _cap = cap > 0 ? cap : 1;
        _buf = (T*)Marshal.AllocHGlobal(sizeof(T) * _cap);
    }
    
    public ref T this[int i]
    {
        get
        {
            #if DEBUG
            if ((uint)i >= (uint)Count) throw new IndexOutOfRangeException();
            #endif
            return ref _buf[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(in T value)
    {
        if (Count == _cap) Grow(_cap << 1);
        _buf[Count++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AddUninitialized()
    {
        if (Count == _cap) Grow(_cap << 1);
        return Count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Count = 0;

    private void Grow(int newCap)
    {
        T* nb = (T*)Marshal.AllocHGlobal(sizeof(T) * newCap);
        Buffer.MemoryCopy(_buf, nb, sizeof(T) * newCap, sizeof(T) * Count);
        Marshal.FreeHGlobal((IntPtr)_buf);
        _buf = nb; _cap = newCap;
    }

    public void Dispose()
    {
        if (_buf == null) return;
        Marshal.FreeHGlobal((IntPtr)_buf);
        _buf = null; _cap = Count = 0;
    }
}

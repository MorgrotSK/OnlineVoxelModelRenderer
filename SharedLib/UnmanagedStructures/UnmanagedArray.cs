namespace FE3.VoxelRenderer.Utils.UnmanagedStructures;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public unsafe struct UnmanagedArray<T> : IDisposable where T : unmanaged
{
    private T* _buf; private int _cap, _count;
    public T* RawPtr => _buf;
    public int Count => _count;

    public UnmanagedArray(int cap = 1)
    {
        _cap = cap > 0 ? cap : 1;
        _buf = (T*)Marshal.AllocHGlobal(sizeof(T) * _cap);
        Unsafe.InitBlockUnaligned(_buf, 0, (uint)(sizeof(T) * _cap));
    }
    
    public ref T this[int i]
    {
        get
        {
            #if DEBUG
            if ((uint)i >= (uint)_count) throw new IndexOutOfRangeException();
            #endif
            return ref _buf[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Allocate(int n)
    {
        int s = _count; EnsureCapacity(_count + n); _count += n; return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int req)
    {
        if (req <= _cap) return;
        int nc = Math.Max(_cap << 1, req);
        T* nb = (T*)Marshal.AllocHGlobal(sizeof(T) * nc);
        Buffer.MemoryCopy(_buf, nb, sizeof(T) * nc, sizeof(T) * _count);
        Unsafe.InitBlockUnaligned(nb + _count, 0, (uint)(sizeof(T) * (nc - _count)));
        Marshal.FreeHGlobal((IntPtr)_buf);
        _buf = nb; _cap = nc;
    }

    public void Dispose()
    {
        if (_buf == null) return;
        Marshal.FreeHGlobal((IntPtr)_buf);
        _buf = null; _cap = _count = 0;
    }
}

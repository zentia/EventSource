using System;
using System.Runtime.InteropServices;
using System.Threading;

public unsafe class NativeMemory
{
    public enum NativeMemoryType
    {
        None,
        RawList,
        Count,
    }

    public struct Info
    {
        public int size;
        public int count;
    }

    public const int cInfo = (int) NativeMemoryType.Count;
    public static Info[] typeInfos = new Info[cInfo];
    public static Info totalInfo;

    public static byte* Alloc(int c, NativeMemoryType t_ = NativeMemoryType.None)
    {
        int t = (int) t_;
        int m = c + 8;
        byte* p = (byte*) Marshal.AllocHGlobal(m).ToPointer();
        int* d = (int*) p;
        d[0] = m;
        d[1] = t;

        Interlocked.Add(ref totalInfo.size, m);
        Interlocked.Increment(ref totalInfo.count);
        if (0 <= t && t < cInfo)
        {
            Interlocked.Add(ref typeInfos[t].size, m);
            Interlocked.Increment(ref typeInfos[t].count);
        }
        return p + 8;
    }

    public static byte* AllocZero(int c, NativeMemoryType t = NativeMemoryType.None)
    {
        byte* p = Alloc(c, t);
        FillZero(p, c);
        return p;
    }

    public static void Free(IntPtr p)
    {
        Free((void*)p); 
    }

    public static void Free(void* p)
    {
        int* d = (int*) ((byte*) p - 8);
        int m = d[0];
        int t = d[1];
        Marshal.FreeHGlobal(new IntPtr(d));

        Interlocked.Add(ref totalInfo.size, -m);
        Interlocked.Decrement(ref totalInfo.count);
        if (0 <= t && t < cInfo)
        {
            Interlocked.Add(ref typeInfos[t].size, -m);
            Interlocked.Decrement(ref typeInfos[t].count);
        }
    }

    public static void FillZero(byte* p, int c)
    {
        if (c >= 16)
        {
            int* d = (int*) p;
            int n = c / sizeof(int);
            for (int i = 0; i < n; i++)
            {
                d[i] = 0;
            }
            c %= sizeof(int);
            p = p + n * sizeof(int);
        }
        for (int i = 0; i < c; i++)
        {
            p[i] = 0;
        }
    }

    public static void Copy(byte *src, byte* dest, int c)
    {
        if (c >= 16)
        {
            int* p = (int*)src;
            int* d = (int*) dest;
            int n = c / sizeof(int);
            for (int i = 0; i < n; i++)
            {
                d[i] = p[i];
            }
            c %= sizeof(int);
            src = src + n * sizeof(int);
            dest = dest + n * sizeof(int);
        }
        for (int i = 0; i < c; i++)
        {
            dest[i] = src[i];
        }
    }
}

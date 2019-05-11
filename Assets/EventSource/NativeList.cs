using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

unsafe struct NativeListData
{
    public byte* arrayPtr;
    public int length;
    public int capacity;
    public int structSize;
    public const int SIZE = 20;
}

public unsafe class NativeList<T>:IDisposable where T : struct
{
    private NativeListData* data;

    public int length
    {
        get { return data->length; }
    }

    public int capacity
    {
        get { return data->capacity; }
    }

    public NativeList(int capacity = 10)
    {
        if (capacity < 1) capacity = 1;
        data = (NativeListData*) NativeMemory.Alloc(NativeListData.SIZE, NativeMemory.NativeMemoryType.RawList);
        data->structSize = Marshal.SizeOf<NativeListData>();
        data->capacity = capacity;
        data->length = 0;
        data->arrayPtr = NativeMemory.Alloc(data->structSize * data->capacity, NativeMemory.NativeMemoryType.RawList);
    }

    private void Resize()
    {
        int newCapacity = data->capacity * 2;
        byte* newArr = NativeMemory.Alloc(data->structSize * newCapacity, NativeMemory.NativeMemoryType.RawList);
        NativeMemory.Copy(newArr, data->arrayPtr, data->structSize * data->capacity);
        NativeMemory.Free(data->arrayPtr);
        data->arrayPtr = newArr;
        data->capacity = newCapacity;
    }

    public void Add(ref T value)
    {
        int lastElement = data->length++;
        if (data->length > data->capacity) Resize();
        IntPtr intPrt = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
        Marshal.StructureToPtr(value, intPrt, true);

    }

    public void Dispose()
    {
        
    }
}

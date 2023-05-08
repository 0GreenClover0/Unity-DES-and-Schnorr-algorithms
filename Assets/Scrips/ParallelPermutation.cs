using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct ParallelPermutation : IJobParallelFor
{
    [ReadOnly] public NativeArray<int> permutationTable;
    [ReadOnly] public NativeArray<NativeArray<byte>> blocks;
    [ReadOnly] public int permutationTableBytesLength;

    public NativeArray<NativeArray<byte>> result;

    public void Execute(int i)
    {
        result[i] = DoPermutationAndResizeIfNeeded(permutationTable, blocks[i], permutationTableBytesLength);
    }

    private NativeArray<byte> DoPermutationAndResizeIfNeeded(NativeArray<int> permutationTable, NativeArray<byte> bytes, int permutationTableBytesLength)
    {
        // NativeArray<byte> result = new NativeArray<byte>(permutationTableBytesLength, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        byte[] temp = null;
        bytes.CopyTo(temp);
        BitArray test = new BitArray(temp);
        test.SetAll(true);
        byte[] temp2 = new byte[permutationTableBytesLength];
        test.CopyTo(temp2, 0);
        NativeArray<byte> result2 = new NativeArray<byte>(temp2, Allocator.TempJob);
        return result2;
        // for (int i = 0; i < permutationTable.Length; i++)
        // {
        //     int bitIndex = permutationTable[i] - 1;
        //     int byteIndex = bitIndex / 8;
        //     int bitInByteIndex = bitIndex % 8;
        //     byte mask = (byte) (1 << bitInByteIndex);
        //     resultBitArray.Set(i, originalBitArray.Get(permutationTable[i] - 1));
        // }

        // return result;
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// public struct ParallelPermutation : IJobParallelFor
// {
//     [ReadOnly] public NativeArray<int> permutationTable;
//     [ReadOnly] public NativeArray<NativeArray<byte>> blocks;
//     [ReadOnly] public int permutationTableBytesLength;

//     public byte[][] result;

//     public void Execute(int i)
//     {
//         result[i] = DoPermutationAndResizeIfNeeded(permutationTable, blocks[i], permutationTableBytesLength);
//     }

//     private byte[] DoPermutationAndResizeIfNeeded(NativeArray<int> permutationTable, NativeArray<byte> bytes, int permutationTableBytesLength)
//     {
//         byte[] result = new byte[permutationTableBytesLength];
//         BitArray resultBitArray = new BitArray(permutationTable.Length);
//         NativeBitArray originalBitArray = new NativeBitArray(bytes, All);

//         for (int i = 0; i < permutationTable.Length; i++)
//             resultBitArray.Set(i, originalBitArray.Get(permutationTable[i] - 1));

//         resultBitArray.CopyTo(result, 0);

//         return result;
//     }
// }

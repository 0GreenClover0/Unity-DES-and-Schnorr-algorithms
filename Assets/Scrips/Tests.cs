// using System;
// using System.Collections;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.Assertions;

// public class Tests : MonoBehaviour
// {
//     [SerializeField] public bool runTests;
//     [SerializeField] public Encoder encoder;

//     public const string keyText = "133457799BBCDFF1";
//     public const string inputText = "0123456789ABCDEF";
//     public readonly bool[] expectedContentBits = {false, false, false, false, false, false, false, true, false, false, true, false, false, false, true, true, false, true, false, false, false, true, false, true, false, true, true, false, false, true, true, true, true, false, false, false, true, false, false, true, true, false, true, false, true, false, true, true, true, true, false, false, true, true, false, true, true, true, true, false, true, true, true, true};
//     public readonly bool[] expectedKeyBits = {false, false, false, true, false, false, true, true, false, false, true, true, false, true, false, false, false, true, false, true, false, true, true, true, false, true, true, true, true, false, false, true, true, false, false, true, true, false, true, true, true, false, true, true, true, true, false, false, true, true, false, true, true, true, true, true, true, true, true, true, false, false, false, true};

//     public readonly bool[][] expectedComputedKeyBits =
//     {
//         new bool[] {false, false, false, true, true, false, true, true, false, false, false, false, false, false, true, false, true, true, true, false, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, false, false, false, false, false, true, true, true, false, false, true, false},
//         new bool[] {false, true, true, true, true, false, false, true, true, false, true, false, true, true, true, false, true, true, false, true, true, false, false, true, true, true, false, true, true, false, true, true, true, true, false, false, true, false, false, true, true, true, true, false, false, true, false, true},
//         new bool[] {false, true, false, true, false, true, false, true, true, true, true, true, true, true, false, false, true, false, false, false, true, false, true, false, false, true, false, false, false, false, true, false, true, true, false, false, true, true, true, true, true, false, false, true, true, false, false, true},
//         new bool[] {false, true, true, true, false, false, true, false, true, false, true, false, true, true, false, true, true, true, false, true, false, true, true, false, true, true, false, true, true, false, true, true, false, false, true, true, false, true, false, true, false, false, false, true, true, true, false, true},
//         new bool[] {false, true, true, true, true, true, false, false, true, true, true, false, true, true, false, false, false, false, false, false, false, true, true, true, true, true, true, false, true, false, true, true, false, true, false, true, false, false, true, true, true, false, true, false, true, false, false, false},
//         new bool[] {false, true, true, false, false, false, true, true, true, false, true, false, false, true, false, true, false, false, true, true, true, true, true, false, false, true, false, true, false, false, false, false, false, true, true, true, true, false, true, true, false, false, true, false, true, true, true, true},
//         new bool[] {true, true, true, false, true, true, false, false, true, false, false, false, false, true, false, false, true, false, true, true, false, true, true, true, true, true, true, true, false, true, true, false, false, false, false, true, true, false, false, false, true, false, true, true, true, true, false, false},
//         new bool[] {true, true, true, true, false, true, true, true, true, false, false, false, true, false, true, false, false, false, true, true, true, false, true, false, true, true, false, false, false, false, false, true, false, false, true, true, true, false, true, true, true, true, true, true, true, false, true, true},
//         new bool[] {true, true, true, false, false, false, false, false, true, true, false, true, true, false, true, true, true, true, true, false, true, false, true, true, true, true, true, false, true, true, false, true, true, true, true, false, false, true, true, true, true, false, false, false, false, false, false, true},
//         new bool[] {true, false, true, true, false, false, false, true, true, true, true, true, false, false, true, true, false, true, false, false, false, true, true, true, true, false, true, true, true, false, true, false, false, true, false, false, false, true, true, false, false, true, false, false, true, true, true, true},
//         new bool[] {false, false, true, false, false, false, false, true, false, true, false, true, true, true, true, true, true, true, false, true, false, false, true, true, true, true, false, true, true, true, true, false, true, true, false, true, false, false, true, true, true, false, false, false, false, true, true, false},
//         new bool[] {false, true, true, true, false, true, false, true, false, true, true, true, false, false, false, true, true, true, true, true, false, true, false, true, true, false, false, true, false, true, false, false, false, true, true, false, false, true, true, true, true, true, true, false, true, false, false, true},
//         new bool[] {true, false, false, true, false, true, true, true, true, true, false, false, false, true, false, true, true, true, false, true, false, false, false, true, true, true, true, true, true, false, true, false, true, false, true, true, true, false, true, false, false, true, false, false, false, false, false, true},
//         new bool[] {false, true, false, true, true, true, true, true, false, true, false, false, false, false, true, true, true, false, true, true, false, true, true, true, true, true, true, true, false, false, true, false, true, true, true, false, false, true, true, true, false, false, true, true, true, false, true, false},
//         new bool[] {true, false, true, true, true, true, true, true, true, false, false, true, false, false, false, true, true, false, false, false, true, true, false, true, false, false, true, true, true, true, false, true, false, false, true, true, true, true, true, true, false, false, false, false, true, false, true, false},
//         new bool[] {true, true, false, false, true, false, true, true, false, false, true, true, true, true, false, true, true, false, false, false, true, false, true, true, false, false, false, false, true, true, true, false, false, false, false, true, false, true, true, true, true, true, true, true, false, true, false, true}
//     };

//     public readonly bool[] expectedInitialPermutationResult = {true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, true, true, false, false, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, true, false, true, false, true, false, true, false, true, true, true, true, false, false, false, false, true, false, true, false, true, false, true, false};

//     public readonly bool[] expectedInitialLPT = {true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, true, true, false, false, true, true, false, false, true, true, true, true, true, true, true, true};
//     public readonly bool[] expectedInitialRPT = {true, true, true, true, false, false, false, false, true, false, true, false, true, false, true, false, true, true, true, true, false, false, false, false, true, false, true, false, true, false, true, false};

//     public readonly bool[] expectedRPTAfterFirstExpand = {false, true, true, true, true, false, true, false, false, false, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, true, true, true, false, true, false, false, false, false, true, false, true, false, true, false, true, false, true, false, true, false, true};
//     public readonly bool[] expectedRPTAfterFirstXOR = {false, true, true, false, false, false, false, true, false, false, false, true, false, true, true, true, true, false, true, true, true, false, true, false, true, false, false, false, false, true, true, false, false, true, true, false, false, true, false, true, false, false, true, false, false, true, true, true};
//     public readonly bool[] expectedRPTAfterFirstSboxes = {false, true, false, true, true, true, false, false, true, false, false, false, false, false, true, false, true, false, true, true, false, true, false, true, true, false, false, true, false, true, true, true};
//     public readonly bool[] expectedRPTAfterFirstStraightPermutation = {false, false, true, false, false, false, true, true, false, true, false, false, true, false, true, false, true, false, true, false, true, false, false, true, true, false, true, true, true, false, true, true};
//     public readonly bool[] expectedLPTAfter16Rounds = {false, true, false, false, false, false, true, true, false, true, false, false, false, false, true, false, false, false, true, true, false, false, true, false, false, false, true, true, false, true, false, false};
//     public readonly bool[] expectedRPTAfter16Rounds = {false, false, false, false, true, false, true, false, false, true, false, false, true, true, false, false, true, true, false, true, true, false, false, true, true, false, false, true, false, true, false, true};
//     public readonly bool[] expectedLPTAndRPTJoined = {false, false, false, false, true, false, true, false, false, true, false, false, true, true, false, false, true, true, false, true, true, false, false, true, true, false, false, true, false, true, false, true, false, true, false, false, false, false, true, true, false, true, false, false, false, false, true, false, false, false, true, true, false, false, true, false, false, false, true, true, false, true, false, false};
//     public readonly bool[] expectedFullBlocksAfterLastPermutation = {true, false, false, false, false, true, false, true, true, true, true, false, true, false, false, false, false, false, false, true, false, false, true, true, false, true, false, true, false, true, false, false, false, false, false, false, true, true, true, true, false, false, false, false, true, false, true, false, true, false, true, true, false, true, false, false, false, false, false, false, false, true, false, true};

//     public void Start()
//     {
//         if (!runTests)
//             return;

//         encoder.keyInputField.text = keyText;
//         encoder.encodeInputField.text = inputText;
//         encoder.treatAsHexToggle.SetIsOnWithoutNotify(true);

//         byte[] contentBytes = encoder.GetBytesFromString(encoder.encodeInputField.text, asHex: true);

//         TestBitsFromBytes(contentBytes, expectedContentBits);

//         byte[] keyBytes = encoder.GetBytesFromString(encoder.keyInputField.text, asHex: true);

//         TestBitsFromBytes(keyBytes, expectedKeyBits);

//         byte[][] keys = encoder.GetKeys(keyBytes);

//         for (int i = 0; i < 16; i++)
//             TestBitsFromBytes(keys[i], expectedComputedKeyBits[i]);

//         Encoder.FullBlocks fullBlocksStruct = encoder.GetBlocksFromContent(contentBytes, Encoder.Action.Encode);

//         Assert.AreEqual(0, fullBlocksStruct.numberOfBytesAppended);

//         byte[][] fullBlocks = fullBlocksStruct.fullBlocks;

//         Assert.AreEqual(1, fullBlocks.Length);

//         fullBlocks = encoder.DoPermutationOnBlocks(encoder.initialPermutationTable, fullBlocks);

//         TestBitsFromBytes(fullBlocks[0], expectedInitialPermutationResult);

//         byte[][] LPT = encoder.CreateTwoDimByteArray(firstDimLength: fullBlocks.Length, secondDimLength: Encoder.HALF_BLOCK_SIZE);
//         byte[][] RPT = encoder.CreateTwoDimByteArray(firstDimLength: fullBlocks.Length, secondDimLength: Encoder.HALF_BLOCK_SIZE);

//         encoder.FillLeftAndRightBlocks(fullBlocks, LPT, RPT);

//         Assert.AreEqual(1, LPT.Length);
//         Assert.AreEqual(1, RPT.Length);

//         TestBitsFromBytes(LPT[0], expectedInitialLPT);
//         TestBitsFromBytes(RPT[0], expectedInitialRPT);

//         byte[][] RPTClone = RPT.Select(x => x.ToArray()).ToArray();

//         RPTClone = encoder.DoPermutationOnBlocks(encoder.eBoxPermutationTable, RPTClone);

//         TestBitsFromBytes(RPTClone[0], expectedRPTAfterFirstExpand);

//         RPTClone = encoder.XORBytesOfBlocks(keys[0], RPTClone);

//         TestBitsFromBytes(RPTClone[0], expectedRPTAfterFirstXOR);

//         RPTClone = encoder.SBoxOfBlocks(RPTClone);

//         TestBitsFromBytes(RPTClone[0], expectedRPTAfterFirstSboxes);

//         RPTClone = encoder.DoPermutationOnBlocks(encoder.straightPermutationTable, RPTClone);

//         TestBitsFromBytes(RPTClone[0], expectedRPTAfterFirstStraightPermutation);

//         RPTClone = encoder.XORBlocksAgainstBlocks(LPT, RPTClone);

//         LPT = RPT.Select(x => x.ToArray()).ToArray();

//         RPT = RPTClone.Select(x => x.ToArray()).ToArray();

//         for (int i = 1; i < 16; i++) 
//         {
//             RPTClone = RPT.Select(x => x.ToArray()).ToArray();

//             RPTClone = encoder.DoPermutationOnBlocks(encoder.eBoxPermutationTable, RPTClone);

//             RPTClone = encoder.XORBytesOfBlocks(keys[i], RPTClone);

//             RPTClone = encoder.SBoxOfBlocks(RPTClone);

//             RPTClone = encoder.DoPermutationOnBlocks(encoder.straightPermutationTable, RPTClone);

//             RPTClone = encoder.XORBlocksAgainstBlocks(LPT, RPTClone);

//             LPT = RPT.Select(x => x.ToArray()).ToArray();

//             RPT = RPTClone.Select(x => x.ToArray()).ToArray();
//         }

//         TestBitsFromBytes(LPT[0], expectedLPTAfter16Rounds);
//         TestBitsFromBytes(RPT[0], expectedRPTAfter16Rounds);

//         RPT = LPT.Select(x => x.ToArray()).ToArray();
//         LPT = RPTClone.Select(x => x.ToArray()).ToArray();

//         encoder.FillFullBlocksFromLeftAndRight(fullBlocks, LPT, RPT);

//         TestBitsFromBytes(fullBlocks[0], expectedLPTAndRPTJoined);

//         fullBlocks = encoder.DoPermutationOnBlocks(encoder.inverseInitialPermutationTable, fullBlocks);

//         TestBitsFromBytes(fullBlocks[0], expectedFullBlocksAfterLastPermutation);
//     }

//     public void TestBitsFromBytes(byte[] contentBytes, bool[] expectedBits)
//     {
//         BitArray contentBits = new BitArray(contentBytes);

//         for (int i = 0; i < expectedBits.Length; i++)
//             Assert.AreEqual(expectedBits[i], contentBits[i]);
//     }
// }

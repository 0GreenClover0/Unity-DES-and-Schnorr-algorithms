using System;
using System.Collections;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

// TODO: Move whole encoding functionality to a separate class
public class EncrypterDecrypter : MonoBehaviour
{
    public FileManager fileManager;
    public GUI gui;
    public TMP_InputField keyInputField;
    public TMP_InputField encryptInputField;
    public TMP_InputField decryptInputField;
    public Toggle encryptFromFile;
    public Toggle encryptToFile;
    public Toggle decryptToFile;
    public Toggle decryptFromFile;

    public readonly int[] initialPermutationTable =
    {
        58, 50, 42, 34, 26, 18, 10,  2, 60, 52, 44, 36, 28, 20, 12,  4,
        62, 54, 46, 38, 30, 22, 14,  6, 64, 56, 48, 40, 32, 24, 16,  8,
        57, 49, 41, 33, 25, 17,  9,  1, 59, 51, 43, 35, 27, 19, 11,  3,
        61, 53, 45, 37, 29, 21, 13,  5, 63, 55, 47, 39, 31, 23, 15,  7
    };

    public readonly int[] inverseInitialPermutationTable =
    {
        40,  8, 48, 16, 56, 24, 64, 32, 39,  7, 47, 15, 55, 23, 63, 31,
        38,  6, 46, 14, 54, 22, 62, 30, 37,  5, 45, 13, 53, 21, 61, 29,
        36,  4, 44, 12, 52, 20, 60, 28, 35,  3, 43, 11, 51, 19, 59, 27,
        34,  2, 42, 10, 50, 18, 58, 26, 33,  1, 41,  9, 49, 17, 57, 25
    };

    public readonly int[] eBoxPermutationTable =
    {
        32,  1,  2,  3,  4,  5,  4,  5,  6,  7,  8,  9,
         8,  9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17,
        16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25,
        24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32,  1
    };

    private readonly int[] permutedChoiceTable1 =
    {
        57, 49, 41, 33, 25, 17,  9,  1, 58, 50, 42, 34, 26, 18,
        10,  2, 59, 51, 43, 35, 27, 19, 11,  3, 60, 52, 44, 36,
        63, 55, 47, 39, 31, 23, 15,  7, 62, 54, 46, 38, 30, 22,
        14,  6, 61, 53, 45, 37, 29, 21, 13,  5, 28, 20, 12,  4
    };

    private readonly int[] permutedChoiceTable2 =
    {
        14, 17, 11, 24,  1,  5,  3, 28, 15,  6, 21, 10,
        23, 19, 12,  4, 26,  8, 16,  7, 27, 20, 13,  2,
        41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
        44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
    };

    private readonly int[] leftShiftsTable =
    {
        1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
    };

    public readonly int[] straightPermutationTable =
    {
        16,  7, 20, 21, 29, 12, 28, 17,  1, 15, 23,
        26,  5, 18, 31, 10,  2,  8, 24, 14, 32, 27,
         3,  9, 19, 13, 30,  6, 22, 11,  4, 25
    };

    private readonly int[][][] sBoxes = new int[][][]
    {
        new int[][]
        {
            new int[] { 14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7 },
            new int[] {  0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8 },
            new int[] {  4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0 },
            new int[] { 15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13 }
        },
        new int[][]
        {
            new int[] { 15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10 },
            new int[] {  3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5 },
            new int[] {  0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15 },
            new int[] { 13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9 }
        },
        new int[][]
        {
            new int[] { 10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8 },
            new int[] { 13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1 },
            new int[] { 13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7 },
            new int[] {  1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12 }
        },
        new int[][]
        {
            new int[] {  7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15 },
            new int[] { 13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9 },
            new int[] { 10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4 },
            new int[] {  3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14 }
        },
        new int[][]
        {
            new int[] {  2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9 },
            new int[] { 14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6 },
            new int[] {  4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14 },
            new int[] { 11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3 }
        },
        new int[][]
        {
            new int[] { 12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11 },
            new int[] { 10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8 },
            new int[] {  9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6 },
            new int[] {  4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13 }
        },
        new int[][]
        {
            new int[] {  4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1 },
            new int[] { 13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6 },
            new int[] {  1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2 },
            new int[] {  6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12 }
        },
        new int[][]
        {
            new int[] { 13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7 },
            new int[] {  1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2 },
            new int[] {  7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8 },
            new int[] {  2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11 }
        }
    };

    public enum Action
    {
        Encrypt,
        Decrypt
    };

    private const int BLOCK_SIZE = 8;
    public const int HALF_BLOCK_SIZE = 4;

    public void OnGenerateKeyClick()
    {
        keyInputField.text = "133457799BBCDFF1";
    }

    public static byte[] GetBytesFromString(string text, bool asHex)
    {
        byte[] result;
        if (asHex)
        {
            result = ParseHexBytes(text);
        }
        else
        {
            result = Encoding.Unicode.GetBytes(text);
            Array.Reverse(result);
        }

        return ReverseBitsInBytes(result);
    }

    public static byte[] ParseHexBytes(string s)
    {
        if (s == null)
            throw new ArgumentNullException("s");

        if (s.Length == 0)
            return new byte[0];

        if (s.Length % 2 != 0)
            throw new ArgumentException("Source length error", "s");

        int length = s.Length >> 1;
        byte[] result = new byte[length];
        for (int i = 0; i < length; i++)
            result[i] = Byte.Parse(s.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

        return result;
    }

    public byte[][] GetBlocksFromContent(byte[] contentBytes, Action action)
    {
        int blockAmount = Mathf.CeilToInt(contentBytes.Length / (float) BLOCK_SIZE);

        // The number of bytes that are missing from the content to complete the entire last block
        byte bytesLacking = (byte) (8 - contentBytes.Length % 8);
        bool needAdditionalBlockForBytesLacking = false;

        if (bytesLacking == 8 && action == Action.Encrypt)
        {
            bytesLacking = 0;
            needAdditionalBlockForBytesLacking = true;
        }

        byte[][] fullBlocks = CreateTwoDimByteArray(firstDimLength: needAdditionalBlockForBytesLacking ? blockAmount + 1 : blockAmount, secondDimLength: BLOCK_SIZE);

        // Copy data from content bytes to bytes in the block array
        int blockIndex = 0;
        int byteIndex = 0;
        for (int i = 0; i < contentBytes.Length; i++)
        {
            byteIndex = i % 8;
            if (i != 0 && byteIndex == 0)
                blockIndex++;

            fullBlocks[blockIndex][byteIndex] = contentBytes[i];
        }

        // Save information about how many bytes were missing to complete the entire last block
        if (action == Action.Encrypt)
        {
            if (needAdditionalBlockForBytesLacking)
            {
                byteIndex = 0;
                blockIndex++;
            }
            else
            {
                byteIndex++;
            }

            fullBlocks[blockIndex][byteIndex] = bytesLacking;

            for (int i = byteIndex + 1; i < 8; i++)
                fullBlocks[blockIndex][i] = 0;
        }

        return fullBlocks;
    }

    public (byte[][], byte[][]) Rounds(byte[][] keys, byte[][] LPT, byte[][] RPT, Action action)
    {
        for (int i = 0; i < 16; i++) 
        {
            byte[][] RPTClone = RPT.Select(x => x.ToArray()).ToArray();

            RPTClone = DoPermutationOnBlocks(eBoxPermutationTable, RPTClone);

            RPTClone = XORBytesOfBlocks(keys[action == Action.Encrypt ? i : 15 - i], RPTClone);

            RPTClone = SBoxOfBlocks(RPTClone);

            RPTClone = DoPermutationOnBlocks(straightPermutationTable, RPTClone);

            RPTClone = XORBlocksAgainstBlocks(LPT, RPTClone);

            LPT = RPT.Select(x => x.ToArray()).ToArray();

            RPT = RPTClone.Select(x => x.ToArray()).ToArray();
        }

        return (LPT, RPT);
    }

    public void OnEncryptClick()
    {
        if (!encryptFromFile.isOn && String.IsNullOrEmpty(encryptInputField.text))
        {
            gui.ShowAlert("Brak danych do zaszyfrowania w polu tekstowym.");
            return;
        }

        if (encryptFromFile.isOn && String.IsNullOrEmpty(fileManager.encryptLoadPathValidated))
        {
            gui.ShowAlert("Opcja 'Wczytywanie danych do zaszyfrowania z pliku' jest wybrana, ale plik źródłowy nie został wybrany.");
            return;
        }

        if (String.IsNullOrEmpty(keyInputField.text))
        {
            gui.ShowAlert("Klucz nie może być pusty.");
            return;
        }

        if (encryptToFile.isOn && String.IsNullOrEmpty(fileManager.encryptSavePathValidated))
        {
            gui.ShowAlert("Opcja 'Zapisywania zaszyfrowanych danych do pliku' jest wybrana, ale plik docelowy nie został wybrany.");
            return;
        }

        byte[] contentBytes = null;
        if (encryptFromFile.isOn)
            contentBytes = File.ReadAllBytes(fileManager.encryptLoadPathValidated);
        else
            contentBytes = GetBytesFromString(encryptInputField.text, asHex: false);

        byte[][] fullBlocks;
        try {
            fullBlocks = EncryptOrDecrypt(contentBytes, keyInputField.text, Action.Encrypt);
        }
        catch {
            return;
        }

        if (encryptToFile.isOn)
        {
            File.WriteAllText(fileManager.encryptSavePathValidated, String.Empty);
            using (var stream = new FileStream(fileManager.encryptSavePathValidated, FileMode.OpenOrCreate))
            {
                for (int i = 0; i < fullBlocks.Length; i++)
                {
                    fullBlocks[i] = ReverseBitsInBytes(fullBlocks[i]);
                    Array.Reverse(fullBlocks[i]);
                    stream.Write(fullBlocks[i], 0, fullBlocks[i].Length);
                }
            }
        }
        else
        {
            string output = "";
            for (int i = 0; i < fullBlocks.Length; i++)
            {
                fullBlocks[i] = ReverseBitsInBytes(fullBlocks[i]);
                Array.Reverse(fullBlocks[i]);
                output += BitConverter.ToInt64(fullBlocks[i]).ToString("x16");
            }

            decryptInputField.text = output;
        }

        gui.ShowAlert("Dane zaszyfrowane", "");
    }

    public void OnDecryptClick()
    {
        if (!decryptFromFile.isOn)
        {
            if (String.IsNullOrEmpty(decryptInputField.text))
            {
                gui.ShowAlert("Brak danych do odszyfrowania w polu tekstowym.");
                return;
            }

            if (decryptInputField.text.Length % 16 != 0)
            {
                gui.ShowAlert("Zawartość do odszyfrowania ma niepoprawną długość.");
                return;
            }
        }

        if (decryptFromFile.isOn && String.IsNullOrEmpty(fileManager.decryptLoadPathValidated))
        {
            gui.ShowAlert("Opcja 'Wczytywanie danych do odszyfrowania z pliku' jest wybrana, ale plik źródłowy nie został wybrany.");
            return;
        }

        if (String.IsNullOrEmpty(keyInputField.text))
        {
            gui.ShowAlert("Klucz nie może być pusty.");
            return;
        }

        if (keyInputField.text.Length != 16)
        {
            gui.ShowAlert("Klucz nie jest odpowiedniej długości.");
            return;
        }

        if (decryptToFile.isOn && String.IsNullOrEmpty(fileManager.decryptSavePathValidated))
        {
            gui.ShowAlert("Opcja 'Zapisywania odszyfrowanych danych do pliku' jest wybrana, ale plik docelowy nie został wybrany.");
            return;
        }

        byte[] contentBytes = null;
        if (decryptFromFile.isOn)
        {
            contentBytes = File.ReadAllBytes(fileManager.decryptLoadPathValidated);
            contentBytes = ReverseBitsInBytes(contentBytes);
            Array.Reverse(contentBytes);
        }
        else
        {
            try {
                contentBytes = GetBytesFromString(decryptInputField.text, asHex: true);
            }
            catch (FormatException) {
                gui.ShowAlert("Zawartość do odszyfrowania nie jest prawidłowo zapisaną liczbą szesnastkową.");
                return;
            }
        }

        byte[][] fullBlocks;
        try {
            fullBlocks = EncryptOrDecrypt(contentBytes, keyInputField.text, Action.Decrypt);
        }
        catch {
            return;
        }

        if (decryptFromFile.isOn)
            Array.Reverse(fullBlocks);

        // Scan last block, from the end to start, for the byte that has any information - this will be the number of bytes that needed to be appended.
        // If the whole last block consists of bytes with a value of 0 -> this block was appended because we didn't need to append any bytes to fill the block
        // and thus there was no space to save the number of lacking bytes (which is equal 0).
        byte bytesLacking = 0;
        for (int i = BLOCK_SIZE - 1; i >= 0; i--)
        {
            if (fullBlocks[fullBlocks.Length - 1][i] != 0)
            {
                bytesLacking = fullBlocks[fullBlocks.Length - 1][i];

                // Zero out the byte with appended information
                fullBlocks[fullBlocks.Length - 1][i] = 0;
                break;
            }
        }

        // Discard the last block if it was added by us
        int numberOfSignificantBlocks = bytesLacking != 0 ? fullBlocks.Length : fullBlocks.Length - 1;
        bool shouldTruncateLastBlock = (bytesLacking != 0);

        if (!decryptToFile.isOn)
        {
            string[] outputs = new string[numberOfSignificantBlocks];
            for (int i = 0; i < numberOfSignificantBlocks; i++)
            {
                fullBlocks[i] = ReverseBitsInBytes(fullBlocks[i]);
                Array.Reverse(fullBlocks[i]);

                if (shouldTruncateLastBlock && i == numberOfSignificantBlocks - 1)
                {
                    int numberOfSignificantBytes = 8 - bytesLacking;
                    byte[] truncatedBlock = new byte[numberOfSignificantBytes];
                    for (int j = 0; j < numberOfSignificantBytes; j++)
                        truncatedBlock[j] = fullBlocks[i][j];

                    outputs[i] = Encoding.Unicode.GetString(truncatedBlock);
                }

                outputs[i] = Encoding.Unicode.GetString(fullBlocks[i]);
            }

            string output = "";
            for (int i = outputs.Length - 1; i >= 0; i--)
                output += outputs[i];

            encryptInputField.text = output;
        }
        else
        {
            File.WriteAllText(fileManager.decryptSavePathValidated, String.Empty);
            using (var stream = new FileStream(fileManager.decryptSavePathValidated, FileMode.Append))
            {
                for (int i = 0; i < numberOfSignificantBlocks; i++)
                {
                    if (shouldTruncateLastBlock && i == numberOfSignificantBlocks - 1)
                    {
                        int numberOfSignificantBytes = 8 - bytesLacking;
                        byte[] truncatedBlock = new byte[numberOfSignificantBytes];
                        for (int j = 0; j < numberOfSignificantBytes; j++)
                            truncatedBlock[j] = fullBlocks[i][j];

                        stream.Write(truncatedBlock, 0, truncatedBlock.Length);
                        break;
                    }

                    stream.Write(fullBlocks[i], 0, fullBlocks[i].Length);
                }
            }
        }

        gui.ShowAlert("Dane odszyfrowane", "");
    }

    private byte[][] EncryptOrDecrypt(byte[] input, string keyInHex, Action action)
    {
        // Get bytes from the key input field. Treat input text as hexadecimal
        byte[] keyBytes;
        try {
            keyBytes = GetBytesFromString(keyInHex, asHex: true);
        }
        catch (FormatException e) {
            gui.ShowAlert("Klucz nie jest prawidłowo zapisaną liczbą szesnastkową.");
            throw e;
        }

        byte[][] keys = GetKeys(keyBytes);

        // Two dimensional array that holds "blocks" of 8 bytes = 64 bits
        byte[][] fullBlocks = GetBlocksFromContent(input, action);

        // Initial permutation
        fullBlocks = DoPermutationOnBlocks(initialPermutationTable, fullBlocks);

        // Two two-dimensional arrays that hold "blocks" of 4 bytes = 32 bits,
        // one for the left-side 4 bytes of the full block, one for the right-side 4 bytes of the full block
        byte[][] LPT = CreateTwoDimByteArray(firstDimLength: fullBlocks.Length, secondDimLength: HALF_BLOCK_SIZE);
        byte[][] RPT = CreateTwoDimByteArray(firstDimLength: fullBlocks.Length, secondDimLength: HALF_BLOCK_SIZE);

        // Copy data from full blocks into two separate arrays that hold half-blocks
        FillLeftAndRightBlocks(fullBlocks, LPT, RPT);

        var temp = Rounds(keys, LPT, RPT, action);

        // Last swap after all rounds
        LPT = temp.Item1;
        RPT = temp.Item2;

        FillFullBlocksFromLeftAndRight(fullBlocks, RPT, LPT);

        // Last permutation
        fullBlocks = DoPermutationOnBlocks(inverseInitialPermutationTable, fullBlocks);

        return fullBlocks;
    }

    public void FillLeftAndRightBlocks(byte[][] fullBlocks, byte[][] LPT, byte[][] RPT)
    {
        int blockIndex = 0;
        int byteIndex = 0;
        int oneSideHalfBlockCount = fullBlocks.Length * HALF_BLOCK_SIZE;
        for (int i = 0; i < oneSideHalfBlockCount; i++)
        {
            byteIndex = i % 4;
            if (i != 0 && byteIndex == 0)
                blockIndex++;

            LPT[blockIndex][byteIndex] = fullBlocks[blockIndex][byteIndex];
            RPT[blockIndex][byteIndex] = fullBlocks[blockIndex][byteIndex + 4];
        }
    }

    public void FillFullBlocksFromLeftAndRight(byte[][] fullBlocks, byte[][] LPT, byte[][] RPT)
    {
        int blockIndex = 0;
        int byteIndex = 0;
        int oneSideHalfBlockCount = fullBlocks.Length * HALF_BLOCK_SIZE;
        for (int i = 0; i < oneSideHalfBlockCount; i++)
        {
            byteIndex = i % 4;
            if (i != 0 && byteIndex == 0)
                blockIndex++;

            fullBlocks[blockIndex][byteIndex] = LPT[blockIndex][byteIndex];
            fullBlocks[blockIndex][byteIndex + 4] = RPT[blockIndex][byteIndex];
        }
    }

    public byte[][] GetKeys(byte[] key)
    {
        byte[][] keys = CreateTwoDimByteArray(firstDimLength: 16, secondDimLength: 6);

        // Shuffle bits of the key, discard every eighth bit of the original key
        int permutationChoice1TableBytesLength = permutedChoiceTable1.Length / 8;
        key = DoPermutationAndResizeIfNeeded(permutedChoiceTable1, key, permutationChoice1TableBytesLength);

        int permutationChoice2TableBytesLength = permutedChoiceTable2.Length / 8;
        BitArray keyBitArray = new BitArray(key);
        BitArray left = GetBitArraySlice(keyBitArray, 0, 28);
        BitArray right = GetBitArraySlice(keyBitArray, 28, 56);
        for (int i = 0; i < 16; i++)
        {
            CircularLeftShift(left, leftShiftsTable[i]);
            CircularLeftShift(right, leftShiftsTable[i]);

            AppendBitArrayToBitArray(left, right).CopyTo(key, 0);

            keys[i] = DoPermutationAndResizeIfNeeded(permutedChoiceTable2, key, permutationChoice2TableBytesLength);
        }

        return keys;
    }

    public byte[][] SBoxOfBlocks(byte[][] blocks)
    {
        byte[][] result = CreateTwoDimByteArray(blocks.Length, 4);

        for (int i = 0; i < blocks.Length; i++)
        {
            BitArray bits = new BitArray(blocks[i]);
            BitArray blockResult = null;

            for (int j = 0; j < 48; j = j + 6)
            {
                BitArray chunk = GetBitArraySlice(bits, j, j + 6);

                int sboxNum = j / 6;

                BitArray rowBitArray = new BitArray(2);
                rowBitArray.Set(0, chunk.Get(0));
                rowBitArray.Set(1, chunk.Get(5));
                ReverseBitArray(rowBitArray);
                int[] row = new int[1];
                rowBitArray.CopyTo(row, 0);

                BitArray columnBitArray = GetBitArraySlice(chunk, 1, 5);
                ReverseBitArray(columnBitArray);
                int[] column = new int[1];
                columnBitArray.CopyTo(column, 0);

                int sBoxValue = sBoxes[sboxNum][row[0]][column[0]];
                BitArray allSBoxValueBits = new BitArray(BitConverter.GetBytes(sBoxValue));
                BitArray meaningfulSBoxValueBits = GetBitArraySlice(allSBoxValueBits, 0, 4);
                ReverseBitArray(meaningfulSBoxValueBits);

                if (blockResult == null)
                    blockResult = meaningfulSBoxValueBits;
                else
                    blockResult = AppendBitArrayToBitArray(blockResult, meaningfulSBoxValueBits);
            }

            blockResult.CopyTo(result[i], 0);
        }

        return result;
    }

    public byte[][] XORBlocksAgainstBlocks(byte[][] left, byte[][] right)
    {
        byte[][] result = CreateTwoDimByteArray(left.Length, left[0].Length);

        for (int i = 0; i < left.Length; i++)
            result[i] = XORBytesAgainstBytes(left[i], right[i]);

        return result;
    }

    public byte[][] XORBytesOfBlocks(byte[] key, byte[][] blocks)
    {
        byte[][] result = CreateTwoDimByteArray(blocks.Length, blocks[0].Length);

        for (int i = 0; i < blocks.Length; i++)
            result[i] = XORBytesAgainstBytes(blocks[i], key);

        return result;
    }

    private byte[] XORBytesAgainstBytes(byte[] left, byte[] right)
    {
        byte[] result = new byte[left.Length];

        for (int i = 0; i < left.Length; i++)
            result[i] = (byte) (left[i] ^ right[i]);

        return result;
    }

    private void CircularLeftShift(BitArray bits, int numberOfShifts)
    {
        bool[] lostBits = new bool[numberOfShifts];
        for (int i = 0; i < numberOfShifts; i++)
            lostBits[i] = bits[i];

        bits.RightShift(numberOfShifts);

        for (int i = numberOfShifts - 1; i >= 0; i--)
            bits.Set(bits.Length - 1 - i, lostBits[lostBits.Length - 1 - i]);
    }

    public byte[][] CreateTwoDimByteArray(int firstDimLength, int secondDimLength)
    {
        byte[][] array = new byte[firstDimLength][];

        for (int i = 0; i < firstDimLength; i++)
            array[i] = new byte[secondDimLength];

        return array;
    }

    /// <summary>
    /// For each block, this will create a corresponding block, with bits rearranged accordingly to the order in the permutation table
    // Ex. for a block [0, 1, 0] and permutation table [3, 3, 2, 1], this will create a block [0, 0, 1, 0]
    /// </summary>
    public byte[][] DoPermutationOnBlocks(int[] permutationTable, byte[][] blocks)
    {
        byte[][] result = new byte[blocks.Length][];

        int permutationTableBytesLength = permutationTable.Length / 8;
        for (int i = 0; i < blocks.Length; i++)
            result[i] = DoPermutationAndResizeIfNeeded(permutationTable, blocks[i], permutationTableBytesLength);

        return result;
    }

    private byte[] DoPermutationAndResizeIfNeeded(int[] permutationTable, byte[] bytes, int permutationTableBytesLength)
    {
        byte[] result = new byte[permutationTableBytesLength];
        BitArray resultBitArray = new BitArray(permutationTable.Length);
        BitArray originalBitArray = new BitArray(bytes);

        for (int i = 0; i < permutationTable.Length; i++)
            resultBitArray.Set(i, originalBitArray.Get(permutationTable[i] - 1));

        resultBitArray.CopyTo(result, 0);

        return result;
    }

    private BitArray AppendBitArrayToBitArray(BitArray current, BitArray append) {
        BitArray result = new BitArray(current.Length + append.Length);

        for (int i = 0; i < result.Length; i++)
        {
            if (i < current.Length)
                result.Set(i, current.Get(i));
            else
                result.Set(i, append.Get(i - current.Length));
        }

        return result;
    }

    public void ReverseBitArray(BitArray array)
    {
        int length = array.Length;
        int mid = (length / 2);

        for (int i = 0; i < mid; i++)
        {
            bool bit = array[i];
            array[i] = array[length - i - 1];
            array[length - i - 1] = bit;
        }    
    }

    public static byte[] ReverseBitsInBytes(byte[] bytes)
    {
        BitArray bits = new BitArray(bytes);

        for (int i = 0; i < bits.Length; i = i + 8)
        {
            bool temp1 = bits[i];
            bool temp2 = bits[i + 1];
            bool temp3 = bits[i + 2];
            bool temp4 = bits[i + 3];

            bits[i] = bits[i + 7];
            bits[i + 1] = bits[i + 6];
            bits[i + 2] = bits[i + 5];
            bits[i + 3] = bits[i + 4];

            bits[i + 7] = temp1;
            bits[i + 6] = temp2;
            bits[i + 5] = temp3;
            bits[i + 4] = temp4;
        }

        byte[] result = new byte[bytes.Length];
        bits.CopyTo(result, 0);
        return result;
    }

    private BitArray GetBitArraySlice(BitArray bitArray, int start, int end)
    {
        BitArray result = new BitArray(end - start);

        int index = 0;
        for (int i = start; i < end; i++)
        {
            result.Set(index, bitArray.Get(i));
            index++;
        }

        return result;
    }
}

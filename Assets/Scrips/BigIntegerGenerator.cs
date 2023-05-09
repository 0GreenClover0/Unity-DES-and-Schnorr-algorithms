using System;
using System.Numerics;
using System.Security.Cryptography;
using UnityEngine.Assertions;

class BigIntegerGenerator
{
    public static BigInteger NextBigInteger(int bitLength, bool even = false)
    {
        if (bitLength < 1) return BigInteger.Zero;

        int bytes = bitLength / 8;
        int bits = bitLength % 8;

        // Generates enough random bytes to cover our bits.
        Random rnd = new Random();
        byte[] bs = new byte[bytes + 1];
        rnd.NextBytes(bs);

        // Mask out the unnecessary bits.
        byte mask = (byte)(0xFF >> (8 - bits));
        bs[bs.Length - 1] &= mask;

        if (even)
            bs[0] = 0;

        return new BigInteger(bs);
    }

    public static BigInteger RandomBigInteger(BigInteger start, BigInteger end)
    {
        if (start == end) return start;

        BigInteger res = end;

        // Swap start and end if given in reverse order.
        if (start > end)
        {
            end = start;
            start = res;
            res = end - start;
        }
        else
            // The distance between start and end to generate a random BigIntger between 0 and (end-start) (non-inclusive).
            res -= start;

        byte[] bs = res.ToByteArray();

        // Count the number of bits necessary for res.
        int bits = 8;
        byte mask = 0x7F;
        while ((bs[bs.Length - 1] & mask) == bs[bs.Length - 1])
        {
            bits--;
            mask >>= 1;
        }
        bits += 8 * bs.Length;

        // Generate a random BigInteger that is the first power of 2 larger than res, 
        // then scale the range down to the size of res,
        // finally add start back on to shift back to the desired range and return.
        return ((NextBigInteger(bits + 1) * res) / BigInteger.Pow(2, bits + 1)) + start;
    }

    /// <summary>
    /// Returns a random BigInteger that is within a specified range.
    /// The lower bound is inclusive, and the upper bound is exclusive.
    /// </summary>
    public static BigInteger NextBigIntegerInRange(Random random, BigInteger minValue, BigInteger maxValue)
    {
        if (minValue > maxValue) throw new ArgumentException();
        if (minValue == maxValue) return minValue;
        BigInteger zeroBasedUpperBound = maxValue - 1 - minValue; // Inclusive
        Assert.IsTrue(zeroBasedUpperBound.Sign >= 0);
        byte[] bytes = zeroBasedUpperBound.ToByteArray();
        Assert.IsTrue(bytes.Length > 0);
        Assert.IsTrue((bytes[bytes.Length - 1] & 0b10000000) == 0);

        // Search for the most significant non-zero bit
        byte lastByteMask = 0b11111111;
        for (byte mask = 0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1)
        {
            if ((bytes[bytes.Length - 1] & mask) == mask)
                break; // We found it
        }

        while (true)
        {
            random.NextBytes(bytes);
            bytes[bytes.Length - 1] &= lastByteMask;
            var result = new BigInteger(bytes);
            Assert.IsTrue(result.Sign >= 0);
            if (result <= zeroBasedUpperBound)
                return result + minValue;
        }
    }
}


// Miller-Rabin primality test as an extension method on the BigInteger type.
class BigIntegerPrimeTest
{
    public static bool IsProbablePrime(BigInteger source, int certainty)
    {
        if (source == 2 || source == 3)
            return true;
        if (source < 2 || source % 2 == 0)
            return false;

        BigInteger d = source - 1;
        int s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }

        // There is no built-in method for generating random BigInteger values.
        // Instead, random BigIntegers are constructed from randomly generated
        // byte arrays of the same length as the source.
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[source.GetByteCount()];
        BigInteger a;

        for (int i = 0; i < certainty; i++)
        {
            do
            {
                rng.GetBytes(bytes);
                a = new BigInteger(bytes);
            }
            while (a < 2 || a >= source - 2);

            BigInteger x = BigInteger.ModPow(a, d, source);
            if (x == 1 || x == source - 1)
                continue;

            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, source);
                if (x == 1)
                    return false;
                if (x == source - 1)
                    break;
            }

            if (x != source - 1)
                return false;
        }

        return true;
    }
}
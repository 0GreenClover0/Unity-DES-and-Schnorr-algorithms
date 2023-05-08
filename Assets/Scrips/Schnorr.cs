using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Schnorr : MonoBehaviour
{
    public GUI gui;
    public TMP_InputField signInputField;
    public TMP_InputField signatureInputField;

    private HashAlgorithm hashAlgorithm = HashAlgorithm.Create();

    private BigInteger a, p, v;

    // TODO: Loading key / content from file
    // TODO: Displaying keys
    // TODO: Figure out whole key buffering

    public void OnSignPressed()
    {
        byte[] contentBytes = null;
        contentBytes = EncrypterDecrypter.GetBytesFromString(signInputField.text, asHex: false);

        // https://mrajacse.files.wordpress.com/2012/01/applied-cryptography-2nd-ed-b-schneier.pdf#%F6F%FD%5C%5CU%EC%E8S%A2ocmd%0C%04%A6Heading4
        // To generate a key pair, first choose two primes, 'p' and 'q', such that 'q' is a prime factor of 'p - 1'

        // Generate 'q'
        BigInteger q;
        bool isPrime = false;
        do
        {
            // Schnorr recommended that 'q' be about 140 bits. Generate a random BigInteger that is ~140 bits long
            q = BigIntegerGenerator.NextBigInteger(140);

            // Check if it is a probable prime
            isPrime = BigIntegerPrimeTest.IsProbablePrime(q, 100);
        } while (!isPrime);

        // Generate p
        BigInteger p1, p2, pMinusOne;
        bool p1Prime = false;
        while (true)
        {
            // Schnorr recommended that 'p' be about 512 bits

            // Generate two random prime numbers that are 512 bits
            // Generating two of them is not in accordance to spec, but it's easier to do it like that
            var temp = GenerateTwoPrimeNumbers(1024);

            // Calculate 'p - 1'
            pMinusOne = temp.Item1 - BigInteger.One;

            // Calculate the remainder of the '(p - 1) / q'
            BigInteger remainder = pMinusOne % q;

            // Subtract the calculated remainder from 'p'
            // Thanks to this, 'p - 1' will for sure be divisible by 'q'. And as 'q' is a prime number,
            // it will be a prime factor of 'p - 1'
            p1 = temp.Item1 - remainder;

            // Check if 'p' is (probably) a prime number
            if (BigIntegerPrimeTest.IsProbablePrime(p1, 2))
            {
                // If it is, mark p1 as the 'p' we are looking for
                p1Prime = true;
                break;
            }

            // If p1 is not meeting the conditions, repeat the procedure for p2

            pMinusOne = temp.Item2 - BigInteger.One;

            remainder = pMinusOne % q;

            p2 = temp.Item2 - remainder;

            if (BigIntegerPrimeTest.IsProbablePrime(p2, 2))
                break;
        }

        p = p1Prime ? p1 : p2;

        // Then, choose an 'a' not equal to 1, thanks to which 'h^k' will be not congruent to '1 mod p'
        // This means that 'h^k mod p' can't result in a value of 1 
        BigInteger k = (p - BigInteger.One) / q;
        do
        {
            BigInteger h = BigIntegerGenerator.NextBigIntegerInRange(new System.Random(), BigInteger.One + BigInteger.One, p);

            a = BigInteger.ModPow(h, k, p);
        } while (a == BigInteger.One);

        // Pick a random number 'r', less than 'q'
        BigInteger r = BigIntegerGenerator.RandomBigInteger(BigInteger.Zero, q);

        // And compute 'x = a^r mod p'
        BigInteger x = BigInteger.ModPow(a, r, p);

        // To generate a particular public-key/private-key key pair
        // choose a random number 's' less than 'q'
        BigInteger s = BigIntegerGenerator.RandomBigInteger(BigInteger.Zero, q);

        // Then calculate 'v = a^(-s) mod p'. This is the public key.
        v = BigInteger.ModPow(a, s, p);
        v = ModInverse(v, p); // NOTE: BigInteger.ModPow doesn't take negative exponents as an argument, so we need to ModInverse() the result

        // Concatenate the message 'M' and 'x'
        byte[] bytes = contentBytes.Concat(x.ToByteArray()).ToArray();

        // Hash the result
        byte[] hash = hashAlgorithm.ComputeHash(bytes); // We are using SHA256 by default here 

        // NOTE: Taking it in Abs is not in accordance to spec
        BigInteger e = BigInteger.Abs(new BigInteger(hash));

        // Compute 'y = (r + s*e) mod q'
        BigInteger y = (r + (s * e)) % q;

        // The signature is 'e' and 'y'
        (BigInteger, BigInteger) signature = (e, y);

        string signature1 = BytesToString(signature.Item1.ToByteArray());
        string signature2 = BytesToString(signature.Item2.ToByteArray());
        string finalSignature = signature1 + "\n" + signature2;
        signatureInputField.SetTextWithoutNotify(finalSignature);
    }

    public void OnVerifyPressed()
    {
        byte[] contentBytes = null;
        contentBytes = EncrypterDecrypter.GetBytesFromString(signInputField.text, asHex: false);

        string signatureText = signatureInputField.text;
        string[] signatureParts = signatureText.Split(Environment.NewLine.ToCharArray());
        Assert.IsTrue(signatureParts.Length == 2);
        BigInteger signatureBack1 = new BigInteger(StringToBytes(signatureParts[0]));
        BigInteger signatureBack2 = new BigInteger(StringToBytes(signatureParts[1]));

        // Compute 'x = (a^v)*(v^e) mod p'
        BigInteger x = (BigInteger.ModPow(a, signatureBack2, p) * BigInteger.ModPow(v, signatureBack1, p)) % p;

        // Confirm that concatenatation of the message 'M' and 'x' hashes to 'e'
        // NOTE: Hash algorithm needs to be the same!!! (SHA256 for us)
        byte[] bytes = contentBytes.Concat(x.ToByteArray()).ToArray();
        byte[] hash = hashAlgorithm.ComputeHash(bytes);
        BigInteger hashInteger = BigInteger.Abs(new BigInteger(hash));

        if (signatureBack1 == hashInteger)
            gui.ShowAlert("Podpis poprawny!", "");
        else
            gui.ShowAlert("Podpis niepoprawny!!!", "");
    }

    private string BytesToString(byte[] bytes)
    {
        StringBuilder hex = new StringBuilder(bytes.Length * 2);
        for (int i = 0; i < bytes.Length; i++)
            hex.AppendFormat("{0:x2}", bytes[i]);
        return hex.ToString();
    }

    private byte[] StringToBytes(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    private (BigInteger, BigInteger) GenerateTwoPrimeNumbers(int length)
    {
        using (RSACryptoServiceProvider myRsa = new RSACryptoServiceProvider(length))
        {
            byte[] sbin; // The raw byte array
            byte[] ubin;

            // Export RSA certificate to an XML string
            string xml = myRsa.ToXmlString(true);

            BigInteger bigPrime1;
            BigInteger bigPrime2;
            int pEndIndex = 0;
            {
                // Locate <P> in the Base64 certificate
                int s = xml.IndexOf("<P>") + 3;
                pEndIndex = xml.IndexOf("<", s);
                int l = pEndIndex - s;

                // Convert P from Base64 to a raw byte array
                sbin = Convert.FromBase64String(xml.Substring(s, l));
                Array.Reverse(sbin);

                // Postfix a null byte to keep BigInteger happy
                ubin = new byte[sbin.Length + 1];
                ubin[sbin.Length] = 0;

                Buffer.BlockCopy(sbin, 0, ubin, 0, sbin.Length);

                // Convert the bytes into positive BigNumber
                bigPrime1 = new BigInteger(ubin);
            }

            {
                // Locate <Q> in the Base64 certificate
                int s = xml.IndexOf("<Q>", pEndIndex) + 3;
                int l = xml.IndexOf("<", s) - s;

                // Convert P from Base64 to a raw byte array
                sbin = Convert.FromBase64String(xml.Substring(s, l));
                Array.Reverse(sbin);

                // Postfix a null byte to keep BigInteger happy
                ubin = new byte[sbin.Length + 1];
                ubin[sbin.Length] = 0;

                Buffer.BlockCopy(sbin, 0, ubin, 0, sbin.Length);

                // Convert the bytes into positive BigNumber
                bigPrime2 = new BigInteger(ubin);
            }

            return (bigPrime1, bigPrime2);
        }
    }

    private static BigInteger ModInverse(BigInteger value, BigInteger modulo) {
        BigInteger x, y;

        if (1 != Egcd(value, modulo, out x, out y))
            throw new ArgumentException("Invalid modulo", nameof(modulo));

        if (x < 0)
            x += modulo;

        return x % modulo;
    }

    private static BigInteger Egcd(BigInteger left, BigInteger right, out BigInteger leftFactor, out BigInteger rightFactor) {
        leftFactor = 0;
        rightFactor = 1;
        BigInteger u = 1;
        BigInteger v = 0;
        BigInteger gcd = 0;

        while (left != 0) {
            BigInteger q = right / left;
            BigInteger r = right % left;

            BigInteger m = leftFactor - u * q;
            BigInteger n = rightFactor - v * q;

            right = left;
            left = r;
            leftFactor = u;
            rightFactor = v;
            u = m;
            v = n;

            gcd = right;
        }

        return gcd;
    }
}

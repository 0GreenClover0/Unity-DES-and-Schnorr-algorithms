using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Schnorr : MonoBehaviour
{
    public GUI gui;
    public FileManager fileManager;

    [SerializeField] private TMP_InputField signInputField;
    [SerializeField] private TMP_InputField signatureInputField;

    [SerializeField] private Toggle signFromFile;
    [SerializeField] private Toggle verifyFromFile;

    [SerializeField] private TMP_InputField publicKey;
    [SerializeField] private TMP_InputField privateKey;
    [SerializeField] private TMP_InputField qText;
    [SerializeField] private TMP_InputField pText;
    [SerializeField] private TMP_InputField aText;

    private HashAlgorithm hashAlgorithm = HashAlgorithm.Create();

    private BigInteger q, p, a, s, v;

    private System.Random systemRandom = new System.Random();

    public void GenerateKeys()
    {
        q = BigInteger.Zero;
        p = BigInteger.Zero;
        a = BigInteger.Zero;
        s = BigInteger.Zero;
        v = BigInteger.Zero;

        // https://mrajacse.files.wordpress.com/2012/01/applied-cryptography-2nd-ed-b-schneier.pdf#%F6F%FD%5C%5CU%EC%E8S%A2ocmd%0C%04%A6Heading4
        // To generate a key pair, first choose two primes, 'p' and 'q', such that 'q' is a prime factor of 'p - 1'

        // Generate 'q'
        bool isPrime = false;
        do
        {
            // Schnorr recommended that 'q' be about 140 bits. Generate a random BigInteger that is ~140 bits long
            q = BigIntegerGenerator.NextBigInteger(140);

            // Check if it is a probable prime
            isPrime = BigIntegerPrimeTest.IsProbablePrime(q, 100);
        } while (!isPrime);

        // Generate p
        while (true)
        {
            BigInteger original = q * BigIntegerGenerator.NextBigInteger(372, even: true) + BigInteger.One;
            BigInteger temp;

            for (int i = 1; i <= 10; i++)
            {
                BigInteger step = new BigInteger(i) * q;
                temp = original + step;
                if (BigIntegerPrimeTest.IsProbablePrime(temp, 2))
                {
                    p = temp;
                    break;
                }

                temp = original - step;
                if (BigIntegerPrimeTest.IsProbablePrime(temp, 2))
                {
                    p = temp;
                    break;
                }
            }

            if (p != BigInteger.Zero)
            {
                break;
            }
        }

        // Then, choose an 'a' not equal to 1, thanks to which 'h^k' will be not congruent to '1 mod p'
        // This means that 'h^k mod p' can't result in a value of 1 
        BigInteger k = (p - BigInteger.One) / q;
        do
        {
            BigInteger h = BigIntegerGenerator.NextBigIntegerInRange(systemRandom, BigInteger.One + BigInteger.One, p);

            a = BigInteger.ModPow(h, k, p);
        } while (a == BigInteger.One);

        // To generate a particular public-key/private-key key pair
        // choose a random number 's' less than 'q', this is the private key
        s = BigIntegerGenerator.RandomBigInteger(BigInteger.Zero, q);

        // Then calculate 'v = a^(-s) mod p'. This is the public key.
        v = BigInteger.ModPow(a, s, p);
        v = ModInverse(v, p); // NOTE: BigInteger.ModPow doesn't take negative exponents as an argument, so we need to ModInverse() the result

        publicKey.text = BytesToString(v.ToByteArray());
        privateKey.text = BytesToString(s.ToByteArray());
        qText.text = BytesToString(q.ToByteArray());
        pText.text = BytesToString(p.ToByteArray());
        aText.text = BytesToString(a.ToByteArray());
    }

    public void OnSignPressed()
    {
        if (signFromFile.isOn && String.IsNullOrEmpty(fileManager.encryptLoadPathValidated))
        {
            gui.ShowAlert("Opcja 'Podpisywanie pliku' jest wybrana, ale plik źródłowy nie został wybrany.");
            return;
        }

        if (a == BigInteger.Zero)
            GenerateKeys();

        byte[] contentBytes = null;
        if (signFromFile.isOn)
            contentBytes = File.ReadAllBytes(fileManager.encryptLoadPathValidated);
        else
            contentBytes = EncrypterDecrypter.GetBytesFromString(signInputField.text, asHex: false);

        // Pick a random number 'r', less than 'q'
        BigInteger r = BigIntegerGenerator.RandomBigInteger(BigInteger.Zero, q);

        // And compute 'x = a^r mod p'
        BigInteger x = BigInteger.ModPow(a, r, p);

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
        if (verifyFromFile.isOn && String.IsNullOrEmpty(fileManager.decryptLoadPathValidated))
        {
            gui.ShowAlert("Opcja 'Weryfikowanie pliku' jest wybrana, ale plik źródłowy nie został wybrany.");
            return;
        }

        byte[] contentBytes = null;
        if (verifyFromFile.isOn)
            contentBytes = File.ReadAllBytes(fileManager.decryptLoadPathValidated);
        else
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

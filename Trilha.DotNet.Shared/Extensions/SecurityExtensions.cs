namespace Trilha.DotNet.Shared.Extensions;

public static class SecurityExtensions
{
    private const int Iteration = 10000;

    public static string Encode(this string plainText, Guid hashKey, Guid saltKey, string iv16Key)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            throw new ArgumentException("Invalid Plain Text", nameof(plainText));

        if (hashKey == Guid.Empty)
            throw new ArgumentException("Invalid Hash Key", nameof(hashKey));

        if (saltKey == Guid.Empty)
            throw new ArgumentException("Invalid Salt Key", nameof(saltKey));

        const string errorIv16 = "(IV) Initial Vector must have 16 bytes";

        if (string.IsNullOrWhiteSpace(iv16Key))
            throw new ArgumentException(errorIv16, nameof(iv16Key));

        var iv16 = Convert.FromBase64String(iv16Key);

        if (iv16.Length != 16)
            throw new ArgumentException(errorIv16, nameof(iv16Key));

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        var keyBytes = new Rfc2898DeriveBytes(
            hashKey.ToString(), Encoding.UTF8.GetBytes(saltKey.ToString()), Iteration, HashAlgorithmName.SHA512).GetBytes(256 / 8);

        var symmetricKey = Aes.Create();
        symmetricKey.Mode = CipherMode.CBC;
        symmetricKey.Padding = PaddingMode.Zeros;

        var encryptor = symmetricKey.CreateEncryptor(keyBytes, iv16);

        byte[] cipherTextBytes;

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            cipherTextBytes = memoryStream.ToArray();
            cryptoStream.Close();
        }

        memoryStream.Close();

        return Convert.ToBase64String(cipherTextBytes);
    }

    public static string Decode(this string encryptedText, Guid hashKey, Guid saltKey, string iv16Key)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            throw new ArgumentException("Invalid Encrypted Text", nameof(encryptedText));

        if (hashKey == Guid.Empty)
            throw new ArgumentException("Invalid Hash Key", nameof(hashKey));

        if (saltKey == Guid.Empty)
            throw new ArgumentException("Invalid Salt Key", nameof(saltKey));

        const string errorIv16 = "(IV) Initial Vector must have 16 bytes";

        if (string.IsNullOrWhiteSpace(iv16Key))
            throw new ArgumentException(errorIv16, nameof(iv16Key));

        var iv16 = Convert.FromBase64String(iv16Key);

        if (iv16.Length != 16)
            throw new ArgumentException(errorIv16, nameof(iv16Key));

        var cipherTextBytes = Convert.FromBase64String(encryptedText);

        var keyBytes = new Rfc2898DeriveBytes(
            hashKey.ToString(), Encoding.UTF8.GetBytes(saltKey.ToString()), Iteration, HashAlgorithmName.SHA512).GetBytes(256 / 8);

        var symmetricKey = Aes.Create();
        symmetricKey.Mode = CipherMode.CBC;
        symmetricKey.Padding = PaddingMode.Zeros;

        var decryptor = symmetricKey.CreateDecryptor(keyBytes, iv16);
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        var plainTextBytes = new byte[cipherTextBytes.Length];

        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();

        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
    }

    public static string NewIvKey(this int byteSize)
    {
        var rnd = new Random();
        var b = new byte[byteSize];
        rnd.NextBytes(b);

        return Convert.ToBase64String(b);
    }

    public static string ToRot18(this string input)
    {
        const string plainText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string cipherText = "NOPQRSTUVWXYZABCDEFGHIJKLM5678901234";

        var map = new Dictionary<char, char>();

        for (var i = 0; i < plainText.Length; i++)
        {
            map.Add(plainText[i], cipherText[i]);
        }

        var newString = new StringBuilder();

        foreach (var c in input)
        {
            var upperC = char.ToUpperInvariant(c);

            if (map.TryGetValue(upperC, out var value))
            {
                newString.Append(char.IsLower(c) ? char.ToLowerInvariant(value) : value);
            }
            else
            {
                newString.Append(c);
            }
        }

        return newString.ToString();
    }
}

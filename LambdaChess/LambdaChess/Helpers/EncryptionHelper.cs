using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace LambdaChess.Helpers;

public static class EncryptionHelper {
    private static readonly byte[]
        Key = Convert.FromBase64String("mAR6UAbkDI/IwA90mrcx44z+PtNqDTjvWQqJqLhdpfQ=");
    private static readonly byte[] IV = Convert.FromBase64String("vBzzUwsVBKZYXtdYJAffZg==");

    public static void SaveUserData<TDataType>(TDataType data, string filePath) where TDataType : class {
        var jsonData = JsonConvert.SerializeObject(data);
        var encryptedData = Encrypt(jsonData);

        File.WriteAllBytes(filePath, encryptedData);
    }

    public static object? LoadUserData<TDataType>(string filePath) {
        if (!File.Exists(filePath))
            return null;

        var encryptedData = File.ReadAllBytes(filePath);
        var jsonData = Decrypt(encryptedData);

        return JsonConvert.DeserializeObject<TDataType>(jsonData);
    }

    private static byte[] Encrypt(string plainText) {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs)) {
            sw.Write(plainText);
        }

        return ms.ToArray();
    }

    private static string Decrypt(byte[] cipherText) {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
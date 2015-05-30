namespace StrangeToolkit.Cryptography
{
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;

    public static class Md5Calculator
    {
        public static string ComputeMd5(string strValue)
        {
            var algorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var buffer = CryptographicBuffer.ConvertStringToBinary(strValue, BinaryStringEncoding.Utf8);
            var hashed = algorithm.HashData(buffer);
            return CryptographicBuffer.EncodeToHexString(hashed);
        }
    }
}

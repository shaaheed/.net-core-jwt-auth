using System;
using System.Security.Cryptography;

namespace JwtAuthExample.Extensions
{
    public static class UniqueKeyGenerator
    {
        public static long GetUniqueNumber()
        {
            var length = 32;
            var bytes = new byte[length];
            var cryptoRandomDataGenerator = RandomNumberGenerator.Create();
            cryptoRandomDataGenerator.GetBytes(bytes);
            var random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return random;
        }
    }
}

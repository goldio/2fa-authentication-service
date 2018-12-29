using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gold.IO.Authentication.Service.Utils
{
    public class Validate
    {
        public static bool ValidateBitcoinAddress(string address)
        {
            if (address.Length < 26 || address.Length > 35)
            {
                //throw new Exception("wrong length");
                return false;
            }

            var decoded = DecodeBase58(address);
            var d1 = Hash(decoded.SubArray(0, 21));
            var d2 = Hash(d1);

            if (!decoded.SubArray(21, 4).SequenceEqual(d2.SubArray(0, 4)))
            {
                //throw new Exception("bad digest");
                return false;
            }

            return true;
        }

        const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        const int Size = 25;

        private static byte[] DecodeBase58(string input)
        {
            var output = new byte[Size];
            foreach (var t in input)
            {
                var p = Alphabet.IndexOf(t);
                if (p == -1) throw new Exception("invalid character found");
                var j = Size;
                while (--j > 0)
                {
                    p += 58 * output[j];
                    output[j] = (byte)(p % 256);
                    p /= 256;
                }
                if (p != 0) throw new Exception("address too long");
            }
            return output;
        }

        private static byte[] Hash(byte[] bytes)
        {
            var hasher = new SHA256Managed();
            return hasher.ComputeHash(bytes);
        }
    }

    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}

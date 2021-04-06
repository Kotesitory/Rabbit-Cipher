using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionImplementation
{
    class RabbitUtils
    {
        public static uint g_function(QByte state, QByte counter)
        {
            uint x = state.ToUInt();
            uint y = counter.ToUInt();
            ulong op1 = (x + y) % uint.MaxValue;
            op1 *= op1;
            //uint op2 = op1 >> 32;
            uint result = (uint)(op1 ^ (op1 >> 32)) % uint.MaxValue;
            return result;
        }

        public static uint LeftRotate(uint num, int bits)
        {
            return (num << bits) | (num >> (32 - bits));
        }

        public static uint RightRotate(uint num, int bits)
        {
            return (num >> bits) | (num << (32 - bits));
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            string result = BitConverter.ToString(bytes);
            return result.Replace("-", "");
        }
    }
}

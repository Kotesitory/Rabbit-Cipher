using System;
using System.Linq;

namespace EncryptionImplementation
{
    class RabbitUtils
    {
        /// <summary>
        /// Helper function defined in the original paper for iterating the state
        /// </summary>
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

        /// <summary>
        /// Converts a hex string to a byte array
        /// There is no value validation. 
        /// Throws FormatException if format is incorrect
        /// </summary>
        /// <param name="hex">String to be converted</param>
        /// <returns>Returns array of bytes representing the value contained in the hex string</returns>
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Converts a byte array to a hex string
        /// This is useful for preview or printing
        /// </summary>
        /// <param name="bytes">Byte array to be converted</param>
        /// <returns>Returns a hex string with the value of the provided bytes</returns>
        public static string ByteArrayToHexString(byte[] bytes)
        {
            string result = BitConverter.ToString(bytes);
            return result.Replace("-", "");
        }
    }
}

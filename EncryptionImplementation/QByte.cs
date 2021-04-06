using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionImplementation
{
    /// <summary>
    /// Data structure to hold four bytes of 
    /// data for easier conversion and working. 
    /// It's basicly an implementation that allows 
    /// me to work with each byte of a uint 
    /// directly without requiring bit shifting.
    /// </summary>
    class QByte
    {
        byte[] bytes = new byte[4];
        public uint Value
        {
            get
            {
                return this.ToUInt();
            }
        }

        public QByte(uint num)
        {
            byte[] num_bytes = BitConverter.GetBytes(num);
            this.bytes[0] = num_bytes[0];
            this.bytes[1] = num_bytes[1];
            this.bytes[2] = num_bytes[2];
            this.bytes[3] = num_bytes[3];
        }

        public QByte(byte a, byte b, byte c, byte d)
        {
            this.bytes[0] = a;
            this.bytes[1] = b;
            this.bytes[2] = c;
            this.bytes[3] = d;
        }

        public byte this[int i]
        {
            get { return bytes[i]; }
            set { bytes[i] = value; }
        }

        /// <summary>
        /// Converts the bytes of data into a uint for operations
        /// that require it
        /// </summary>
        /// <returns>uint representing the value stored in the 4 bytes of data</returns>
        public uint ToUInt()
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.bytes);
            }

            uint result = BitConverter.ToUInt32(bytes, 0);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.bytes);
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionImplementation
{
    class Rabbit
    {
        /// <summary>
        /// Constants used for incrementing the counts
        /// </summary>
        public static readonly uint[] aConstants = {
            0x4D34D34D,
            0xD34D34D3,
            0x34D34D34,
            0x4D34D34D,
            0xD34D34D3,
            0x34D34D34,
            0x4D34D34D,
            0xD34D34D3
        };

        byte[] key;
        byte[] iv;
        State state;
        List<byte[]> streamkeys = new List<byte[]>();

        public Rabbit(byte[] key, byte[] iv = null)
        {
            if (key.Length != 16)
            {
                throw new ArgumentException("Key must be 128 bits");
            }

            if (iv != null && iv.Length != 8)
            {
                throw new ArgumentException("Initialization vector must be 64 bits");
            }

            this.key = key;
            this.iv = iv;
            this.state = new State();
            this.state.Init(key, iv);
        }

        /// <summary>
        /// Generates a keystream from the current state and counts
        /// </summary>
        public byte[] GenerateKeystream()
        {
            byte[] streamkey = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    streamkey[2 * i] = (byte)(this.state.states[i][0] ^ this.state.states[(i + 5) % 8][2]);
                    streamkey[2 * i + 1] = (byte)(this.state.states[i][1] ^ this.state.states[(i + 5) % 8][3]);
                }
                else
                {
                    streamkey[2 * i] = (byte)(this.state.states[i - 1][2] ^ this.state.states[(i + 2) % 8][0]);
                    streamkey[2 * i + 1] = (byte)(this.state.states[i - 1][3] ^ this.state.states[(i + 2) % 8][1]);
                }
            }

            return streamkey;
        }

        /// <summary>
        /// Encrypts a 128bit block of plaintext
        /// </summary>
        /// <param name="plaintextBlock">Block of plaintext to be encrypted</param>
        /// <returns>Returns a 128bit of ciphertext</returns>
        public byte[] EncryptBlock(byte[] plaintextBlock)
        {
            if (plaintextBlock.Length > 16)
            {
                throw new ArgumentException("Blocks must be 128 bits or shorter");
            }

            this.state.Iterate();
            byte[] streamkey = this.GenerateKeystream();
            this.streamkeys.Add(streamkey);
            byte[] ciphertextBlock = new byte[plaintextBlock.Length];
            for (int i = 0; i < plaintextBlock.Length; i++)
            {
                ciphertextBlock[i] = (byte)(plaintextBlock[i] ^ streamkey[i]);
            }

            return ciphertextBlock;
        }
    }
}

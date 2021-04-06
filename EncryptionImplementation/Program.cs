using System;
using System.Text;

namespace EncryptionImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            string keyString = "80000000000000000000000000000000";
            string ivString = "0000000000000000";
            byte[] key = RabbitUtils.StringToByteArray(keyString);
            byte[] iv = RabbitUtils.StringToByteArray(ivString);
            StringBuilder plaintext = new StringBuilder();

            // Building a 128bit block of plain text
            for (int i = 0; i < 2*16; i++)
            {
                plaintext.Append("0");
            }

            byte[] plainBytes = RabbitUtils.StringToByteArray(plaintext.ToString());
            Rabbit rabbit = new Rabbit(key, iv);

            // Repeating 32 times in order to simulate 512 zero bytes
            // As requested in the test file https://github.com/cantora/avr-crypto-lib/blob/master/testvectors/rabbit-verified.test-vectors
            for (int i = 0; i < 32; i++)
            {
                byte[] ciphertextBytes = rabbit.EncryptBlock(plainBytes);
                string cyphertext = RabbitUtils.ByteArrayToHexString(ciphertextBytes);
                Console.WriteLine(cyphertext);
            }
        }
    }
}
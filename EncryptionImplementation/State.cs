using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionImplementation
{
    class State
    {
        public QByte[] states = new QByte[8];
        public QByte[] counters = new QByte[8];
        public ushort carry = 0;

        public void Init(byte[] key, byte[] iv = null)
        {
            if (key.Length != 16)
            {
                throw new ArgumentException("Key must be 128 bits");
            }

            if (iv != null && iv.Length != 8)
            {
                throw new ArgumentException("Initialization vector must be 64 bits");
            }

            this.InitStates(key);
            this.InitCounters(key);
            for (int i = 0; i < 4; i++)
            {
                this.Iterate();
            }

            this.ReinitCounters();
            if (iv != null)
                this.IVInitCounters(iv);
        }

        private void InitStates(byte[] key)
        {
            if (key.Length != 16)
            {
                throw new ArgumentException("Key must be 128 bits");
            }

            for (int j = 0; j < 8; j++)
            {
                if (j % 2 == 0)
                {
                    int index = (j + 1) % 8;
                    this.states[j] = new QByte(key[2 * j], key[2 * j + 1], key[2 * index], key[2 * index + 1]);
                }
                else
                {
                    int index1 = (j + 5) % 8;
                    int index2 = (j + 4) % 8;
                    this.states[j] = new QByte(key[2 * index2], key[2 * index2 + 1], key[2 * index1], key[2 * index1 + 1]);
                }
            }
        }

        private void InitCounters(byte[] key)
        {
            if (key.Length != 16)
            {
                throw new ArgumentException("Key must be 128 bits");
            }

            for (int j = 0; j < 8; j++)
            {
                if (j % 2 == 0)
                {
                    int index1 = (j + 5) % 8;
                    int index2 = (j + 4) % 8;
                    this.counters[j] = new QByte(key[2 * index1], key[2 * index1 + 1], key[2 * index2], key[2 * index2 + 1]);
                }
                else
                {
                    int index = (j + 1) % 8;
                    this.counters[j] = new QByte(key[2 * index], key[2 * index + 1], key[2 * j], key[2 * j + 1]);
                }
            }
        }

        private void IVInitCounters(byte[] iv)
        {
            if (iv.Length != 8)
            {
                throw new ArgumentException("Initialization vector must be 64 bits");
            }

            for (int i = 0; i < 8; i++)
            {
                if (i % 4 == 0)
                {
                    this.counters[i][0] ^= iv[0];
                    this.counters[i][1] ^= iv[1];
                    this.counters[i][2] ^= iv[2];
                    this.counters[i][3] ^= iv[3];
                }
                else if (i % 4 == 1)
                {
                    this.counters[i][0] ^= iv[2];
                    this.counters[i][1] ^= iv[3];
                    this.counters[i][2] ^= iv[6];
                    this.counters[i][3] ^= iv[7];
                }
                else if (i % 4 == 2)
                {
                    this.counters[i][0] ^= iv[4];
                    this.counters[i][1] ^= iv[5];
                    this.counters[i][2] ^= iv[6];
                    this.counters[i][3] ^= iv[7];
                }
                else
                {
                    this.counters[i][0] ^= iv[0];
                    this.counters[i][1] ^= iv[1];
                    this.counters[i][2] ^= iv[4];
                    this.counters[i][3] ^= iv[5];
                }

            }

            for (int i = 0; i < 4; i++)
                this.Iterate();
        }

        private void ReinitCounters()
        {
            for (int i = 0; i < 8; i++)
            {
                int index = (i + 4) % 8;
                this.counters[i][0] ^= this.states[index][0];
                this.counters[i][1] ^= this.states[index][1];
                this.counters[i][2] ^= this.states[index][2];
                this.counters[i][3] ^= this.states[index][3];
            }
        }

        private void IterateState()
        {
            QByte[] newStates = new QByte[8];
            for (int i = 0; i < 8; i++)
            {
                uint op1 = RabbitUtils.g_function(this.states[i], this.counters[i]);
                uint op2 = RabbitUtils.g_function(this.states[(i + 7) % 8], this.counters[(i + 7) % 8]);
                uint op3 = RabbitUtils.g_function(this.states[(i + 6) % 8], this.counters[(i + 6) % 8]);
                if (i % 2 == 0)
                {
                    op2 = RabbitUtils.LeftRotate(op2, 16);
                    op3 = RabbitUtils.LeftRotate(op3, 16);
                }
                else
                {
                    op2 = RabbitUtils.LeftRotate(op2, 8);
                }

                ulong tmp = op1 + op2 + op3;
                uint result = (uint)(tmp % uint.MaxValue);
                newStates[i] = new QByte(result);
            }

            this.states = newStates;
        }

        private void IterateCounters()
        {
            QByte[] newCounters = new QByte[8];
            for (int i = 0; i < 8; i++)
            {
                uint result;
                try
                {
                    result = checked(this.counters[i].ToUInt() + Rabbit.aConstants[i] + this.carry);
                    this.carry = 0;
                }
                catch (OverflowException)
                {
                    result = this.counters[i].ToUInt() + Rabbit.aConstants[i] + this.carry;
                    this.carry = 1;
                }

                newCounters[i] = new QByte(result);
            }

            this.counters = newCounters;
        }

        public void Iterate()
        {
            this.IterateCounters();
            this.IterateState();
        }
    }
}

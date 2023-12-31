using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Memory_Scanner__Take_3_.Imps;

namespace Memory_Scanner__Take_3_
{
    public partial class Form1
    {
        /// <summary>
        /// READ A STRING VALUE FROM AN ADDRESS
        /// </summary>
        /// <param name="code">THE ADDRESS</param>
        /// <param name="length">LENGTH OF BYTES TO READ FROM</param>
        /// <param name="zeroTerminated">TERMINATE THE STRING AT NULL</param>
        /// <param name="stringEncoding">SYSTEM.TEXT.ENCODING IS SET TO ASCII AS DEFAULT</param>
        /// <returns></returns>
        public string ReadString(long code, int length = 32, bool zeroTerminated = true, System.Text.Encoding stringEncoding = null)
        {
            if (stringEncoding == null)
            {
                stringEncoding = System.Text.Encoding.ASCII;
            }

            byte[] mem = new byte[length];

            if (ReadProcessMemory(mProcess.Handle, code, mem, (UIntPtr)length, IntPtr.Zero))
            {
                return (zeroTerminated) ? stringEncoding.GetString(mem).Split('\0')[0] : stringEncoding.GetString(mem);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// READS A SINGLE BYTE FROM AN ADDRESS
        /// </summary>
        /// <param name="code">THE ADDRESS TO READ FROM</param>
        /// <returns></returns>
        public int ReadByte(long code)
        {
            byte[] mem = new byte[1];

            if (ReadProcessMemory(mProcess.Handle, code, mem, (UIntPtr)1, IntPtr.Zero))
            {
                return mem[0];
            }
            return 0;
        }

        /// <summary>
        /// READ A 4 BYTE BIG ENDIAN VALUE FROM AN ADDRESS
        /// </summary>
        /// <param name="code">THE ADDRESS TO READ FROM</param>
        /// <returns></returns>
        public int Read4ByteBigEndian(long code)
        {
            byte[] memory = new byte[4];
            if (ReadProcessMemory(mProcess.Handle, code, memory, (UIntPtr)4, IntPtr.Zero))
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(memory);
                }
                return BitConverter.ToInt32(memory, 0);
            }
            return 0;
        }

        /// <summary>
        /// READ A 2 BYTE BIG ENDIAN VALUE FROM AN ADDRESS
        /// </summary>
        /// <param name="code">THE ADDRESS TO READ FROM</param>
        /// <returns></returns>
        public short Read2ByteBigEndian(long code)
        {
            byte[] memory = new byte[2];
            if (ReadProcessMemory(mProcess.Handle, code, memory, (UIntPtr)2, IntPtr.Zero))
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(memory);
                }
                return BitConverter.ToInt16(memory, 0);
            }
            return 0;
        }

        /// <summary>
        /// READ A FLOAT BIG ENDIAN VALUE FROM AN ADDRESS
        /// </summary>
        /// <param name="code">THE ADDRESS TO READ FROM</param>
        /// <returns></returns>
        public float ReadFloatBigEndian(long code)
        {
            byte[] memory = new byte[4];
            if (ReadProcessMemory(mProcess.Handle, code, memory, (UIntPtr)4, IntPtr.Zero))
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(memory);
                }
                return BitConverter.ToSingle(memory, 0);
            }
            return 0;
        }

        /// <summary>
        /// READ A DOUBLE BIG ENDIAN VALUE FROM AN ADDRESS
        /// </summary>
        /// <param name="code">THE ADDRESS TO READ FROM</param>
        /// <returns></returns>
        public double ReadDoubleBigEndian(long code)
        {
            byte[] memory = new byte[8];
            if (ReadProcessMemory(mProcess.Handle, code, memory, (UIntPtr)8, IntPtr.Zero))
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(memory);
                }
                return BitConverter.ToDouble(memory, 0);
            }
            return 0;
        }

    }
}

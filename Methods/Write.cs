﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Memory_Scanner__Take_3_.Imps;

namespace Memory_Scanner__Take_3_
{
    public partial class Main
    {
        public bool WriteMemory(long code, string type, string write, System.Text.Encoding stringEncoding = null)
        {
            byte[] memory = new byte[4];
            int size = 4;

            try
            {
                if (type.ToLower() == "4 byte big endian")
                {
                    memory = BitConverter.GetBytes(Convert.ToInt32(write));

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(memory);
                    }
                }
                else if (type.ToLower() == "2 byte big endian")
                {
                    memory = BitConverter.GetBytes(Convert.ToInt16(write));

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(memory);
                    }
                }
                else if (type.ToLower() == "string")
                {
                    stringEncoding = System.Text.Encoding.ASCII;

                    memory = stringEncoding.GetBytes(write);

                    Debug.WriteLine(memory);

                    size = memory.Length;
                }
                else if (type.ToLower() == "byte")
                {
                    memory = new byte[] { Convert.ToByte(write) };

                    size = memory.Length;
                }
            }
            catch (FormatException)
            {
                Debug.WriteLine($"ERROR: {write} is not a valid integer");
            }

            bool writeProcessMemory = false;

            writeProcessMemory = WriteProcessMemory(mProcess.Handle, code, memory, (UIntPtr)size, IntPtr.Zero);

            return writeProcessMemory;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Memory_Scanner__Take_3_.Imps;

namespace Memory_Scanner__Take_3_
{
    public partial class Form1 : Form
    {
        public Proc mProcess = new Proc();

        public Form1()
        {
            InitializeComponent();

            // VALUE TYPE COMBO BOX

            comboBox1.Items.Add("String");
            comboBox1.Items.Add("Byte");
            comboBox1.Items.Add("4 Byte Big Endian");
            comboBox1.Items.Add("2 Byte Big Endian");
            comboBox1.Items.Add("Float Big Endian");
            comboBox1.Items.Add("Double Big Endian");

            // SCAN TYPE COMBO BOX

            comboBox2.Items.Add("Exact Value");

            label2.Text = "NULL";
        }

        public int GetProcessIdFromName(string name)
        {
            Process[] processList = Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == name)
                {
                    return process.Id;
                }
            }

            return 0; // RETURN 0 IF WE FAIL TO FIND THE TARGET PROCESS
        }

        public bool OpenProcess(int processId)
        {
            try
            {
                mProcess.Process = Process.GetProcessById(processId);

                if (mProcess.Process != null && !mProcess.Process.Responding)
                {
                    Debug.WriteLine("ERROR: THE PROCESS IS NOT RESPONDING OR IS NULL");
                    return false;

                }

                mProcess.Handle = Imps.OpenProcess(0x1F0FFF, true, processId);

                Debug.WriteLine($"PROCESS: {mProcess.Process} HAS NOW BEEN OPENED, PROCESS ID: {processId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: OPENPROCESS HAS CRASHED" + ex);
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string processName = "rpcs3";

            int processId = GetProcessIdFromName(processName);

            if (processId <= 0)
            {
                Debug.WriteLine($"ERROR: FAILED TO FIND PROCESS ({processName})");

                label2.Text = "FAILED TO OPEN PROCESS";
            }
            else
            {
                bool isProcOpen = OpenProcess(processId);

                if (isProcOpen)
                {
                    label2.Text = "OPENED";
                }
            }

        }

        private Dictionary<long, object> keyValuePairs = new Dictionary<long, object>();

        private string valueType;

        public void FirstScan(long start, long end, object valueToFind, string valueType, ListView listView1)
        {
            this.valueType = valueType; // STORE THE SPECIFIED VALUE TYPE

            keyValuePairs.Clear(); // CLEAR PREVIOUS SCAN RESULTS BEFORE A NEW SCAN

            for (long address = start; address <= end; address++)
            {
                if (valueType == "String")
                {
                    string valueAtAddress = ReadString(address);

                    Debug.WriteLine($"VALUE AT ADDRESS: {address:X} VALUE: {valueAtAddress}");

                    if (valueAtAddress == valueToFind as string)
                    {
                        listView1.Items.Add($"ADDRESS: {address:X}, VALUE: {valueAtAddress}");
                        keyValuePairs[address] = valueAtAddress;
                    }
                }

                if (valueType == "4 Byte Big Endian")
                {
                    int valueAtAddress = Read4ByteBigEndian(address);

                    if (valueAtAddress == Convert.ToInt32(valueToFind))
                    {
                        listView1.Items.Add($"ADDRESS: {address:X}, VALUE: {valueAtAddress}");
                        keyValuePairs[address] = valueAtAddress;
                    }
                }

                if (valueType == "2 Byte Big Endian")
                {
                    int valueAtAddress = Read2ByteBigEndian(address);

                    Debug.WriteLine($"VALUE AT ADDRESS: {address:X} VALUE: {valueAtAddress}");

                    if (valueAtAddress == Convert.ToInt16(valueToFind))
                    {
                        listView1.Items.Add($"ADDRESS: {address:X}, VALUE: {valueAtAddress}");
                        keyValuePairs[address] = valueAtAddress;
                    }
                }
            }

            MessageBox.Show("SCAN COMPLETE.");
        }

        public void NextScan(object valueToFind, ListView listView1)
        {
            Dictionary<long, object> keyValuePairs2 = new Dictionary<long, object>();

            foreach (var pair in keyValuePairs)
            {
                long address = pair.Key; // GET THE ADDRESS FROM THE CURRENT PAIR
                object valueAtAddress = null;

                if (valueType == "String")
                {
                    valueAtAddress = ReadString(address);
                }
                else if (valueType == "4 Byte Big Endian")
                {
                    valueAtAddress = Read4ByteBigEndian(address);
                }
                else if (valueType == "2 Byte Big Endian")
                {
                    valueAtAddress = Read2ByteBigEndian(address);
                }

                // IF THE VALUE AT THE ADDRESS EQUALS THE VALUE TO FIND

                if (valueAtAddress.Equals(valueToFind))
                {
                    listView1.Items.Add($"ADDRESS: {address:X}, VALUE: {valueAtAddress}");
                    keyValuePairs2[address] = valueAtAddress; // ADD THE CURRENT ADDRESS AND VALUE TO THE CURRENT SCAN RESULTS
                }

                keyValuePairs = keyValuePairs2; // UPDATE THE PREVIOUS SCAN WITH THE CURRENT SCAN SO THAT NEXT SCAN IS READY AGAIN
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FirstScan(0x330000000, 0x340000000, textBox1.Text, comboBox1.SelectedItem.ToString(), listView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() == "Exact Value")
            {
                NextScan(textBox1.Text, listView1);
            }
        }
    }
}

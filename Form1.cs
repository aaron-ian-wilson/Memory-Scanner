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
using Microsoft.VisualBasic;
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

            foreach (Control control in this.Controls)
            {
                control.Enabled = false;
            }

            button1.Enabled = true;

            label1.Enabled = true;
            label2.Enabled = true;

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

                mProcess.Handle = Imps.OpenProcess(PROCESS_VM_READ + PROCESS_VM_WRITE + PROCESS_VM_OPERATION, true, processId);

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
            foreach (Control control in this.Controls)
            {
                control.Enabled = true;
            }

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

        public void FirstScan(long start, long end, object valueToFind, string valueType, ListView listView1, ProgressBar progressBar1)
        {
            listView1.Clear();

            this.valueType = valueType; // STORE THE SPECIFIED VALUE TYPE

            keyValuePairs.Clear(); // CLEAR PREVIOUS SCAN RESULTS BEFORE A NEW SCAN

            List<string> results = new List<string>();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = (int)(end - start + 1);
            progressBar1.Value = 0;

            for (long address = start; address <= end; address += 4)
            {
                progressBar1.Value++;

                if (valueType == "String")
                {
                    string valueAtAddress = ReadString(address);

                    if (valueAtAddress.Equals(valueToFind))
                    {
                        results.Add($"ADDRESS: {address:X}, TYPE: {valueType}, VALUE: {valueAtAddress}");
                        keyValuePairs[address] = valueAtAddress;
                    }
                }

                if (valueType == "4 Byte Big Endian")
                {
                    long valueAtAddress = Read4ByteBigEndian(address);

                    if (valueAtAddress.Equals(Convert.ToInt32(valueToFind)))
                    {
                        results.Add($"ADDRESS: {address:X}, TYPE: {valueType}, VALUE: {valueAtAddress}");
                        keyValuePairs[address] = valueAtAddress;
                    }
                }

                if (valueType == "2 Byte Big Endian")
                {
                    int valueAtAddress = Read2ByteBigEndian(address);

                    if (valueAtAddress.Equals(Convert.ToInt16(valueToFind)))
                    {
                        results.Add($"ADDRESS: {address:X}, VALUE: {valueAtAddress}");
                        keyValuePairs[address] = valueAtAddress;
                    }
                }
            }

            foreach (string result in results)
            {
                listView1.Items.Add(result); // CUTTING DOWN ON UI UPDATES
            }

            label7.Text = listView1.Items.Count.ToString();

            DialogResult dialogResult = MessageBox.Show("SCAN COMPLETE.");

            if (dialogResult == DialogResult.OK)
            {
                progressBar1.Value = 0;
            }
        }

        public void NextScan(object valueToFind, ListView listView1)
        {
            listView1.Clear();

            Dictionary<long, object> keyValuePairs2 = new Dictionary<long, object>();

            List<string> results = new List<string>();

            foreach (var pair in keyValuePairs)
            {
                long address = pair.Key; // GET THE ADDRESS FROM THE CURRENT PAIR
                long valueAtAddress = 0;

                if (valueType == "String")
                {
                    // valueAtAddress = ReadString(address);
                }

                if (valueType == "4 Byte Big Endian")
                {
                    valueAtAddress = Read4ByteBigEndian(address); // THIS SHOULD CONTAIN THE CHANGED NUMBER
                }

                if (valueType == "2 Byte Big Endian")
                {
                    valueAtAddress = Read2ByteBigEndian(address);
                }

                // IF THE VALUE AT THE ADDRESS EQUALS THE VALUE TO FIND

                if (valueAtAddress.Equals(Convert.ToInt32(valueToFind)))
                {
                    results.Add($"ADDRESS: {address:X}, VALUE: {valueAtAddress}");
                    keyValuePairs2[address] = valueAtAddress; // ADD THE CURRENT ADDRESS AND VALUE TO THE CURRENT SCAN RESULTS
                }

                label7.Text = listView1.Items.Count.ToString();

                keyValuePairs = keyValuePairs2; // UPDATE THE PREVIOUS SCAN WITH THE CURRENT SCAN SO THAT NEXT SCAN IS READY AGAIN
            }

            foreach (string result in results)
            {
                listView1.Items.Add(result);
            }
        }

        public void PointerScan(long start, long end, long address)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = (int)(end - start + 1);
            progressBar1.Value = 0;

            listView1.Clear();

            List<Tuple<long, int>> pointers = new List<Tuple<long, int>>();

            int range = (int)(end - start);

            for (int i = 0; i < range; i += 4)
            {
                long valueAtAddress = Read4ByteBigEndian(start + (long)i);

                int distance = (int)(valueAtAddress - address); // THE DISTANCE IN BYTES

                // IF DISTANCE IS NEGATIVE, CHANGE TO A ABSOLUTE VALUE
                if (distance < 0)
                {
                    distance *= -1;
                }

                // IF THE DIFFERENCE IS LESS THAN OR EQUAL TO 0xFFFF (65535), CONSIDER IT A POTENTIAL POINTER
                if (distance <= 0xFFFF)
                {
                    pointers.Add(new Tuple<long, int>(start + (long)i, distance));
                }
            }

            foreach (var pointer in pointers)
            {
                string result = $"ADDRESS: {pointer.Item1:X}, DIFFERENCE: {pointer.Item2}";
                listView1.Items.Add(result);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            long start, end;

            if (long.TryParse(textBox2.Text, System.Globalization.NumberStyles.HexNumber, null, out start) &&
                long.TryParse(textBox3.Text, System.Globalization.NumberStyles.HexNumber, null, out end))
            {

                FirstScan(start, end, textBox1.Text, comboBox1.SelectedItem.ToString(), listView1, progressBar1);

            }

            textBox1.SelectAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NextScan(textBox1.Text, listView1);

            textBox1.SelectAll();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("DO YOU WISH TO CLEAR THE CURRENT ADDRESS LIST?", "CLEAR ADDRESS LIST", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                listView2.Clear();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];

                listView2.Items.Add((ListViewItem)selectedItem.Clone());
            }
        }

        private void listView2_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem selectedItem = listView2.SelectedItems[0];

            string value = Microsoft.VisualBasic.Interaction.InputBox("ENTER A VALUE TO WRITE TO THE ADDRESS", "WRITE PROCESS MEMORY", "");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            long start, end;

            long str = Convert.ToInt64(textBox4.Text, 16);

            if (long.TryParse(textBox2.Text, System.Globalization.NumberStyles.HexNumber, null, out start) &&
                long.TryParse(textBox3.Text, System.Globalization.NumberStyles.HexNumber, null, out end))
            {
                PointerScan(start, end, str);
            }
        }
    }
}

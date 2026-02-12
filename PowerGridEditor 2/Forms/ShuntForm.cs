using System;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class ShuntForm : Form
    {
        public Shunt MyShunt { get; private set; }
        private readonly string[] keys = { "Start", "R", "X" };

        public TextBox StartNodeTextBox => paramBoxes[0];
        public TextBox ActiveResistanceTextBox => paramBoxes[1];
        public TextBox ReactiveResistanceTextBox => paramBoxes[2];

        public ShuntForm()
        {
            InitializeComponent();
            MyShunt = new Shunt(0);

            buttonSave.Click += (s, e) => SaveData();
            buttonCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnCheckIP.Click += async (s, e) => await RunPing();

            LoadData();
        }

        private void LoadData()
        {
            var inv = CultureInfo.InvariantCulture;
            paramBoxes[0].Text = MyShunt.StartNodeNumber.ToString(inv);
            paramBoxes[1].Text = MyShunt.ActiveResistance.ToString(inv);
            paramBoxes[2].Text = MyShunt.ReactiveResistance.ToString(inv);

            for (int i = 1; i < 3; i++)
            {
                if (MyShunt.ParamAutoModes.ContainsKey(keys[i])) paramChecks[i].Checked = MyShunt.ParamAutoModes[keys[i]];
                if (MyShunt.ParamRegisters.ContainsKey(keys[i])) addrBoxes[i].Text = MyShunt.ParamRegisters[keys[i]];
            }

            textBoxIP.Text = MyShunt.IPAddress;
            textBoxPort.Text = MyShunt.Port;
            textBoxID.Text = MyShunt.DeviceID;
            comboBoxProtocol.SelectedItem = MyShunt.Protocol;
            if (comboBoxProtocol.SelectedIndex < 0) comboBoxProtocol.SelectedIndex = 0;
        }

        private void SaveData()
        {
            try
            {
                var inv = CultureInfo.InvariantCulture;
                MyShunt.StartNodeNumber = int.Parse(paramBoxes[0].Text);
                MyShunt.ActiveResistance = double.Parse(paramBoxes[1].Text.Replace(',', '.'), inv);
                MyShunt.ReactiveResistance = double.Parse(paramBoxes[2].Text.Replace(',', '.'), inv);

                for (int i = 1; i < 3; i++)
                {
                    MyShunt.ParamAutoModes[keys[i]] = paramChecks[i].Checked;
                    MyShunt.ParamRegisters[keys[i]] = addrBoxes[i].Text;
                }

                MyShunt.Protocol = comboBoxProtocol.Text;
                MyShunt.IPAddress = textBoxIP.Text;
                MyShunt.Port = textBoxPort.Text;
                MyShunt.DeviceID = textBoxID.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch
            {
                MessageBox.Show("Ошибка в числовых полях!");
            }
        }

        private async System.Threading.Tasks.Task RunPing()
        {
            try
            {
                using (Ping p = new Ping())
                {
                    var reply = await p.SendPingAsync(textBoxIP.Text, 1000);
                    textBoxIP.BackColor = reply.Status == IPStatus.Success ? Color.LightGreen : Color.LightPink;
                }
            }
            catch
            {
                textBoxIP.BackColor = Color.LightPink;
            }
        }
    }
}

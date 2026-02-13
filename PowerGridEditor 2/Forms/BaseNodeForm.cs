using System;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class BaseNodeForm : Form
    {
        public BaseNode MyBaseNode { get; private set; }
        private readonly string[] keys = { "Number", "U", "P", "Q", "Pg", "Qg", "Uf", "Qmin", "Qmax" };
        private NumericUpDown numericMeasurementInterval;
        private NumericUpDown numericIncrementStep;
        private NumericUpDown numericIncrementInterval;

        public TextBox NodeNumberTextBox => paramBoxes[0];
        public TextBox InitialVoltageTextBox => paramBoxes[1];
        public TextBox NominalActivePowerTextBox => paramBoxes[2];
        public TextBox NominalReactivePowerTextBox => paramBoxes[3];
        public TextBox ActivePowerGenerationTextBox => paramBoxes[4];
        public TextBox ReactivePowerGenerationTextBox => paramBoxes[5];
        public TextBox FixedVoltageModuleTextBox => paramBoxes[6];
        public TextBox MinReactivePowerTextBox => paramBoxes[7];
        public TextBox MaxReactivePowerTextBox => paramBoxes[8];

        public BaseNodeForm()
        {
            InitializeComponent();
            MyBaseNode = new BaseNode(0);

            buttonSave.Click += (s, e) => SaveData();
            buttonCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnCheckIP.Click += async (s, e) => await RunPing();

            SetupExtendedConnectionSettings();

            LoadData();
        }

        private void SetupExtendedConnectionSettings()
        {
            int sy = 150;
            tabSettings.Controls.Add(new Label { Text = "Интервал измерения (сек):", Location = new Point(15, sy + 3), Size = new Size(170, 20) });
            numericMeasurementInterval = new NumericUpDown { Location = new Point(190, sy), Size = new Size(80, 23), Minimum = 1, Maximum = 3600 };
            tabSettings.Controls.Add(numericMeasurementInterval);

            sy += 30;
            tabSettings.Controls.Add(new Label { Text = "Шаг изменения:", Location = new Point(15, sy + 3), Size = new Size(170, 20) });
            numericIncrementStep = new NumericUpDown { Location = new Point(190, sy), Size = new Size(80, 23), DecimalPlaces = 2, Minimum = -100000, Maximum = 100000 };
            tabSettings.Controls.Add(numericIncrementStep);

            sy += 30;
            tabSettings.Controls.Add(new Label { Text = "Интервал инкремента (сек):", Location = new Point(15, sy + 3), Size = new Size(170, 20) });
            numericIncrementInterval = new NumericUpDown { Location = new Point(190, sy), Size = new Size(80, 23), Minimum = 1, Maximum = 3600 };
            tabSettings.Controls.Add(numericIncrementInterval);
        }

        private void LoadData()
        {
            var inv = CultureInfo.InvariantCulture;
            double[] vals = {
                MyBaseNode.Number, MyBaseNode.InitialVoltage, MyBaseNode.NominalActivePower,
                MyBaseNode.NominalReactivePower, MyBaseNode.ActivePowerGeneration, MyBaseNode.ReactivePowerGeneration,
                MyBaseNode.FixedVoltageModule, MyBaseNode.MinReactivePower, MyBaseNode.MaxReactivePower
            };

            for (int i = 0; i < vals.Length; i++)
            {
                paramBoxes[i].Text = vals[i].ToString(inv);
                if (i > 0)
                {
                    if (MyBaseNode.ParamAutoModes.ContainsKey(keys[i])) paramChecks[i].Checked = MyBaseNode.ParamAutoModes[keys[i]];
                    if (MyBaseNode.ParamRegisters.ContainsKey(keys[i])) addrBoxes[i].Text = MyBaseNode.ParamRegisters[keys[i]];
                }
            }

            textBoxIP.Text = MyBaseNode.IPAddress;
            textBoxPort.Text = MyBaseNode.Port;
            textBoxID.Text = MyBaseNode.DeviceID;
            comboBoxProtocol.SelectedItem = MyBaseNode.Protocol;
            if (comboBoxProtocol.SelectedIndex < 0) comboBoxProtocol.SelectedIndex = 0;
            numericMeasurementInterval.Value = MyBaseNode.MeasurementIntervalSeconds;
            numericIncrementStep.Value = (decimal)MyBaseNode.IncrementStep;
            numericIncrementInterval.Value = MyBaseNode.IncrementIntervalSeconds;
        }

        private void SaveData()
        {
            try
            {
                var inv = CultureInfo.InvariantCulture;
                MyBaseNode.Number = int.Parse(paramBoxes[0].Text);
                MyBaseNode.InitialVoltage = double.Parse(paramBoxes[1].Text.Replace(',', '.'), inv);
                MyBaseNode.NominalActivePower = double.Parse(paramBoxes[2].Text.Replace(',', '.'), inv);
                MyBaseNode.NominalReactivePower = double.Parse(paramBoxes[3].Text.Replace(',', '.'), inv);
                MyBaseNode.ActivePowerGeneration = double.Parse(paramBoxes[4].Text.Replace(',', '.'), inv);
                MyBaseNode.ReactivePowerGeneration = double.Parse(paramBoxes[5].Text.Replace(',', '.'), inv);
                MyBaseNode.FixedVoltageModule = double.Parse(paramBoxes[6].Text.Replace(',', '.'), inv);
                MyBaseNode.MinReactivePower = double.Parse(paramBoxes[7].Text.Replace(',', '.'), inv);
                MyBaseNode.MaxReactivePower = double.Parse(paramBoxes[8].Text.Replace(',', '.'), inv);

                for (int i = 1; i < 9; i++)
                {
                    MyBaseNode.ParamAutoModes[keys[i]] = paramChecks[i].Checked;
                    MyBaseNode.ParamRegisters[keys[i]] = addrBoxes[i].Text;
                }

                MyBaseNode.Protocol = comboBoxProtocol.Text;
                MyBaseNode.IPAddress = textBoxIP.Text;
                MyBaseNode.Port = textBoxPort.Text;
                MyBaseNode.DeviceID = textBoxID.Text;
                MyBaseNode.MeasurementIntervalSeconds = (int)numericMeasurementInterval.Value;
                MyBaseNode.IncrementStep = (double)numericIncrementStep.Value;
                MyBaseNode.IncrementIntervalSeconds = (int)numericIncrementInterval.Value;

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

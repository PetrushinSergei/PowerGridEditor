using System;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public partial class BranchForm : Form
    {
        public Branch MyBranch { get; private set; }
        private readonly string[] keys = { "Start", "End", "R", "X", "B", "Ktr", "G" };
        private NumericUpDown numericMeasurementInterval;
        private NumericUpDown numericIncrementStep;
        private NumericUpDown numericIncrementInterval;

        public TextBox StartNodeTextBox => paramBoxes[0];
        public TextBox EndNodeTextBox => paramBoxes[1];
        public TextBox ActiveResistanceTextBox => paramBoxes[2];
        public TextBox ReactiveResistanceTextBox => paramBoxes[3];
        public TextBox ReactiveConductivityTextBox => paramBoxes[4];
        public TextBox TransformationRatioTextBox => paramBoxes[5];
        public TextBox ActiveConductivityTextBox => paramBoxes[6];

        public BranchForm()
        {
            InitializeComponent();
            MyBranch = new Branch(0, 0);

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
            paramBoxes[0].Text = MyBranch.StartNodeNumber.ToString(inv);
            paramBoxes[1].Text = MyBranch.EndNodeNumber.ToString(inv);
            paramBoxes[2].Text = MyBranch.ActiveResistance.ToString(inv);
            paramBoxes[3].Text = MyBranch.ReactiveResistance.ToString(inv);
            paramBoxes[4].Text = MyBranch.ReactiveConductivity.ToString(inv);
            paramBoxes[5].Text = MyBranch.TransformationRatio.ToString(inv);
            paramBoxes[6].Text = MyBranch.ActiveConductivity.ToString(inv);

            for (int i = 2; i < 7; i++)
            {
                if (MyBranch.ParamAutoModes.ContainsKey(keys[i])) paramChecks[i].Checked = MyBranch.ParamAutoModes[keys[i]];
                if (MyBranch.ParamRegisters.ContainsKey(keys[i])) addrBoxes[i].Text = MyBranch.ParamRegisters[keys[i]];
            }

            textBoxIP.Text = MyBranch.IPAddress;
            textBoxPort.Text = MyBranch.Port;
            textBoxID.Text = MyBranch.DeviceID;
            comboBoxProtocol.SelectedItem = MyBranch.Protocol;
            if (comboBoxProtocol.SelectedIndex < 0) comboBoxProtocol.SelectedIndex = 0;
            numericMeasurementInterval.Value = MyBranch.MeasurementIntervalSeconds;
            numericIncrementStep.Value = (decimal)MyBranch.IncrementStep;
            numericIncrementInterval.Value = MyBranch.IncrementIntervalSeconds;
        }

        private void SaveData()
        {
            try
            {
                var inv = CultureInfo.InvariantCulture;
                MyBranch.StartNodeNumber = int.Parse(paramBoxes[0].Text);
                MyBranch.EndNodeNumber = int.Parse(paramBoxes[1].Text);
                MyBranch.ActiveResistance = double.Parse(paramBoxes[2].Text.Replace(',', '.'), inv);
                MyBranch.ReactiveResistance = double.Parse(paramBoxes[3].Text.Replace(',', '.'), inv);
                MyBranch.ReactiveConductivity = double.Parse(paramBoxes[4].Text.Replace(',', '.'), inv);
                MyBranch.TransformationRatio = double.Parse(paramBoxes[5].Text.Replace(',', '.'), inv);
                MyBranch.ActiveConductivity = double.Parse(paramBoxes[6].Text.Replace(',', '.'), inv);

                for (int i = 2; i < 7; i++)
                {
                    MyBranch.ParamAutoModes[keys[i]] = paramChecks[i].Checked;
                    MyBranch.ParamRegisters[keys[i]] = addrBoxes[i].Text;
                }

                MyBranch.Protocol = comboBoxProtocol.Text;
                MyBranch.IPAddress = textBoxIP.Text;
                MyBranch.Port = textBoxPort.Text;
                MyBranch.DeviceID = textBoxID.Text;
                MyBranch.MeasurementIntervalSeconds = (int)numericMeasurementInterval.Value;
                MyBranch.IncrementStep = (double)numericIncrementStep.Value;
                MyBranch.IncrementIntervalSeconds = (int)numericIncrementInterval.Value;

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

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
        private NumericUpDown numericMeasurementInterval;
        private TextBox[] incrementStepBoxes;
        private TextBox[] incrementIntervalBoxes;

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

            SetupExtendedConnectionSettings();
            SetupParameterIncrementEditors();

            LoadData();
        }

        private void SetupExtendedConnectionSettings()
        {
            int sy = 150;
            tabSettings.Controls.Add(new Label { Text = "Интервал измерения (сек):", Location = new Point(15, sy + 3), Size = new Size(170, 20) });
            numericMeasurementInterval = new NumericUpDown { Location = new Point(190, sy), Size = new Size(80, 23), Minimum = 1, Maximum = 3600 };
            tabSettings.Controls.Add(numericMeasurementInterval);
        }

        private void SetupParameterIncrementEditors()
        {
            incrementStepBoxes = new TextBox[3];
            incrementIntervalBoxes = new TextBox[3];

            tabParams.Controls.Add(new Label { Text = "Шаг:", Location = new Point(520, 2), Size = new Size(45, 18) });
            tabParams.Controls.Add(new Label { Text = "Инт.,с:", Location = new Point(585, 2), Size = new Size(55, 18) });

            for (int i = 1; i < 3; i++)
            {
                var stepBox = new TextBox { Size = new Size(55, 23), Text = "1" };
                var intervalBox = new TextBox { Size = new Size(50, 23), Text = "2" };
                if (addrBoxes[i] != null)
                {
                    stepBox.Location = new Point(addrBoxes[i].Right + 10, addrBoxes[i].Top);
                    intervalBox.Location = new Point(stepBox.Right + 8, addrBoxes[i].Top);
                }
                incrementStepBoxes[i] = stepBox;
                incrementIntervalBoxes[i] = intervalBox;
                tabParams.Controls.Add(stepBox);
                tabParams.Controls.Add(intervalBox);
            }
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
            numericMeasurementInterval.Value = MyShunt.MeasurementIntervalSeconds;
            for (int i = 1; i < 3; i++)
            {
                if (MyShunt.ParamIncrementSteps.ContainsKey(keys[i])) incrementStepBoxes[i].Text = MyShunt.ParamIncrementSteps[keys[i]].ToString(inv);
                if (MyShunt.ParamIncrementIntervals.ContainsKey(keys[i])) incrementIntervalBoxes[i].Text = MyShunt.ParamIncrementIntervals[keys[i]].ToString(inv);
            }
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
                MyShunt.MeasurementIntervalSeconds = (int)numericMeasurementInterval.Value;
                for (int i = 1; i < 3; i++)
                {
                    if (double.TryParse(incrementStepBoxes[i].Text.Replace(',', '.'), NumberStyles.Any, inv, out double step))
                        MyShunt.ParamIncrementSteps[keys[i]] = step;
                    if (int.TryParse(incrementIntervalBoxes[i].Text, out int interval))
                        MyShunt.ParamIncrementIntervals[keys[i]] = Math.Max(1, interval);
                }

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

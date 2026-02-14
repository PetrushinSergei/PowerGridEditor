using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Sockets;
using NModbus;

namespace PowerGridEditor
{
    public partial class NodeForm : Form
    {
        public Node MyNode { get; private set; }
        private Timer liveTimer;
        private string[] keys = { "Number", "U", "P", "Q", "Pg", "Qg", "Uf", "Qmin", "Qmax" };
        private NumericUpDown numericMeasurementInterval;
        private TextBox[] incrementStepBoxes;
        private TextBox[] incrementIntervalBoxes;
        private bool suppressTelemetryUiEvents;
        private readonly Dictionary<Control, Point> fixedParamLocations = new Dictionary<Control, Point>();
        public event EventHandler TelemetryUpdated;

        public NodeForm(Node node)
        {
            InitializeComponent(); // Все контроллеры создаются здесь (в Designer.cs)
            MyNode = node;

            liveTimer = new Timer { Interval = 2000 };
            liveTimer.Tick += async (s, e) => await PollModbusTask();
            liveTimer.Start();

            // Привязка событий (контроллеры берутся из Designer)
            btnCheckIP.Click += async (s, e) => await RunPing();
            buttonSave.Click += (s, e) => SaveData();
            buttonCancel.Click += (s, e) => this.Close();
            this.FormClosing += (s, e) => liveTimer.Stop();

            SetupParameterIncrementEditors();
            CaptureFixedParamLayout();
            WireTelemetryCheckboxes();
            WireNumericInputGuards();

            LoadData();
        }

        private void WireTelemetryCheckboxes()
        {
            for (int i = 1; i < paramChecks.Length; i++)
            {
                int idx = i;
                if (paramChecks[idx] != null)
                {
                    paramChecks[idx].CheckedChanged += (s, e) =>
                    {
                        if (suppressTelemetryUiEvents) return;
                        ApplyTelemetryState(idx, paramChecks[idx]);
                    };
                }
            }
        }

        private void WireNumericInputGuards()
        {
            KeyPressEventHandler decimalGuard = (s, e) =>
            {
                char c = e.KeyChar;
                if (char.IsControl(c)) return;
                if (char.IsDigit(c)) return;
                if (c == '-' || c == ',' || c == '.') return;
                e.Handled = true;
            };

            KeyPressEventHandler intGuard = (s, e) =>
            {
                char c = e.KeyChar;
                if (char.IsControl(c) || char.IsDigit(c)) return;
                e.Handled = true;
            };

            for (int i = 0; i < paramBoxes.Length; i++)
            {
                if (paramBoxes[i] != null) paramBoxes[i].KeyPress += decimalGuard;
                if (i > 0 && addrBoxes[i] != null) addrBoxes[i].KeyPress += intGuard;
                if (i > 0 && incrementIntervalBoxes != null && incrementIntervalBoxes[i] != null) incrementIntervalBoxes[i].KeyPress += intGuard;
                if (i > 0 && incrementStepBoxes != null && incrementStepBoxes[i] != null) incrementStepBoxes[i].KeyPress += decimalGuard;
            }

            textBoxPort.KeyPress += intGuard;
            textBoxID.KeyPress += intGuard;
        }

        private void ApplyTelemetryState(int index, Control focusTarget)
        {
            if (index <= 0 || index >= paramBoxes.Length || paramChecks[index] == null) return;

            TextBox targetBox = paramBoxes[index];
            bool telemetryOn = paramChecks[index].Checked;

            targetBox.ReadOnly = telemetryOn;
            targetBox.BackColor = telemetryOn ? SystemColors.Control : SystemColors.Window;
            EnsureFixedParamLayout();

            if (focusTarget == null) return;

            var target = focusTarget;
            BeginInvoke(new Action(() =>
            {
                if (target != null && target.CanFocus)
                {
                    target.Focus();
                }
                else if (targetBox.CanFocus)
                {
                    targetBox.Focus();
                }
            }));
        }

        private void CaptureFixedParamLayout()
        {
            fixedParamLocations.Clear();
            foreach (Control control in tabParams.Controls)
            {
                fixedParamLocations[control] = control.Location;
            }
        }

        private void EnsureFixedParamLayout()
        {
            if (fixedParamLocations.Count == 0) return;

            tabParams.SuspendLayout();
            foreach (var pair in fixedParamLocations)
            {
                var control = pair.Key;
                if (control != null && !control.IsDisposed)
                {
                    control.Location = pair.Value;
                }
            }
            tabParams.ResumeLayout(false);
        }

        private void SetupParameterIncrementEditors()
        {
            incrementStepBoxes = new TextBox[9];
            incrementIntervalBoxes = new TextBox[9];

            tabParams.Controls.Add(new Label { Text = "Шаг:", Location = new Point(520, 2), Size = new Size(45, 18) });
            tabParams.Controls.Add(new Label { Text = "Инт.,с:", Location = new Point(585, 2), Size = new Size(55, 18) });

            for (int i = 1; i < 9; i++)
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
            double[] vals = {
                MyNode.Number, MyNode.InitialVoltage, MyNode.NominalActivePower,
                MyNode.NominalReactivePower, MyNode.ActivePowerGeneration, MyNode.ReactivePowerGeneration,
                MyNode.FixedVoltageModule, MyNode.MinReactivePower, MyNode.MaxReactivePower
            };

            suppressTelemetryUiEvents = true;
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    paramBoxes[i].Text = vals[i].ToString(inv);
                    // Проверяем на null, так как для i=0 чекбокс и адрес не создавались
                    if (i > 0 && paramChecks[i] != null)
                    {
                        if (MyNode.ParamAutoModes.ContainsKey(keys[i]))
                            paramChecks[i].Checked = MyNode.ParamAutoModes[keys[i]];
                        if (MyNode.ParamRegisters.ContainsKey(keys[i]))
                            addrBoxes[i].Text = MyNode.ParamRegisters[keys[i]];
                    }
                }
            }
            finally
            {
                suppressTelemetryUiEvents = false;
            }

            for (int i = 1; i < 9; i++)
            {
                ApplyTelemetryState(i, null);
            }

            textBoxIP.Text = MyNode.IPAddress;
            textBoxPort.Text = MyNode.Port;
            textBoxID.Text = MyNode.NodeID;
            comboBoxProtocol.SelectedItem = MyNode.Protocol;
            numericMeasurementInterval.Value = MyNode.MeasurementIntervalSeconds;

            for (int i = 1; i < 9; i++)
            {
                if (MyNode.ParamIncrementSteps.ContainsKey(keys[i])) incrementStepBoxes[i].Text = MyNode.ParamIncrementSteps[keys[i]].ToString(inv);
                if (MyNode.ParamIncrementIntervals.ContainsKey(keys[i])) incrementIntervalBoxes[i].Text = MyNode.ParamIncrementIntervals[keys[i]].ToString(inv);
            }
        }

        private async Task PollModbusTask()
        {
            if (string.IsNullOrWhiteSpace(textBoxIP.Text)) return;

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync(textBoxIP.Text, int.Parse(textBoxPort.Text));
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) != connectTask) return;

                    var factory = new ModbusFactory();
                    IModbusMaster master = factory.CreateMaster(client);
                    byte slaveId = byte.Parse(textBoxID.Text);

                    for (int i = 1; i < 9; i++) // Начинаем с 1
                    {
                        if (paramChecks[i] != null && paramChecks[i].Checked)
                        {
                            ushort addr;
                            if (ushort.TryParse(addrBoxes[i].Text, out addr))
                            {
                                ushort[] res = await master.ReadHoldingRegistersAsync(slaveId, addr, 1);
                                paramBoxes[i].Text = res[0].ToString();
                                paramBoxes[i].ReadOnly = true;

                                double value = res[0];
                                if (i == 1) MyNode.InitialVoltage = value;
                                else if (i == 2) MyNode.NominalActivePower = value;
                                else if (i == 3) MyNode.NominalReactivePower = value;
                                else if (i == 4) MyNode.ActivePowerGeneration = value;
                                else if (i == 5) MyNode.ReactivePowerGeneration = value;
                                else if (i == 6) MyNode.FixedVoltageModule = value;
                                else if (i == 7) MyNode.MinReactivePower = value;
                                else if (i == 8) MyNode.MaxReactivePower = value;
                            }
                        }
                        else
                        {
                            paramBoxes[i].ReadOnly = false;
                            paramBoxes[i].BackColor = SystemColors.Window;
                        }
                    }
                }

                TelemetryUpdated?.Invoke(this, EventArgs.Empty);
            }
            catch { /* Ошибка связи */ }
        }

        private void SaveData()
        {
            try
            {
                var inv = CultureInfo.InvariantCulture;
                MyNode.Number = int.Parse(paramBoxes[0].Text);
                MyNode.InitialVoltage = double.Parse(paramBoxes[1].Text.Replace(',', '.'), inv);
                MyNode.NominalActivePower = double.Parse(paramBoxes[2].Text.Replace(',', '.'), inv);
                MyNode.NominalReactivePower = double.Parse(paramBoxes[3].Text.Replace(',', '.'), inv);
                MyNode.ActivePowerGeneration = double.Parse(paramBoxes[4].Text.Replace(',', '.'), inv);
                MyNode.ReactivePowerGeneration = double.Parse(paramBoxes[5].Text.Replace(',', '.'), inv);
                MyNode.FixedVoltageModule = double.Parse(paramBoxes[6].Text.Replace(',', '.'), inv);
                MyNode.MinReactivePower = double.Parse(paramBoxes[7].Text.Replace(',', '.'), inv);
                MyNode.MaxReactivePower = double.Parse(paramBoxes[8].Text.Replace(',', '.'), inv);

                for (int i = 1; i < 9; i++)
                {
                    if (paramChecks[i] != null)
                    {
                        MyNode.ParamAutoModes[keys[i]] = paramChecks[i].Checked;
                        MyNode.ParamRegisters[keys[i]] = addrBoxes[i].Text;
                    }
                }

                MyNode.IPAddress = textBoxIP.Text;
                MyNode.Port = textBoxPort.Text;
                MyNode.NodeID = textBoxID.Text;
                MyNode.Protocol = comboBoxProtocol.Text;
                MyNode.MeasurementIntervalSeconds = (int)numericMeasurementInterval.Value;

                for (int i = 1; i < 9; i++)
                {
                    if (double.TryParse(incrementStepBoxes[i].Text.Replace(',', '.'), NumberStyles.Any, inv, out double step))
                        MyNode.ParamIncrementSteps[keys[i]] = step;
                    if (int.TryParse(incrementIntervalBoxes[i].Text, out int interval))
                        MyNode.ParamIncrementIntervals[keys[i]] = Math.Max(1, interval);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch { MessageBox.Show("Ошибка в числовых полях!"); }
        }

        private async Task RunPing()
        {
            try
            {
                using (Ping p = new Ping())
                {
                    var reply = await p.SendPingAsync(textBoxIP.Text, 1000);
                    textBoxIP.BackColor = (reply.Status == IPStatus.Success) ? Color.LightGreen : Color.LightPink;
                }
            }
            catch { textBoxIP.BackColor = Color.LightPink; }
        }
    }
}

using System;
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

            LoadData();
        }

        private void LoadData()
        {
            var inv = CultureInfo.InvariantCulture;
            double[] vals = {
                MyNode.Number, MyNode.InitialVoltage, MyNode.NominalActivePower,
                MyNode.NominalReactivePower, MyNode.ActivePowerGeneration, MyNode.ReactivePowerGeneration,
                MyNode.FixedVoltageModule, MyNode.MinReactivePower, MyNode.MaxReactivePower
            };

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

            textBoxIP.Text = MyNode.IPAddress;
            textBoxPort.Text = MyNode.Port;
            textBoxID.Text = MyNode.NodeID;
            comboBoxProtocol.SelectedItem = MyNode.Protocol;
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
                            }
                        }
                        else
                        {
                            paramBoxes[i].ReadOnly = false;
                        }
                    }
                }
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
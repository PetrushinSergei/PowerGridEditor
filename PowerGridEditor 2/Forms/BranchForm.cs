using System;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Linq;

namespace PowerGridEditor
{
    public partial class BranchForm : Form
    {
        public Branch MyBranch { get; private set; }
        private readonly string[] keys = { "Start", "End", "R", "X", "B", "Ktr", "G", "Imax" };
        private NumericUpDown numericMeasurementInterval;
        private Timer modelSyncTimer;
        private TextBox[] incrementStepBoxes;
        private TextBox[] incrementIntervalBoxes;
        private Button[] incrementToggleButtons;
        private ComboBox comboStartNode;
        private ComboBox comboEndNode;

        public TextBox StartNodeTextBox => paramBoxes[0];
        public TextBox EndNodeTextBox => paramBoxes[1];
        public TextBox ActiveResistanceTextBox => paramBoxes[2];
        public TextBox ReactiveResistanceTextBox => paramBoxes[3];
        public TextBox ReactiveConductivityTextBox => paramBoxes[4];
        public TextBox TransformationRatioTextBox => paramBoxes[5];
        public TextBox ActiveConductivityTextBox => paramBoxes[6];
        public TextBox PermissibleCurrentTextBox => paramBoxes[7];


        public void BindBranchModel(Branch branch)
        {
            MyBranch = branch;
            LoadData();
        }

        public BranchForm()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(245, 250, 255);
            tabParams.BackColor = Color.FromArgb(245, 250, 255);
            tabSettings.BackColor = Color.FromArgb(245, 250, 255);
            MyBranch = new Branch(0, 0);

            buttonSave.Click += (s, e) => SaveData();
            buttonCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnCheckIP.Click += async (s, e) => await RunPing();

            SetupParameterIncrementEditors();
            SetupNodeSelectors();
            ApplyBoldFonts(this);

            modelSyncTimer = new Timer { Interval = 700 };
            modelSyncTimer.Tick += (s, e) =>
            {
                if (ContainsFocus && ActiveControl is TextBox) return;
                RefreshFromModel();
            };
            modelSyncTimer.Start();
            FormClosing += (s, e) => modelSyncTimer.Stop();

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
            incrementStepBoxes = new TextBox[8];
            incrementIntervalBoxes = new TextBox[8];
            incrementToggleButtons = new Button[8];

            tabParams.Controls.Add(new Label { Text = "Шаг:", Location = new Point(520, 2), Size = new Size(45, 18) });
            tabParams.Controls.Add(new Label { Text = "Инт.,с:", Location = new Point(585, 2), Size = new Size(55, 18) });
            tabParams.Controls.Add(new Label { Text = "Авто изм.", Location = new Point(650, 2), Size = new Size(80, 18) });

            for (int i = 2; i < 8; i++)
            {
                var stepBox = new TextBox { Size = new Size(55, 23), Text = "1" };
                var intervalBox = new TextBox { Size = new Size(50, 23), Text = "2" };
                if (addrBoxes[i] != null)
                {
                    stepBox.Location = new Point(addrBoxes[i].Right + 10, addrBoxes[i].Top);
                    intervalBox.Location = new Point(stepBox.Right + 8, addrBoxes[i].Top);
                }
                var toggleButton = new Button { Size = new Size(75, 23), Text = "Старт" };
                toggleButton.Location = new Point(intervalBox.Right + 8, addrBoxes[i].Top);
                int idx = i;
                toggleButton.Click += (s, e) => ToggleIncrement(idx);

                incrementStepBoxes[i] = stepBox;
                incrementIntervalBoxes[i] = intervalBox;
                incrementToggleButtons[i] = toggleButton;
                tabParams.Controls.Add(stepBox);
                tabParams.Controls.Add(intervalBox);
                tabParams.Controls.Add(toggleButton);
            }
        }

        public void SetAvailableNodeNumbers(System.Collections.Generic.IEnumerable<int> nodeNumbers)
        {
            if (comboStartNode == null || comboEndNode == null)
            {
                return;
            }

            var items = nodeNumbers
                .Where(x => x > 0)
                .Distinct()
                .OrderBy(x => x)
                .Cast<object>()
                .ToArray();

            comboStartNode.Items.Clear();
            comboEndNode.Items.Clear();
            comboStartNode.Items.AddRange(items);
            comboEndNode.Items.AddRange(items);

            if (MyBranch.StartNodeNumber > 0 && comboStartNode.Items.Contains(MyBranch.StartNodeNumber))
            {
                comboStartNode.SelectedItem = MyBranch.StartNodeNumber;
            }

            if (MyBranch.EndNodeNumber > 0 && comboEndNode.Items.Contains(MyBranch.EndNodeNumber))
            {
                comboEndNode.SelectedItem = MyBranch.EndNodeNumber;
            }
        }

        private void SetupNodeSelectors()
        {
            comboStartNode = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = paramBoxes[0].Location,
                Size = paramBoxes[0].Size,
                Font = paramBoxes[0].Font
            };

            comboEndNode = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = paramBoxes[1].Location,
                Size = paramBoxes[1].Size,
                Font = paramBoxes[1].Font
            };

            tabParams.Controls.Add(comboStartNode);
            tabParams.Controls.Add(comboEndNode);
            comboStartNode.BringToFront();
            comboEndNode.BringToFront();
            paramBoxes[0].Visible = false;
            paramBoxes[1].Visible = false;

            comboStartNode.SelectedIndexChanged += (s, e) =>
            {
                if (comboStartNode.SelectedItem != null)
                {
                    paramBoxes[0].Text = comboStartNode.SelectedItem.ToString();
                }
            };

            comboEndNode.SelectedIndexChanged += (s, e) =>
            {
                if (comboEndNode.SelectedItem != null)
                {
                    paramBoxes[1].Text = comboEndNode.SelectedItem.ToString();
                }
            };
        }

        public void RefreshFromModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var inv = CultureInfo.InvariantCulture;
            paramBoxes[0].Text = MyBranch.StartNodeNumber.ToString(inv);
            paramBoxes[1].Text = MyBranch.EndNodeNumber.ToString(inv);
            if (comboStartNode != null && comboStartNode.Items.Contains(MyBranch.StartNodeNumber)) comboStartNode.SelectedItem = MyBranch.StartNodeNumber;
            if (comboEndNode != null && comboEndNode.Items.Contains(MyBranch.EndNodeNumber)) comboEndNode.SelectedItem = MyBranch.EndNodeNumber;
            paramBoxes[2].Text = MyBranch.ActiveResistance.ToString(inv);
            paramBoxes[3].Text = MyBranch.ReactiveResistance.ToString(inv);
            paramBoxes[4].Text = MyBranch.ReactiveConductivity.ToString(inv);
            paramBoxes[5].Text = MyBranch.TransformationRatio.ToString(inv);
            paramBoxes[6].Text = MyBranch.ActiveConductivity.ToString(inv);
            paramBoxes[7].Text = MyBranch.PermissibleCurrent.ToString(inv);

            for (int i = 2; i < 8; i++)
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
            for (int i = 2; i < 8; i++)
            {
                if (MyBranch.ParamIncrementSteps.ContainsKey(keys[i])) incrementStepBoxes[i].Text = MyBranch.ParamIncrementSteps[keys[i]].ToString(inv);
                if (MyBranch.ParamIncrementIntervals.ContainsKey(keys[i])) incrementIntervalBoxes[i].Text = MyBranch.ParamIncrementIntervals[keys[i]].ToString(inv);
                UpdateIncrementButtonState(i);
            }
        }


        private void ToggleIncrement(int index)
        {
            var inv = CultureInfo.InvariantCulture;
            double step = 1;
            int interval = 2;
            if (double.TryParse(incrementStepBoxes[index].Text.Replace(',', '.'), NumberStyles.Any, inv, out double parsedStep)) step = parsedStep;
            if (int.TryParse(incrementIntervalBoxes[index].Text, out int parsedInterval)) interval = Math.Max(1, parsedInterval);

            MyBranch.ParamIncrementSteps[keys[index]] = step;
            MyBranch.ParamIncrementIntervals[keys[index]] = interval;

            string id = ParameterAutoChangeService.BuildId(MyBranch, keys[index]);
            bool running = ParameterAutoChangeService.TryGet(id, out _, out _, out bool isRunning) && isRunning;
            bool enable = !running;

            ParameterAutoChangeService.Configure(
                id,
                step,
                interval,
                enable,
                () => GetParamValue(index),
                value => SetParamValue(index, value),
                () => BeginInvoke(new Action(() =>
                {
                    paramBoxes[index].Text = GetParamValue(index).ToString(inv);
                    UpdateIncrementButtonState(index);
                })));

            UpdateIncrementButtonState(index);
        }

        private double GetParamValue(int index)
        {
            if (index == 2) return MyBranch.ActiveResistance;
            if (index == 3) return MyBranch.ReactiveResistance;
            if (index == 4) return MyBranch.ReactiveConductivity;
            if (index == 5) return MyBranch.TransformationRatio;
            if (index == 6) return MyBranch.ActiveConductivity;
            if (index == 7) return MyBranch.PermissibleCurrent;
            return 0;
        }

        private void SetParamValue(int index, double value)
        {
            if (index == 2) MyBranch.ActiveResistance = value;
            else if (index == 3) MyBranch.ReactiveResistance = value;
            else if (index == 4) MyBranch.ReactiveConductivity = value;
            else if (index == 5) MyBranch.TransformationRatio = value;
            else if (index == 6) MyBranch.ActiveConductivity = value;
            else if (index == 7) MyBranch.PermissibleCurrent = value;
            paramBoxes[index].Text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void UpdateIncrementButtonState(int index)
        {
            if (incrementToggleButtons == null || incrementToggleButtons[index] == null) return;
            string id = ParameterAutoChangeService.BuildId(MyBranch, keys[index]);
            bool running = ParameterAutoChangeService.TryGet(id, out _, out _, out bool isRunning) && isRunning;
            incrementToggleButtons[index].Text = running ? "Стоп" : "Старт";
            incrementToggleButtons[index].BackColor = running ? Color.FromArgb(252, 165, 165) : Color.FromArgb(191, 219, 254);
            incrementToggleButtons[index].Font = new Font(incrementToggleButtons[index].Font, FontStyle.Bold);
        }

        private void ApplyBoldFonts(Control root)
        {
            root.Font = new Font(root.Font, FontStyle.Bold);
            foreach (Control c in root.Controls) ApplyBoldFonts(c);
        }

        private void SaveData()
        {
            try
            {
                var inv = CultureInfo.InvariantCulture;
                if (comboStartNode != null && comboStartNode.SelectedItem != null) paramBoxes[0].Text = comboStartNode.SelectedItem.ToString();
                if (comboEndNode != null && comboEndNode.SelectedItem != null) paramBoxes[1].Text = comboEndNode.SelectedItem.ToString();
                MyBranch.StartNodeNumber = int.Parse(paramBoxes[0].Text);
                MyBranch.EndNodeNumber = int.Parse(paramBoxes[1].Text);
                if (MyBranch.StartNodeNumber <= 0 || MyBranch.EndNodeNumber <= 0)
                {
                    MessageBox.Show("Номера узлов ветви должны быть больше 0.");
                    return;
                }
                MyBranch.ActiveResistance = double.Parse(paramBoxes[2].Text.Replace(',', '.'), inv);
                MyBranch.ReactiveResistance = double.Parse(paramBoxes[3].Text.Replace(',', '.'), inv);
                MyBranch.ReactiveConductivity = double.Parse(paramBoxes[4].Text.Replace(',', '.'), inv);
                MyBranch.TransformationRatio = double.Parse(paramBoxes[5].Text.Replace(',', '.'), inv);
                MyBranch.ActiveConductivity = double.Parse(paramBoxes[6].Text.Replace(',', '.'), inv);
                MyBranch.PermissibleCurrent = double.Parse(paramBoxes[7].Text.Replace(',', '.'), inv);

                for (int i = 2; i < 8; i++)
                {
                    MyBranch.ParamAutoModes[keys[i]] = paramChecks[i].Checked;
                    MyBranch.ParamRegisters[keys[i]] = addrBoxes[i].Text;
                }

                MyBranch.Protocol = comboBoxProtocol.Text;
                MyBranch.IPAddress = textBoxIP.Text;
                MyBranch.Port = textBoxPort.Text;
                MyBranch.DeviceID = textBoxID.Text;
                MyBranch.MeasurementIntervalSeconds = (int)numericMeasurementInterval.Value;
                for (int i = 2; i < 8; i++)
                {
                    if (double.TryParse(incrementStepBoxes[i].Text.Replace(',', '.'), NumberStyles.Any, inv, out double step))
                        MyBranch.ParamIncrementSteps[keys[i]] = step;
                    if (int.TryParse(incrementIntervalBoxes[i].Text, out int interval))
                        MyBranch.ParamIncrementIntervals[keys[i]] = Math.Max(1, interval);
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

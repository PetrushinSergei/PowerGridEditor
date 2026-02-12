using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace PowerGridEditor
{
    public class TelemetryEditorForm : Form
    {
        private sealed class GridRowTag
        {
            public string Key { get; set; }
            public dynamic Data { get; set; }
        }

        private readonly Func<IEnumerable<object>> elementsProvider;
        private readonly Func<IEnumerable<GraphicBranch>> branchesProvider;
        private readonly Func<IEnumerable<GraphicShunt>> shuntsProvider;
        private readonly Action invalidateCanvas;

        private readonly DataGridView grid;
        private readonly TextBox textBoxBulkIp;
        private readonly TextBox textBoxBulkPort;
        private readonly TextBox textBoxBulkDeviceId;
        private readonly ComboBox comboBoxBulkProtocol;
        private readonly CheckBox checkBoxBulkTelemetry;
        private readonly Timer refreshTimer;

        public TelemetryEditorForm(
            Func<IEnumerable<object>> elementsProvider,
            Func<IEnumerable<GraphicBranch>> branchesProvider,
            Func<IEnumerable<GraphicShunt>> shuntsProvider,
            Action invalidateCanvas)
        {
            this.elementsProvider = elementsProvider;
            this.branchesProvider = branchesProvider;
            this.shuntsProvider = shuntsProvider;
            this.invalidateCanvas = invalidateCanvas;

            Text = "Телеметрия";
            Width = 1400;
            Height = 780;

            var topHint = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(10, 7, 10, 0),
                Text = "Единая таблица: узлы → базисный узел → ветви → шунты. Данные обновляются в реальном времени.",
                ForeColor = Color.FromArgb(43, 71, 104)
            };

            var topBar = new Panel { Dock = DockStyle.Top, Height = 68, BackColor = Color.FromArgb(236, 242, 251), Padding = new Padding(8) };
            var buttonRefresh = new Button { Left = 8, Top = 8, Width = 95, Height = 30, Text = "Обновить" };
            buttonRefresh.Click += (s, e) => RefreshGridFromModels();

            textBoxBulkIp = new TextBox { Left = 120, Top = 10, Width = 130, Text = "127.0.0.1" };
            textBoxBulkPort = new TextBox { Left = 256, Top = 10, Width = 60, Text = "502" };
            textBoxBulkDeviceId = new TextBox { Left = 322, Top = 10, Width = 70, Text = "1" };
            comboBoxBulkProtocol = new ComboBox { Left = 398, Top = 10, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxBulkProtocol.Items.AddRange(new object[] { "Modbus TCP", "МЭК-104" });
            comboBoxBulkProtocol.SelectedIndex = 0;
            checkBoxBulkTelemetry = new CheckBox { Left = 526, Top = 12, Width = 145, Text = "Телеметрия для всех" };

            var buttonApplyBulk = new Button { Left = 676, Top = 8, Width = 205, Height = 30, Text = "Применить ко всем элементам" };
            buttonApplyBulk.Click += ButtonApplyBulk_Click;

            topBar.Controls.Add(buttonRefresh);
            topBar.Controls.Add(textBoxBulkIp);
            topBar.Controls.Add(textBoxBulkPort);
            topBar.Controls.Add(textBoxBulkDeviceId);
            topBar.Controls.Add(comboBoxBulkProtocol);
            topBar.Controls.Add(checkBoxBulkTelemetry);
            topBar.Controls.Add(buttonApplyBulk);
            topBar.Controls.Add(new Label { Left = 120, Top = 42, Width = 130, Text = "IP" });
            topBar.Controls.Add(new Label { Left = 256, Top = 42, Width = 60, Text = "Порт" });
            topBar.Controls.Add(new Label { Left = 322, Top = 42, Width = 70, Text = "ID" });
            topBar.Controls.Add(new Label { Left = 398, Top = 42, Width = 120, Text = "Протокол" });

            grid = BuildGrid();

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(topHint);

            refreshTimer = new Timer { Interval = 1000 };
            refreshTimer.Tick += (s, e) =>
            {
                if (!grid.IsCurrentCellInEditMode)
                {
                    RefreshGridFromModels();
                }
            };

            Load += (s, e) =>
            {
                RefreshGridFromModels();
                refreshTimer.Start();
            };
            FormClosed += (s, e) => refreshTimer.Stop();
        }

        private DataGridView BuildGrid()
        {
            var table = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
            };

            table.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(228, 236, 246);
            table.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            table.EnableHeadersVisualStyles = false;

            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Тип", ReadOnly = true, Width = 120 });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "Element", HeaderText = "Элемент", ReadOnly = true, Width = 110 });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "Param", HeaderText = "Параметр", ReadOnly = true, Width = 140 });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "Value", HeaderText = "Значение", Width = 90 });
            table.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Telemetry", HeaderText = "Телеметрия", Width = 80 });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "Register", HeaderText = "Адрес", Width = 80 });
            table.Columns.Add(new DataGridViewComboBoxColumn { Name = "Protocol", HeaderText = "Протокол", Width = 95, DataSource = new[] { "Modbus TCP", "МЭК-104" } });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "IP", HeaderText = "IP адрес", Width = 110 });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "Port", HeaderText = "Порт", Width = 65 });
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = "DeviceID", HeaderText = "ID устройства", Width = 95 });
            table.Columns.Add(new DataGridViewButtonColumn { Name = "Ping", HeaderText = "Пинг", Text = "Пинг", UseColumnTextForButtonValue = true, Width = 70 });

            table.CellEndEdit += Grid_CellChanged;
            table.CellValueChanged += Grid_CellChanged;
            table.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (table.IsCurrentCellDirty)
                {
                    table.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            table.CellContentClick += Grid_CellContentClick;

            return table;
        }

        private void RefreshGridFromModels()
        {
            if (grid.IsDisposed) return;
            grid.Rows.Clear();

            AddSectionRow("Узлы");
            foreach (var node in elementsProvider().OfType<GraphicNode>().OrderBy(n => n.Data.Number))
                AddRowsForData("Узел", $"N{node.Data.Number}", node.Data, new[]
                {
                    ("U", "Напряжение", node.Data.InitialVoltage),
                    ("P", "P нагрузка", node.Data.NominalActivePower),
                    ("Q", "Q нагрузка", node.Data.NominalReactivePower),
                    ("Pg", "P генерация", node.Data.ActivePowerGeneration),
                    ("Qg", "Q генерация", node.Data.ReactivePowerGeneration),
                    ("Uf", "U фикс.", node.Data.FixedVoltageModule),
                    ("Qmin", "Q мин", node.Data.MinReactivePower),
                    ("Qmax", "Q макс", node.Data.MaxReactivePower)
                });

            AddSectionRow("Базисный узел");
            foreach (var baseNode in elementsProvider().OfType<GraphicBaseNode>().OrderBy(n => n.Data.Number))
                AddRowsForData("Базисный узел", $"B{baseNode.Data.Number}", baseNode.Data, new[]
                {
                    ("U", "Напряжение", baseNode.Data.InitialVoltage),
                    ("P", "P нагрузка", baseNode.Data.NominalActivePower),
                    ("Q", "Q нагрузка", baseNode.Data.NominalReactivePower),
                    ("Pg", "P генерация", baseNode.Data.ActivePowerGeneration),
                    ("Qg", "Q генерация", baseNode.Data.ReactivePowerGeneration),
                    ("Uf", "U фикс.", baseNode.Data.FixedVoltageModule),
                    ("Qmin", "Q мин", baseNode.Data.MinReactivePower),
                    ("Qmax", "Q макс", baseNode.Data.MaxReactivePower)
                });

            AddSectionRow("Ветви");
            foreach (var branch in branchesProvider().OrderBy(b => b.Data.StartNodeNumber).ThenBy(b => b.Data.EndNodeNumber))
                AddRowsForData("Ветвь", $"{branch.Data.StartNodeNumber}-{branch.Data.EndNodeNumber}", branch.Data, new[]
                {
                    ("R", "R", branch.Data.ActiveResistance),
                    ("X", "X", branch.Data.ReactiveResistance),
                    ("B", "B", branch.Data.ReactiveConductivity),
                    ("Ktr", "K трансф.", branch.Data.TransformationRatio),
                    ("G", "G", branch.Data.ActiveConductivity)
                });

            AddSectionRow("Шунты");
            foreach (var shunt in shuntsProvider().OrderBy(s => s.Data.StartNodeNumber))
                AddRowsForData("Шунт", $"Sh{shunt.Data.StartNodeNumber}", shunt.Data, new[]
                {
                    ("R", "R", shunt.Data.ActiveResistance),
                    ("X", "X", shunt.Data.ReactiveResistance)
                });
        }

        private void AddSectionRow(string title)
        {
            int index = grid.Rows.Add(title, "", "", "", false, "", "", "", "", "", "");
            var row = grid.Rows[index];
            row.ReadOnly = true;
            row.DefaultCellStyle.BackColor = Color.FromArgb(236, 242, 251);
            row.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            row.Tag = null;
        }

        private void AddRowsForData(string type, string elementName, dynamic data, IEnumerable<(string Key, string Label, double Value)> rows)
        {
            foreach (var row in rows)
            {
                int index = grid.Rows.Add(
                    type,
                    elementName,
                    row.Label,
                    row.Value,
                    data.ParamAutoModes[row.Key],
                    data.ParamRegisters[row.Key],
                    data.Protocol,
                    data.IPAddress,
                    data.Port,
                    data is Node ? data.NodeID : data.DeviceID,
                    "Пинг");

                grid.Rows[index].Tag = new GridRowTag { Key = row.Key, Data = data };
            }
        }

        private void Grid_CellChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            if (row.Tag == null) return;
            if (!(row.Tag is GridRowTag tag)) return;

            dynamic data = tag.Data;
            string key = tag.Key;

            if (double.TryParse(Convert.ToString(row.Cells["Value"].Value), out double value))
            {
                ApplyParamValue(data, key, value);
            }

            data.ParamAutoModes[key] = Convert.ToBoolean(row.Cells["Telemetry"].Value);
            data.ParamRegisters[key] = Convert.ToString(row.Cells["Register"].Value) ?? "0";
            data.Protocol = Convert.ToString(row.Cells["Protocol"].Value) ?? "Modbus TCP";
            data.IPAddress = Convert.ToString(row.Cells["IP"].Value) ?? "127.0.0.1";
            data.Port = Convert.ToString(row.Cells["Port"].Value) ?? "502";
            if (data is Node)
            {
                data.NodeID = Convert.ToString(row.Cells["DeviceID"].Value) ?? "1";
            }
            else
            {
                data.DeviceID = Convert.ToString(row.Cells["DeviceID"].Value) ?? "1";
            }

            invalidateCanvas();
        }

        private async void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (grid.Columns[e.ColumnIndex].Name != "Ping") return;

            var row = grid.Rows[e.RowIndex];
            if (row.Tag == null) return;

            string ip = Convert.ToString(row.Cells["IP"].Value);
            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("Введите IP адрес для проверки.");
                return;
            }

            bool ok = await PingHostAsync(ip);
            row.Cells["IP"].Style.BackColor = ok ? Color.LightGreen : Color.LightPink;
            row.Cells["Ping"].Value = ok ? "ОК" : "Нет";
        }

        private void ButtonApplyBulk_Click(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(textBoxBulkIp.Text, out _))
            {
                MessageBox.Show("Введите корректный IP для массового применения.", "Телеметрия", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var allData = elementsProvider().OfType<GraphicNode>().Select(x => (dynamic)x.Data)
                .Concat(elementsProvider().OfType<GraphicBaseNode>().Select(x => (dynamic)x.Data))
                .Concat(branchesProvider().Select(x => (dynamic)x.Data))
                .Concat(shuntsProvider().Select(x => (dynamic)x.Data))
                .ToList();

            foreach (var data in allData)
            {
                data.IPAddress = textBoxBulkIp.Text;
                data.Port = textBoxBulkPort.Text;
                data.Protocol = Convert.ToString(comboBoxBulkProtocol.SelectedItem) ?? "Modbus TCP";
                if (data is Node)
                {
                    data.NodeID = textBoxBulkDeviceId.Text;
                }
                else
                {
                    data.DeviceID = textBoxBulkDeviceId.Text;
                }

                foreach (var key in new List<string>(data.ParamAutoModes.Keys))
                {
                    data.ParamAutoModes[key] = checkBoxBulkTelemetry.Checked;
                }
            }

            RefreshGridFromModels();
            invalidateCanvas();
            MessageBox.Show("Параметры телеметрии применены ко всем элементам.", "Телеметрия", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyParamValue(dynamic data, string key, double value)
        {
            if (key == "U") data.InitialVoltage = value;
            else if (key == "P") data.NominalActivePower = value;
            else if (key == "Q") data.NominalReactivePower = value;
            else if (key == "Pg") data.ActivePowerGeneration = value;
            else if (key == "Qg") data.ReactivePowerGeneration = value;
            else if (key == "Uf") data.FixedVoltageModule = value;
            else if (key == "Qmin") data.MinReactivePower = value;
            else if (key == "Qmax") data.MaxReactivePower = value;
            else if (key == "R") data.ActiveResistance = value;
            else if (key == "X") data.ReactiveResistance = value;
            else if (key == "B") data.ReactiveConductivity = value;
            else if (key == "Ktr") data.TransformationRatio = value;
            else if (key == "G") data.ActiveConductivity = value;
        }

        private async System.Threading.Tasks.Task<bool> PingHostAsync(string ip)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(ip, 1000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

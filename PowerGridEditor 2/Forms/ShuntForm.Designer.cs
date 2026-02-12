namespace PowerGridEditor
{
    partial class ShuntForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabParams, tabSettings;
        private System.Windows.Forms.Button btnCheckIP, buttonSave, buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxProtocol;
        private System.Windows.Forms.TextBox textBoxIP, textBoxPort, textBoxID;

        private System.Windows.Forms.TextBox[] paramBoxes;
        private System.Windows.Forms.CheckBox[] paramChecks;
        private System.Windows.Forms.TextBox[] addrBoxes;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabParams = new System.Windows.Forms.TabPage { Text = "Параметры шунта" };
            this.tabSettings = new System.Windows.Forms.TabPage { Text = "Настройки связи" };

            this.paramBoxes = new System.Windows.Forms.TextBox[3];
            this.paramChecks = new System.Windows.Forms.CheckBox[3];
            this.addrBoxes = new System.Windows.Forms.TextBox[3];

            this.buttonSave = new System.Windows.Forms.Button { Text = "Сохранить", Size = new System.Drawing.Size(100, 32) };
            this.buttonCancel = new System.Windows.Forms.Button { Text = "Отмена", Size = new System.Drawing.Size(100, 32) };

            this.SuspendLayout();

            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Size = new System.Drawing.Size(620, 240);
            this.tabControl.Controls.Add(tabParams);
            this.tabControl.Controls.Add(tabSettings);

            string[] names = {
                "Начальный узел:",
                "Активное сопротивление R:",
                "Реактивное сопротивление X:"
            };

            int y = 20;
            for (int i = 0; i < 3; i++)
            {
                var lblName = new System.Windows.Forms.Label
                {
                    Text = names[i],
                    Location = new System.Drawing.Point(10, y + 3),
                    Size = new System.Drawing.Size(190, 20)
                };

                paramBoxes[i] = new System.Windows.Forms.TextBox
                {
                    Location = new System.Drawing.Point(205, y),
                    Size = new System.Drawing.Size(80, 23)
                };

                tabParams.Controls.Add(lblName);
                tabParams.Controls.Add(paramBoxes[i]);

                if (i >= 1)
                {
                    paramChecks[i] = new System.Windows.Forms.CheckBox
                    {
                        Text = "Телеметрия",
                        Location = new System.Drawing.Point(295, y + 2),
                        Size = new System.Drawing.Size(100, 20)
                    };

                    var lblAddr = new System.Windows.Forms.Label
                    {
                        Text = "Адрес:",
                        Location = new System.Drawing.Point(405, y + 3),
                        Size = new System.Drawing.Size(50, 20)
                    };

                    addrBoxes[i] = new System.Windows.Forms.TextBox
                    {
                        Location = new System.Drawing.Point(460, y),
                        Size = new System.Drawing.Size(60, 23)
                    };

                    tabParams.Controls.Add(paramChecks[i]);
                    tabParams.Controls.Add(lblAddr);
                    tabParams.Controls.Add(addrBoxes[i]);
                }

                y += 35;
            }

            SetupSettingsTab();

            this.buttonSave.Location = new System.Drawing.Point(390, 265);
            this.buttonCancel.Location = new System.Drawing.Point(505, 265);

            this.ClientSize = new System.Drawing.Size(644, 315);
            this.Controls.AddRange(new System.Windows.Forms.Control[] { tabControl, buttonSave, buttonCancel });
            this.Text = "Редактор параметров шунта";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            this.ResumeLayout(false);
        }

        private void SetupSettingsTab()
        {
            int sy = 30;

            var lblProt = new System.Windows.Forms.Label { Text = "Протокол:", Location = new System.Drawing.Point(20, sy + 3) };
            comboBoxProtocol = new System.Windows.Forms.ComboBox
            {
                Location = new System.Drawing.Point(140, sy),
                Size = new System.Drawing.Size(150, 23),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };
            comboBoxProtocol.Items.AddRange(new object[] { "Modbus TCP", "МЭК-104" });
            tabSettings.Controls.AddRange(new System.Windows.Forms.Control[] { lblProt, comboBoxProtocol });
            sy += 45;

            var lblIp = new System.Windows.Forms.Label { Text = "IP адрес:", Location = new System.Drawing.Point(20, sy + 3) };
            textBoxIP = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(140, sy), Size = new System.Drawing.Size(150, 23) };
            btnCheckIP = new System.Windows.Forms.Button { Text = "Пинг", Location = new System.Drawing.Point(300, sy - 1), Size = new System.Drawing.Size(80, 25) };
            tabSettings.Controls.AddRange(new System.Windows.Forms.Control[] { lblIp, textBoxIP, btnCheckIP });
            sy += 45;

            var lblPort = new System.Windows.Forms.Label { Text = "TCP Порт:", Location = new System.Drawing.Point(20, sy + 3) };
            textBoxPort = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(140, sy), Size = new System.Drawing.Size(80, 23) };
            tabSettings.Controls.AddRange(new System.Windows.Forms.Control[] { lblPort, textBoxPort });
            sy += 45;

            var lblId = new System.Windows.Forms.Label { Text = "ID устройства:", Location = new System.Drawing.Point(20, sy + 3) };
            textBoxID = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(140, sy), Size = new System.Drawing.Size(80, 23) };
            tabSettings.Controls.AddRange(new System.Windows.Forms.Control[] { lblId, textBoxID });
        }
    }
}

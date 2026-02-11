namespace PowerGridEditor
{
    partial class BaseNodeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxMaxReactivePower = new System.Windows.Forms.TextBox();
            this.textBoxMinReactivePower = new System.Windows.Forms.TextBox();
            this.textBoxFixedVoltageModule = new System.Windows.Forms.TextBox();
            this.textBoxReactivePowerGeneration = new System.Windows.Forms.TextBox();
            this.textBoxActivePowerGeneration = new System.Windows.Forms.TextBox();
            this.textBoxNominalReactivePower = new System.Windows.Forms.TextBox();
            this.textBoxNominalActivePower = new System.Windows.Forms.TextBox();
            this.textBoxInitialVoltage = new System.Windows.Forms.TextBox();
            this.textBoxNodeNumber = new System.Windows.Forms.TextBox();
            this.labelMaxReactivePower = new System.Windows.Forms.Label();
            this.labelMinReactivePower = new System.Windows.Forms.Label();
            this.labelFixedVoltageModule = new System.Windows.Forms.Label();
            this.labelReactivePowerGeneration = new System.Windows.Forms.Label();
            this.labelActivePowerGeneration = new System.Windows.Forms.Label();
            this.labelNominalReactivePower = new System.Windows.Forms.Label();
            this.labelNominalActivePower = new System.Windows.Forms.Label();
            this.labelInitialVoltage = new System.Windows.Forms.Label();
            this.labelNodeNumber = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(309, 363);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 41;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(184, 363);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 40;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxMaxReactivePower
            // 
            this.textBoxMaxReactivePower.Location = new System.Drawing.Point(300, 323);
            this.textBoxMaxReactivePower.Name = "textBoxMaxReactivePower";
            this.textBoxMaxReactivePower.Size = new System.Drawing.Size(100, 20);
            this.textBoxMaxReactivePower.TabIndex = 39;
            // 
            // textBoxMinReactivePower
            // 
            this.textBoxMinReactivePower.Location = new System.Drawing.Point(300, 288);
            this.textBoxMinReactivePower.Name = "textBoxMinReactivePower";
            this.textBoxMinReactivePower.Size = new System.Drawing.Size(100, 20);
            this.textBoxMinReactivePower.TabIndex = 38;
            // 
            // textBoxFixedVoltageModule
            // 
            this.textBoxFixedVoltageModule.Location = new System.Drawing.Point(300, 262);
            this.textBoxFixedVoltageModule.Name = "textBoxFixedVoltageModule";
            this.textBoxFixedVoltageModule.Size = new System.Drawing.Size(100, 20);
            this.textBoxFixedVoltageModule.TabIndex = 37;
            // 
            // textBoxReactivePowerGeneration
            // 
            this.textBoxReactivePowerGeneration.Location = new System.Drawing.Point(300, 232);
            this.textBoxReactivePowerGeneration.Name = "textBoxReactivePowerGeneration";
            this.textBoxReactivePowerGeneration.Size = new System.Drawing.Size(100, 20);
            this.textBoxReactivePowerGeneration.TabIndex = 36;
            // 
            // textBoxActivePowerGeneration
            // 
            this.textBoxActivePowerGeneration.Location = new System.Drawing.Point(300, 205);
            this.textBoxActivePowerGeneration.Name = "textBoxActivePowerGeneration";
            this.textBoxActivePowerGeneration.Size = new System.Drawing.Size(100, 20);
            this.textBoxActivePowerGeneration.TabIndex = 35;
            // 
            // textBoxNominalReactivePower
            // 
            this.textBoxNominalReactivePower.Location = new System.Drawing.Point(300, 169);
            this.textBoxNominalReactivePower.Name = "textBoxNominalReactivePower";
            this.textBoxNominalReactivePower.Size = new System.Drawing.Size(100, 20);
            this.textBoxNominalReactivePower.TabIndex = 34;
            // 
            // textBoxNominalActivePower
            // 
            this.textBoxNominalActivePower.Location = new System.Drawing.Point(300, 139);
            this.textBoxNominalActivePower.Name = "textBoxNominalActivePower";
            this.textBoxNominalActivePower.Size = new System.Drawing.Size(100, 20);
            this.textBoxNominalActivePower.TabIndex = 33;
            // 
            // textBoxInitialVoltage
            // 
            this.textBoxInitialVoltage.Location = new System.Drawing.Point(300, 107);
            this.textBoxInitialVoltage.Name = "textBoxInitialVoltage";
            this.textBoxInitialVoltage.Size = new System.Drawing.Size(100, 20);
            this.textBoxInitialVoltage.TabIndex = 32;
            // 
            // textBoxNodeNumber
            // 
            this.textBoxNodeNumber.Location = new System.Drawing.Point(300, 68);
            this.textBoxNodeNumber.Name = "textBoxNodeNumber";
            this.textBoxNodeNumber.Size = new System.Drawing.Size(100, 20);
            this.textBoxNodeNumber.TabIndex = 31;
            // 
            // labelMaxReactivePower
            // 
            this.labelMaxReactivePower.AutoSize = true;
            this.labelMaxReactivePower.Location = new System.Drawing.Point(65, 323);
            this.labelMaxReactivePower.Name = "labelMaxReactivePower";
            this.labelMaxReactivePower.Size = new System.Drawing.Size(193, 13);
            this.labelMaxReactivePower.TabIndex = 30;
            this.labelMaxReactivePower.Text = "Максимум по реактивной мощности";
            // 
            // labelMinReactivePower
            // 
            this.labelMinReactivePower.AutoSize = true;
            this.labelMinReactivePower.Location = new System.Drawing.Point(65, 291);
            this.labelMinReactivePower.Name = "labelMinReactivePower";
            this.labelMinReactivePower.Size = new System.Drawing.Size(187, 13);
            this.labelMinReactivePower.TabIndex = 29;
            this.labelMinReactivePower.Text = "Минимум по реактивной мощности";
            // 
            // labelFixedVoltageModule
            // 
            this.labelFixedVoltageModule.AutoSize = true;
            this.labelFixedVoltageModule.Location = new System.Drawing.Point(62, 262);
            this.labelFixedVoltageModule.Name = "labelFixedVoltageModule";
            this.labelFixedVoltageModule.Size = new System.Drawing.Size(187, 13);
            this.labelFixedVoltageModule.TabIndex = 28;
            this.labelFixedVoltageModule.Text = "Закрепленный модуль напряжения";
            // 
            // labelReactivePowerGeneration
            // 
            this.labelReactivePowerGeneration.AutoSize = true;
            this.labelReactivePowerGeneration.Location = new System.Drawing.Point(62, 232);
            this.labelReactivePowerGeneration.Name = "labelReactivePowerGeneration";
            this.labelReactivePowerGeneration.Size = new System.Drawing.Size(178, 13);
            this.labelReactivePowerGeneration.TabIndex = 27;
            this.labelReactivePowerGeneration.Text = "Реактивная мощность генерации";
            // 
            // labelActivePowerGeneration
            // 
            this.labelActivePowerGeneration.AutoSize = true;
            this.labelActivePowerGeneration.Location = new System.Drawing.Point(65, 205);
            this.labelActivePowerGeneration.Name = "labelActivePowerGeneration";
            this.labelActivePowerGeneration.Size = new System.Drawing.Size(166, 13);
            this.labelActivePowerGeneration.TabIndex = 26;
            this.labelActivePowerGeneration.Text = "Активная мощность генерации";
            // 
            // labelNominalReactivePower
            // 
            this.labelNominalReactivePower.AutoSize = true;
            this.labelNominalReactivePower.Location = new System.Drawing.Point(65, 172);
            this.labelNominalReactivePower.Name = "labelNominalReactivePower";
            this.labelNominalReactivePower.Size = new System.Drawing.Size(198, 13);
            this.labelNominalReactivePower.TabIndex = 25;
            this.labelNominalReactivePower.Text = "Ном. реактивная мощность нагрузки";
            // 
            // labelNominalActivePower
            // 
            this.labelNominalActivePower.AutoSize = true;
            this.labelNominalActivePower.Location = new System.Drawing.Point(65, 139);
            this.labelNominalActivePower.Name = "labelNominalActivePower";
            this.labelNominalActivePower.Size = new System.Drawing.Size(186, 13);
            this.labelNominalActivePower.TabIndex = 24;
            this.labelNominalActivePower.Text = "Ном. активная мощность нагрузки";
            // 
            // labelInitialVoltage
            // 
            this.labelInitialVoltage.AutoSize = true;
            this.labelInitialVoltage.Location = new System.Drawing.Point(65, 107);
            this.labelInitialVoltage.Name = "labelInitialVoltage";
            this.labelInitialVoltage.Size = new System.Drawing.Size(165, 13);
            this.labelInitialVoltage.TabIndex = 23;
            this.labelInitialVoltage.Text = "Нач. приближение напряжения";
            // 
            // labelNodeNumber
            // 
            this.labelNodeNumber.AutoSize = true;
            this.labelNodeNumber.Location = new System.Drawing.Point(65, 76);
            this.labelNodeNumber.Name = "labelNodeNumber";
            this.labelNodeNumber.Size = new System.Drawing.Size(67, 13);
            this.labelNodeNumber.TabIndex = 22;
            this.labelNodeNumber.Text = "Номер узла";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Enabled = false;
            this.labelCode.Location = new System.Drawing.Point(62, 45);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(62, 13);
            this.labelCode.TabIndex = 21;
            this.labelCode.Text = "Код 0102 0";
            this.labelCode.Click += new System.EventHandler(this.labelCode_Click);
            // 
            // BaseNodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxMaxReactivePower);
            this.Controls.Add(this.textBoxMinReactivePower);
            this.Controls.Add(this.textBoxFixedVoltageModule);
            this.Controls.Add(this.textBoxReactivePowerGeneration);
            this.Controls.Add(this.textBoxActivePowerGeneration);
            this.Controls.Add(this.textBoxNominalReactivePower);
            this.Controls.Add(this.textBoxNominalActivePower);
            this.Controls.Add(this.textBoxInitialVoltage);
            this.Controls.Add(this.textBoxNodeNumber);
            this.Controls.Add(this.labelMaxReactivePower);
            this.Controls.Add(this.labelMinReactivePower);
            this.Controls.Add(this.labelFixedVoltageModule);
            this.Controls.Add(this.labelReactivePowerGeneration);
            this.Controls.Add(this.labelActivePowerGeneration);
            this.Controls.Add(this.labelNominalReactivePower);
            this.Controls.Add(this.labelNominalActivePower);
            this.Controls.Add(this.labelInitialVoltage);
            this.Controls.Add(this.labelNodeNumber);
            this.Controls.Add(this.labelCode);
            this.Name = "BaseNodeForm";
            this.Text = "BaseNodeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxMaxReactivePower;
        private System.Windows.Forms.TextBox textBoxMinReactivePower;
        private System.Windows.Forms.TextBox textBoxFixedVoltageModule;
        private System.Windows.Forms.TextBox textBoxReactivePowerGeneration;
        private System.Windows.Forms.TextBox textBoxActivePowerGeneration;
        private System.Windows.Forms.TextBox textBoxNominalReactivePower;
        private System.Windows.Forms.TextBox textBoxNominalActivePower;
        private System.Windows.Forms.TextBox textBoxInitialVoltage;
        private System.Windows.Forms.TextBox textBoxNodeNumber;
        private System.Windows.Forms.Label labelMaxReactivePower;
        private System.Windows.Forms.Label labelMinReactivePower;
        private System.Windows.Forms.Label labelFixedVoltageModule;
        private System.Windows.Forms.Label labelReactivePowerGeneration;
        private System.Windows.Forms.Label labelActivePowerGeneration;
        private System.Windows.Forms.Label labelNominalReactivePower;
        private System.Windows.Forms.Label labelNominalActivePower;
        private System.Windows.Forms.Label labelInitialVoltage;
        private System.Windows.Forms.Label labelNodeNumber;
        private System.Windows.Forms.Label labelCode;
    }
}
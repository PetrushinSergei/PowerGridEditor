namespace PowerGridEditor
{
    partial class ShuntForm
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
            this.textBoxReactiveResistance = new System.Windows.Forms.TextBox();
            this.textBoxActiveResistance = new System.Windows.Forms.TextBox();
            this.textBoxEndNode = new System.Windows.Forms.TextBox();
            this.textBoxStartNode = new System.Windows.Forms.TextBox();
            this.labelReactiveResistance = new System.Windows.Forms.Label();
            this.labelActiveResistance = new System.Windows.Forms.Label();
            this.labelEndNode = new System.Windows.Forms.Label();
            this.labelStartNode = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(478, 373);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 79;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(353, 373);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 78;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxReactiveResistance
            // 
            this.textBoxReactiveResistance.Location = new System.Drawing.Point(469, 179);
            this.textBoxReactiveResistance.Name = "textBoxReactiveResistance";
            this.textBoxReactiveResistance.Size = new System.Drawing.Size(100, 20);
            this.textBoxReactiveResistance.TabIndex = 74;
            // 
            // textBoxActiveResistance
            // 
            this.textBoxActiveResistance.Location = new System.Drawing.Point(469, 149);
            this.textBoxActiveResistance.Name = "textBoxActiveResistance";
            this.textBoxActiveResistance.Size = new System.Drawing.Size(100, 20);
            this.textBoxActiveResistance.TabIndex = 73;
            // 
            // textBoxEndNode
            // 
            this.textBoxEndNode.Location = new System.Drawing.Point(469, 117);
            this.textBoxEndNode.Name = "textBoxEndNode";
            this.textBoxEndNode.ReadOnly = true;
            this.textBoxEndNode.Size = new System.Drawing.Size(100, 20);
            this.textBoxEndNode.TabIndex = 72;
            // 
            // textBoxStartNode
            // 
            this.textBoxStartNode.Location = new System.Drawing.Point(469, 78);
            this.textBoxStartNode.Name = "textBoxStartNode";
            this.textBoxStartNode.Size = new System.Drawing.Size(100, 20);
            this.textBoxStartNode.TabIndex = 71;
            // 
            // labelReactiveResistance
            // 
            this.labelReactiveResistance.AutoSize = true;
            this.labelReactiveResistance.Location = new System.Drawing.Point(234, 182);
            this.labelReactiveResistance.Name = "labelReactiveResistance";
            this.labelReactiveResistance.Size = new System.Drawing.Size(147, 13);
            this.labelReactiveResistance.TabIndex = 67;
            this.labelReactiveResistance.Text = "Реактивное сопротивление";
            // 
            // labelActiveResistance
            // 
            this.labelActiveResistance.AutoSize = true;
            this.labelActiveResistance.Location = new System.Drawing.Point(234, 149);
            this.labelActiveResistance.Name = "labelActiveResistance";
            this.labelActiveResistance.Size = new System.Drawing.Size(135, 13);
            this.labelActiveResistance.TabIndex = 66;
            this.labelActiveResistance.Text = "Активное сопротивление";
            // 
            // labelEndNode
            // 
            this.labelEndNode.AutoSize = true;
            this.labelEndNode.Location = new System.Drawing.Point(234, 117);
            this.labelEndNode.Name = "labelEndNode";
            this.labelEndNode.Size = new System.Drawing.Size(83, 13);
            this.labelEndNode.TabIndex = 65;
            this.labelEndNode.Text = "Конечный узел";
            // 
            // labelStartNode
            // 
            this.labelStartNode.AutoSize = true;
            this.labelStartNode.Location = new System.Drawing.Point(234, 86);
            this.labelStartNode.Name = "labelStartNode";
            this.labelStartNode.Size = new System.Drawing.Size(90, 13);
            this.labelStartNode.TabIndex = 64;
            this.labelStartNode.Text = "Начальный узел";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Enabled = false;
            this.labelCode.Location = new System.Drawing.Point(231, 55);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(62, 13);
            this.labelCode.TabIndex = 63;
            this.labelCode.Text = "Код 0103 0";
            this.labelCode.Click += new System.EventHandler(this.labelCode_Click);
            // 
            // ShuntForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxReactiveResistance);
            this.Controls.Add(this.textBoxActiveResistance);
            this.Controls.Add(this.textBoxEndNode);
            this.Controls.Add(this.textBoxStartNode);
            this.Controls.Add(this.labelReactiveResistance);
            this.Controls.Add(this.labelActiveResistance);
            this.Controls.Add(this.labelEndNode);
            this.Controls.Add(this.labelStartNode);
            this.Controls.Add(this.labelCode);
            this.Name = "ShuntForm";
            this.Text = "Параметры шунта";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxReactiveResistance;
        private System.Windows.Forms.TextBox textBoxActiveResistance;
        private System.Windows.Forms.TextBox textBoxEndNode;
        private System.Windows.Forms.TextBox textBoxStartNode;
        private System.Windows.Forms.Label labelReactiveResistance;
        private System.Windows.Forms.Label labelActiveResistance;
        private System.Windows.Forms.Label labelEndNode;
        private System.Windows.Forms.Label labelStartNode;
        private System.Windows.Forms.Label labelCode;
    }
}
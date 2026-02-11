namespace PowerGridEditor
{
    partial class BranchForm
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
            this.textBoxActiveConductivity = new System.Windows.Forms.TextBox();
            this.textBoxTransformationRatio = new System.Windows.Forms.TextBox();
            this.textBoxReactiveConductivity = new System.Windows.Forms.TextBox();
            this.textBoxReactiveResistance = new System.Windows.Forms.TextBox();
            this.textBoxActiveResistance = new System.Windows.Forms.TextBox();
            this.textBoxEndNode = new System.Windows.Forms.TextBox();
            this.textBoxStartNode = new System.Windows.Forms.TextBox();
            this.labelActiveConductivity = new System.Windows.Forms.Label();
            this.labelTransformationRatio = new System.Windows.Forms.Label();
            this.labelReactiveConductivity = new System.Windows.Forms.Label();
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
            this.buttonCancel.Location = new System.Drawing.Point(332, 364);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 62;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(207, 364);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 61;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxActiveConductivity
            // 
            this.textBoxActiveConductivity.Location = new System.Drawing.Point(323, 263);
            this.textBoxActiveConductivity.Name = "textBoxActiveConductivity";
            this.textBoxActiveConductivity.Size = new System.Drawing.Size(100, 20);
            this.textBoxActiveConductivity.TabIndex = 58;
            // 
            // textBoxTransformationRatio
            // 
            this.textBoxTransformationRatio.Location = new System.Drawing.Point(323, 233);
            this.textBoxTransformationRatio.Name = "textBoxTransformationRatio";
            this.textBoxTransformationRatio.Size = new System.Drawing.Size(100, 20);
            this.textBoxTransformationRatio.TabIndex = 57;
            // 
            // textBoxReactiveConductivity
            // 
            this.textBoxReactiveConductivity.Location = new System.Drawing.Point(323, 206);
            this.textBoxReactiveConductivity.Name = "textBoxReactiveConductivity";
            this.textBoxReactiveConductivity.Size = new System.Drawing.Size(100, 20);
            this.textBoxReactiveConductivity.TabIndex = 56;
            // 
            // textBoxReactiveResistance
            // 
            this.textBoxReactiveResistance.Location = new System.Drawing.Point(323, 170);
            this.textBoxReactiveResistance.Name = "textBoxReactiveResistance";
            this.textBoxReactiveResistance.Size = new System.Drawing.Size(100, 20);
            this.textBoxReactiveResistance.TabIndex = 55;
            // 
            // textBoxActiveResistance
            // 
            this.textBoxActiveResistance.Location = new System.Drawing.Point(323, 140);
            this.textBoxActiveResistance.Name = "textBoxActiveResistance";
            this.textBoxActiveResistance.Size = new System.Drawing.Size(100, 20);
            this.textBoxActiveResistance.TabIndex = 54;
            // 
            // textBoxEndNode
            // 
            this.textBoxEndNode.Location = new System.Drawing.Point(323, 108);
            this.textBoxEndNode.Name = "textBoxEndNode";
            this.textBoxEndNode.Size = new System.Drawing.Size(100, 20);
            this.textBoxEndNode.TabIndex = 53;
            // 
            // textBoxStartNode
            // 
            this.textBoxStartNode.Location = new System.Drawing.Point(323, 69);
            this.textBoxStartNode.Name = "textBoxStartNode";
            this.textBoxStartNode.Size = new System.Drawing.Size(100, 20);
            this.textBoxStartNode.TabIndex = 52;
            // 
            // labelActiveConductivity
            // 
            this.labelActiveConductivity.AutoSize = true;
            this.labelActiveConductivity.Location = new System.Drawing.Point(85, 263);
            this.labelActiveConductivity.Name = "labelActiveConductivity";
            this.labelActiveConductivity.Size = new System.Drawing.Size(218, 13);
            this.labelActiveConductivity.TabIndex = 49;
            this.labelActiveConductivity.Text = "Активная проводимость на землю (мкСм";
            // 
            // labelTransformationRatio
            // 
            this.labelTransformationRatio.AutoSize = true;
            this.labelTransformationRatio.Location = new System.Drawing.Point(85, 233);
            this.labelTransformationRatio.Name = "labelTransformationRatio";
            this.labelTransformationRatio.Size = new System.Drawing.Size(207, 13);
            this.labelTransformationRatio.TabIndex = 48;
            this.labelTransformationRatio.Text = "Модуль коэффициента трансформации";
            // 
            // labelReactiveConductivity
            // 
            this.labelReactiveConductivity.AutoSize = true;
            this.labelReactiveConductivity.Location = new System.Drawing.Point(88, 206);
            this.labelReactiveConductivity.Name = "labelReactiveConductivity";
            this.labelReactiveConductivity.Size = new System.Drawing.Size(181, 13);
            this.labelReactiveConductivity.TabIndex = 47;
            this.labelReactiveConductivity.Text = "Реактивная проводимость (мкСм)";
            // 
            // labelReactiveResistance
            // 
            this.labelReactiveResistance.AutoSize = true;
            this.labelReactiveResistance.Location = new System.Drawing.Point(88, 173);
            this.labelReactiveResistance.Name = "labelReactiveResistance";
            this.labelReactiveResistance.Size = new System.Drawing.Size(147, 13);
            this.labelReactiveResistance.TabIndex = 46;
            this.labelReactiveResistance.Text = "Реактивное сопротивление";
            // 
            // labelActiveResistance
            // 
            this.labelActiveResistance.AutoSize = true;
            this.labelActiveResistance.Location = new System.Drawing.Point(88, 140);
            this.labelActiveResistance.Name = "labelActiveResistance";
            this.labelActiveResistance.Size = new System.Drawing.Size(135, 13);
            this.labelActiveResistance.TabIndex = 45;
            this.labelActiveResistance.Text = "Активное сопротивление";
            // 
            // labelEndNode
            // 
            this.labelEndNode.AutoSize = true;
            this.labelEndNode.Location = new System.Drawing.Point(88, 108);
            this.labelEndNode.Name = "labelEndNode";
            this.labelEndNode.Size = new System.Drawing.Size(83, 13);
            this.labelEndNode.TabIndex = 44;
            this.labelEndNode.Text = "Конечный узел";
            // 
            // labelStartNode
            // 
            this.labelStartNode.AutoSize = true;
            this.labelStartNode.Location = new System.Drawing.Point(88, 77);
            this.labelStartNode.Name = "labelStartNode";
            this.labelStartNode.Size = new System.Drawing.Size(90, 13);
            this.labelStartNode.TabIndex = 43;
            this.labelStartNode.Text = "Начальный узел";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Enabled = false;
            this.labelCode.Location = new System.Drawing.Point(85, 46);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(62, 13);
            this.labelCode.TabIndex = 42;
            this.labelCode.Text = "Код 0103 0";
            // 
            // BranchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxActiveConductivity);
            this.Controls.Add(this.textBoxTransformationRatio);
            this.Controls.Add(this.textBoxReactiveConductivity);
            this.Controls.Add(this.textBoxReactiveResistance);
            this.Controls.Add(this.textBoxActiveResistance);
            this.Controls.Add(this.textBoxEndNode);
            this.Controls.Add(this.textBoxStartNode);
            this.Controls.Add(this.labelActiveConductivity);
            this.Controls.Add(this.labelTransformationRatio);
            this.Controls.Add(this.labelReactiveConductivity);
            this.Controls.Add(this.labelReactiveResistance);
            this.Controls.Add(this.labelActiveResistance);
            this.Controls.Add(this.labelEndNode);
            this.Controls.Add(this.labelStartNode);
            this.Controls.Add(this.labelCode);
            this.Name = "BranchForm";
            this.Text = "BranchForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxActiveConductivity;
        private System.Windows.Forms.TextBox textBoxTransformationRatio;
        private System.Windows.Forms.TextBox textBoxReactiveConductivity;
        private System.Windows.Forms.TextBox textBoxReactiveResistance;
        private System.Windows.Forms.TextBox textBoxActiveResistance;
        private System.Windows.Forms.TextBox textBoxEndNode;
        private System.Windows.Forms.TextBox textBoxStartNode;
        private System.Windows.Forms.Label labelActiveConductivity;
        private System.Windows.Forms.Label labelTransformationRatio;
        private System.Windows.Forms.Label labelReactiveConductivity;
        private System.Windows.Forms.Label labelReactiveResistance;
        private System.Windows.Forms.Label labelActiveResistance;
        private System.Windows.Forms.Label labelEndNode;
        private System.Windows.Forms.Label labelStartNode;
        private System.Windows.Forms.Label labelCode;
    }
}
namespace PowerGridEditor
{
    partial class ReportForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInput;
        private System.Windows.Forms.TabPage tabResults;
        private System.Windows.Forms.TabPage tabLoss;
        private System.Windows.Forms.TabPage tabBreakdown;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.TextBox txtLoss;
        private System.Windows.Forms.TextBox txtBreakdown;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInput = new System.Windows.Forms.TabPage();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.tabLoss = new System.Windows.Forms.TabPage();
            this.txtLoss = new System.Windows.Forms.TextBox();
            this.tabBreakdown = new System.Windows.Forms.TabPage();
            this.txtBreakdown = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabInput.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabLoss.SuspendLayout();
            this.tabBreakdown.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabInput);
            this.tabControl1.Controls.Add(this.tabResults);
            this.tabControl1.Controls.Add(this.tabLoss);
            this.tabControl1.Controls.Add(this.tabBreakdown);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(860, 620);
            this.tabControl1.TabIndex = 0;
            // 
            // tabInput
            // 
            this.tabInput.Controls.Add(this.txtInput);
            this.tabInput.Location = new System.Drawing.Point(4, 22);
            this.tabInput.Name = "tabInput";
            this.tabInput.Padding = new System.Windows.Forms.Padding(3);
            this.tabInput.Size = new System.Drawing.Size(852, 594);
            this.tabInput.TabIndex = 0;
            this.tabInput.Text = "Исх. данные";
            this.tabInput.UseVisualStyleBackColor = true;
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtInput.Location = new System.Drawing.Point(3, 3);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInput.Size = new System.Drawing.Size(846, 588);
            this.txtInput.TabIndex = 0;
            this.txtInput.WordWrap = false;
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.txtResults);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabResults.Size = new System.Drawing.Size(852, 594);
            this.tabResults.TabIndex = 1;
            this.tabResults.Text = "Результаты";
            this.tabResults.UseVisualStyleBackColor = true;
            // 
            // txtResults
            // 
            this.txtResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResults.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtResults.Location = new System.Drawing.Point(3, 3);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(846, 588);
            this.txtResults.TabIndex = 0;
            this.txtResults.WordWrap = false;
            // 
            // tabLoss
            // 
            this.tabLoss.Controls.Add(this.txtLoss);
            this.tabLoss.Location = new System.Drawing.Point(4, 22);
            this.tabLoss.Name = "tabLoss";
            this.tabLoss.Size = new System.Drawing.Size(852, 594);
            this.tabLoss.TabIndex = 2;
            this.tabLoss.Text = "Анализ потерь";
            this.tabLoss.UseVisualStyleBackColor = true;
            // 
            // txtLoss
            // 
            this.txtLoss.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLoss.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtLoss.Location = new System.Drawing.Point(0, 0);
            this.txtLoss.Multiline = true;
            this.txtLoss.Name = "txtLoss";
            this.txtLoss.ReadOnly = true;
            this.txtLoss.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLoss.Size = new System.Drawing.Size(852, 594);
            this.txtLoss.TabIndex = 0;
            this.txtLoss.WordWrap = false;
            // 
            // tabBreakdown
            // 
            this.tabBreakdown.Controls.Add(this.txtBreakdown);
            this.tabBreakdown.Location = new System.Drawing.Point(4, 22);
            this.tabBreakdown.Name = "tabBreakdown";
            this.tabBreakdown.Size = new System.Drawing.Size(852, 594);
            this.tabBreakdown.TabIndex = 3;
            this.tabBreakdown.Text = "Составляющие потерь";
            this.tabBreakdown.UseVisualStyleBackColor = true;
            // 
            // txtBreakdown
            // 
            this.txtBreakdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBreakdown.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtBreakdown.Location = new System.Drawing.Point(0, 0);
            this.txtBreakdown.Multiline = true;
            this.txtBreakdown.Name = "txtBreakdown";
            this.txtBreakdown.ReadOnly = true;
            this.txtBreakdown.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBreakdown.Size = new System.Drawing.Size(852, 594);
            this.txtBreakdown.TabIndex = 0;
            this.txtBreakdown.WordWrap = false;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 620);
            this.Controls.Add(this.tabControl1);
            this.Name = "ReportForm";
            this.Text = "network.cdu";
            this.tabControl1.ResumeLayout(false);
            this.tabInput.ResumeLayout(false);
            this.tabInput.PerformLayout();
            this.tabResults.ResumeLayout(false);
            this.tabResults.PerformLayout();
            this.tabLoss.ResumeLayout(false);
            this.tabLoss.PerformLayout();
            this.tabBreakdown.ResumeLayout(false);
            this.tabBreakdown.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}

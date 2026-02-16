namespace PowerGridEditor
{
    partial class ReportForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.TabPage tabErr;
        private System.Windows.Forms.TabPage tabInput;
        private System.Windows.Forms.TabPage tabResults;
        private System.Windows.Forms.TabPage tabLoss;
        private System.Windows.Forms.TabPage tabBreakdown;
        private System.Windows.Forms.TextBox txtOverview;
        private System.Windows.Forms.TextBox txtErr;
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
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.txtOverview = new System.Windows.Forms.TextBox();
            this.tabErr = new System.Windows.Forms.TabPage();
            this.txtErr = new System.Windows.Forms.TextBox();
            this.tabInput = new System.Windows.Forms.TabPage();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.tabLoss = new System.Windows.Forms.TabPage();
            this.txtLoss = new System.Windows.Forms.TextBox();
            this.tabBreakdown = new System.Windows.Forms.TabPage();
            this.txtBreakdown = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.tabErr.SuspendLayout();
            this.tabInput.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabLoss.SuspendLayout();
            this.tabBreakdown.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabOverview);
            this.tabControl1.Controls.Add(this.tabErr);
            this.tabControl1.Controls.Add(this.tabInput);
            this.tabControl1.Controls.Add(this.tabResults);
            this.tabControl1.Controls.Add(this.tabLoss);
            this.tabControl1.Controls.Add(this.tabBreakdown);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(900, 650);
            this.tabControl1.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.txtOverview);
            this.tabOverview.Location = new System.Drawing.Point(4, 22);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverview.Size = new System.Drawing.Size(892, 624);
            this.tabOverview.TabIndex = 0;
            this.tabOverview.Text = "network.cdu";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // txtOverview
            // 
            this.txtOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOverview.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtOverview.Location = new System.Drawing.Point(3, 3);
            this.txtOverview.Multiline = true;
            this.txtOverview.Name = "txtOverview";
            this.txtOverview.ReadOnly = true;
            this.txtOverview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOverview.Size = new System.Drawing.Size(886, 618);
            this.txtOverview.TabIndex = 0;
            this.txtOverview.WordWrap = false;
            // 
            // tabErr
            // 
            this.tabErr.Controls.Add(this.txtErr);
            this.tabErr.Location = new System.Drawing.Point(4, 22);
            this.tabErr.Name = "tabErr";
            this.tabErr.Padding = new System.Windows.Forms.Padding(3);
            this.tabErr.Size = new System.Drawing.Size(892, 624);
            this.tabErr.TabIndex = 1;
            this.tabErr.Text = "network.err";
            this.tabErr.UseVisualStyleBackColor = true;
            // 
            // txtErr
            // 
            this.txtErr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErr.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtErr.Location = new System.Drawing.Point(3, 3);
            this.txtErr.Multiline = true;
            this.txtErr.Name = "txtErr";
            this.txtErr.ReadOnly = true;
            this.txtErr.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErr.Size = new System.Drawing.Size(886, 618);
            this.txtErr.TabIndex = 0;
            this.txtErr.WordWrap = false;
            // 
            // tabInput
            // 
            this.tabInput.Controls.Add(this.txtInput);
            this.tabInput.Location = new System.Drawing.Point(4, 22);
            this.tabInput.Name = "tabInput";
            this.tabInput.Padding = new System.Windows.Forms.Padding(3);
            this.tabInput.Size = new System.Drawing.Size(892, 624);
            this.tabInput.TabIndex = 2;
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
            this.txtInput.Size = new System.Drawing.Size(886, 618);
            this.txtInput.TabIndex = 0;
            this.txtInput.WordWrap = false;
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.txtResults);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabResults.Size = new System.Drawing.Size(892, 624);
            this.tabResults.TabIndex = 3;
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
            this.txtResults.Size = new System.Drawing.Size(886, 618);
            this.txtResults.TabIndex = 0;
            this.txtResults.WordWrap = false;
            // 
            // tabLoss
            // 
            this.tabLoss.Controls.Add(this.txtLoss);
            this.tabLoss.Location = new System.Drawing.Point(4, 22);
            this.tabLoss.Name = "tabLoss";
            this.tabLoss.Size = new System.Drawing.Size(892, 624);
            this.tabLoss.TabIndex = 4;
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
            this.txtLoss.Size = new System.Drawing.Size(892, 624);
            this.txtLoss.TabIndex = 0;
            this.txtLoss.WordWrap = false;
            // 
            // tabBreakdown
            // 
            this.tabBreakdown.Controls.Add(this.txtBreakdown);
            this.tabBreakdown.Location = new System.Drawing.Point(4, 22);
            this.tabBreakdown.Name = "tabBreakdown";
            this.tabBreakdown.Size = new System.Drawing.Size(892, 624);
            this.tabBreakdown.TabIndex = 5;
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
            this.txtBreakdown.Size = new System.Drawing.Size(892, 624);
            this.txtBreakdown.TabIndex = 0;
            this.txtBreakdown.WordWrap = false;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 650);
            this.Controls.Add(this.tabControl1);
            this.Name = "ReportForm";
            this.Text = "Отчёт сети";
            this.tabControl1.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.tabOverview.PerformLayout();
            this.tabErr.ResumeLayout(false);
            this.tabErr.PerformLayout();
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

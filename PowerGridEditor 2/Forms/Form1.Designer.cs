namespace PowerGridEditor
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonImportData = new System.Windows.Forms.Button();
            this.buttonOpenReport = new System.Windows.Forms.Button();
            this.buttonExportData = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAddShunt = new System.Windows.Forms.Button();
            this.buttonAddBranch = new System.Windows.Forms.Button();
            this.buttonAddBaseNode = new System.Windows.Forms.Button();
            this.buttonAddNode = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.buttonImportData);
            this.panel1.Controls.Add(this.buttonOpenReport);
            this.panel1.Controls.Add(this.buttonExportData);
            this.panel1.Controls.Add(this.buttonClearAll);
            this.panel1.Controls.Add(this.buttonDelete);
            this.panel1.Controls.Add(this.buttonAddShunt);
            this.panel1.Controls.Add(this.buttonAddBranch);
            this.panel1.Controls.Add(this.buttonAddBaseNode);
            this.panel1.Controls.Add(this.buttonAddNode);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1251, 50);
            this.panel1.TabIndex = 0;
            // 
            // buttonImportData
            // 
            this.buttonImportData.FlatAppearance.BorderSize = 2;
            this.buttonImportData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonImportData.Location = new System.Drawing.Point(1034, 12);
            this.buttonImportData.Name = "buttonImportData";
            this.buttonImportData.Size = new System.Drawing.Size(140, 30);
            this.buttonImportData.TabIndex = 7;
            this.buttonImportData.Text = "Импорт данных";
            this.buttonImportData.UseVisualStyleBackColor = true;
            this.buttonImportData.Click += new System.EventHandler(this.buttonImportData_Click);
            // 
            // buttonOpenReport
            // 
            this.buttonOpenReport.FlatAppearance.BorderSize = 2;
            this.buttonOpenReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOpenReport.Location = new System.Drawing.Point(1180, 12);
            this.buttonOpenReport.Name = "buttonOpenReport";
            this.buttonOpenReport.Size = new System.Drawing.Size(140, 30);
            this.buttonOpenReport.TabIndex = 8;
            this.buttonOpenReport.Text = "Окно отчёта";
            this.buttonOpenReport.UseVisualStyleBackColor = true;
            this.buttonOpenReport.Click += new System.EventHandler(this.buttonOpenReport_Click);
            // 
            // buttonExportData
            // 
            this.buttonExportData.FlatAppearance.BorderSize = 2;
            this.buttonExportData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExportData.Location = new System.Drawing.Point(888, 12);
            this.buttonExportData.Name = "buttonExportData";
            this.buttonExportData.Size = new System.Drawing.Size(140, 30);
            this.buttonExportData.TabIndex = 6;
            this.buttonExportData.Text = "Экспорт данных";
            this.buttonExportData.UseVisualStyleBackColor = true;
            this.buttonExportData.Click += new System.EventHandler(this.buttonExportData_Click);
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.FlatAppearance.BorderSize = 2;
            this.buttonClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClearAll.Location = new System.Drawing.Point(742, 12);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(140, 30);
            this.buttonClearAll.TabIndex = 5;
            this.buttonClearAll.Text = "Очистить всё";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.FlatAppearance.BorderSize = 2;
            this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDelete.Location = new System.Drawing.Point(596, 12);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(140, 30);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "Удалить";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAddShunt
            // 
            this.buttonAddShunt.FlatAppearance.BorderSize = 2;
            this.buttonAddShunt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddShunt.Location = new System.Drawing.Point(450, 12);
            this.buttonAddShunt.Name = "buttonAddShunt";
            this.buttonAddShunt.Size = new System.Drawing.Size(140, 30);
            this.buttonAddShunt.TabIndex = 3;
            this.buttonAddShunt.Text = "Шунт";
            this.buttonAddShunt.UseVisualStyleBackColor = true;
            this.buttonAddShunt.Click += new System.EventHandler(this.buttonAddShunt_Click);
            // 
            // buttonAddBranch
            // 
            this.buttonAddBranch.FlatAppearance.BorderSize = 2;
            this.buttonAddBranch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddBranch.Location = new System.Drawing.Point(304, 12);
            this.buttonAddBranch.Name = "buttonAddBranch";
            this.buttonAddBranch.Size = new System.Drawing.Size(140, 30);
            this.buttonAddBranch.TabIndex = 2;
            this.buttonAddBranch.Text = "Ветвь";
            this.buttonAddBranch.UseVisualStyleBackColor = true;
            this.buttonAddBranch.Click += new System.EventHandler(this.buttonAddBranch_Click);
            // 
            // buttonAddBaseNode
            // 
            this.buttonAddBaseNode.FlatAppearance.BorderSize = 2;
            this.buttonAddBaseNode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddBaseNode.Location = new System.Drawing.Point(158, 12);
            this.buttonAddBaseNode.Name = "buttonAddBaseNode";
            this.buttonAddBaseNode.Size = new System.Drawing.Size(140, 30);
            this.buttonAddBaseNode.TabIndex = 1;
            this.buttonAddBaseNode.Text = "Базисный Узел";
            this.buttonAddBaseNode.UseVisualStyleBackColor = true;
            this.buttonAddBaseNode.Click += new System.EventHandler(this.buttonAddBaseNode_Click);
            // 
            // buttonAddNode
            // 
            this.buttonAddNode.FlatAppearance.BorderSize = 2;
            this.buttonAddNode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddNode.Location = new System.Drawing.Point(12, 12);
            this.buttonAddNode.Name = "buttonAddNode";
            this.buttonAddNode.Size = new System.Drawing.Size(140, 30);
            this.buttonAddNode.TabIndex = 0;
            this.buttonAddNode.Text = "Узел";
            this.buttonAddNode.UseVisualStyleBackColor = true;
            this.buttonAddNode.Click += new System.EventHandler(this.buttonAddNode_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1251, 611);
            this.panel2.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 661);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Редактор электрической сети";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonAddNode;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAddShunt;
        private System.Windows.Forms.Button buttonAddBranch;
        private System.Windows.Forms.Button buttonAddBaseNode;
        private System.Windows.Forms.Button buttonExportData;
        private System.Windows.Forms.Button buttonImportData;
        private System.Windows.Forms.Button buttonOpenReport;
    }
}


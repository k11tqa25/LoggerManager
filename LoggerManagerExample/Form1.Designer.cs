namespace LoggerManagerExample
{
    partial class LoggerManagerExample
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonLog = new System.Windows.Forms.Button();
            this.textBoxMsg = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonShowResultDataset = new System.Windows.Forms.Button();
            this.buttonSaveResultDataset = new System.Windows.Forms.Button();
            this.mDataGrid = new System.Windows.Forms.DataGridView();
            this.buttonShowDefaultSettingsDataset = new System.Windows.Forms.Button();
            this.buttonSaveDefaultSettingsDataset = new System.Windows.Forms.Button();
            this.buttonReadDefaultSettingsAndDisplay = new System.Windows.Forms.Button();
            this.buttonSaveMultiDefulatSettings = new System.Windows.Forms.Button();
            this.buttonReadMultiDefaultSettings = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelLog1 = new System.Windows.Forms.Label();
            this.buttonLog2 = new System.Windows.Forms.Button();
            this.labelLog2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxTab = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonLog
            // 
            this.buttonLog.Location = new System.Drawing.Point(577, 19);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(65, 36);
            this.buttonLog.TabIndex = 0;
            this.buttonLog.Text = "LOG1";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonLog_Click);
            // 
            // textBoxMsg
            // 
            this.textBoxMsg.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBoxMsg.Location = new System.Drawing.Point(170, 19);
            this.textBoxMsg.Name = "textBoxMsg";
            this.textBoxMsg.Size = new System.Drawing.Size(401, 35);
            this.textBoxMsg.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(35, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Text to log:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(35, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Dataset Output:";
            // 
            // buttonShowResultDataset
            // 
            this.buttonShowResultDataset.Location = new System.Drawing.Point(199, 81);
            this.buttonShowResultDataset.Name = "buttonShowResultDataset";
            this.buttonShowResultDataset.Size = new System.Drawing.Size(169, 36);
            this.buttonShowResultDataset.TabIndex = 5;
            this.buttonShowResultDataset.Text = "Show Reuslt Dataset";
            this.buttonShowResultDataset.UseVisualStyleBackColor = true;
            this.buttonShowResultDataset.Click += new System.EventHandler(this.buttonShowResultDataset_Click);
            // 
            // buttonSaveResultDataset
            // 
            this.buttonSaveResultDataset.Location = new System.Drawing.Point(730, 174);
            this.buttonSaveResultDataset.Name = "buttonSaveResultDataset";
            this.buttonSaveResultDataset.Size = new System.Drawing.Size(263, 40);
            this.buttonSaveResultDataset.TabIndex = 7;
            this.buttonSaveResultDataset.Text = "Save result dataset";
            this.buttonSaveResultDataset.UseVisualStyleBackColor = true;
            this.buttonSaveResultDataset.Click += new System.EventHandler(this.buttonSaveResultDataset_Click);
            // 
            // mDataGrid
            // 
            this.mDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mDataGrid.Location = new System.Drawing.Point(39, 174);
            this.mDataGrid.Name = "mDataGrid";
            this.mDataGrid.RowHeadersWidth = 51;
            this.mDataGrid.RowTemplate.Height = 27;
            this.mDataGrid.Size = new System.Drawing.Size(674, 341);
            this.mDataGrid.TabIndex = 8;
            // 
            // buttonShowDefaultSettingsDataset
            // 
            this.buttonShowDefaultSettingsDataset.Location = new System.Drawing.Point(393, 81);
            this.buttonShowDefaultSettingsDataset.Name = "buttonShowDefaultSettingsDataset";
            this.buttonShowDefaultSettingsDataset.Size = new System.Drawing.Size(224, 36);
            this.buttonShowDefaultSettingsDataset.TabIndex = 9;
            this.buttonShowDefaultSettingsDataset.Text = "Show Defaut Settings Dataset";
            this.buttonShowDefaultSettingsDataset.UseVisualStyleBackColor = true;
            this.buttonShowDefaultSettingsDataset.Click += new System.EventHandler(this.buttonShowDefaultSettingsDataset_Click);
            // 
            // buttonSaveDefaultSettingsDataset
            // 
            this.buttonSaveDefaultSettingsDataset.Location = new System.Drawing.Point(730, 255);
            this.buttonSaveDefaultSettingsDataset.Name = "buttonSaveDefaultSettingsDataset";
            this.buttonSaveDefaultSettingsDataset.Size = new System.Drawing.Size(263, 44);
            this.buttonSaveDefaultSettingsDataset.TabIndex = 10;
            this.buttonSaveDefaultSettingsDataset.Text = "Save default settings dataset";
            this.buttonSaveDefaultSettingsDataset.UseVisualStyleBackColor = true;
            this.buttonSaveDefaultSettingsDataset.Click += new System.EventHandler(this.buttonSaveDefaultSettingsDataset_Click);
            // 
            // buttonReadDefaultSettingsAndDisplay
            // 
            this.buttonReadDefaultSettingsAndDisplay.Location = new System.Drawing.Point(730, 315);
            this.buttonReadDefaultSettingsAndDisplay.Name = "buttonReadDefaultSettingsAndDisplay";
            this.buttonReadDefaultSettingsAndDisplay.Size = new System.Drawing.Size(263, 48);
            this.buttonReadDefaultSettingsAndDisplay.TabIndex = 11;
            this.buttonReadDefaultSettingsAndDisplay.Text = "Read default settings and display";
            this.buttonReadDefaultSettingsAndDisplay.UseVisualStyleBackColor = true;
            this.buttonReadDefaultSettingsAndDisplay.Click += new System.EventHandler(this.buttonReadDefaultSettingsAndDisplay_Click);
            // 
            // buttonSaveMultiDefulatSettings
            // 
            this.buttonSaveMultiDefulatSettings.Location = new System.Drawing.Point(730, 395);
            this.buttonSaveMultiDefulatSettings.Name = "buttonSaveMultiDefulatSettings";
            this.buttonSaveMultiDefulatSettings.Size = new System.Drawing.Size(263, 44);
            this.buttonSaveMultiDefulatSettings.TabIndex = 12;
            this.buttonSaveMultiDefulatSettings.Text = "Save multiple default settings dataset";
            this.buttonSaveMultiDefulatSettings.UseVisualStyleBackColor = true;
            this.buttonSaveMultiDefulatSettings.Click += new System.EventHandler(this.buttonSaveMultiDefulatSettings_Click);
            // 
            // buttonReadMultiDefaultSettings
            // 
            this.buttonReadMultiDefaultSettings.Location = new System.Drawing.Point(730, 458);
            this.buttonReadMultiDefaultSettings.Name = "buttonReadMultiDefaultSettings";
            this.buttonReadMultiDefaultSettings.Size = new System.Drawing.Size(263, 48);
            this.buttonReadMultiDefaultSettings.TabIndex = 13;
            this.buttonReadMultiDefaultSettings.Text = "Read multiple default settings and display";
            this.buttonReadMultiDefaultSettings.UseVisualStyleBackColor = true;
            this.buttonReadMultiDefaultSettings.Click += new System.EventHandler(this.buttonReadMultiDefaultSettings_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(744, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Custom Function Logger Test:";
            // 
            // labelLog1
            // 
            this.labelLog1.AutoSize = true;
            this.labelLog1.Location = new System.Drawing.Point(801, 52);
            this.labelLog1.Name = "labelLog1";
            this.labelLog1.Size = new System.Drawing.Size(17, 15);
            this.labelLog1.TabIndex = 15;
            this.labelLog1.Text = "--";
            // 
            // buttonLog2
            // 
            this.buttonLog2.Location = new System.Drawing.Point(648, 19);
            this.buttonLog2.Name = "buttonLog2";
            this.buttonLog2.Size = new System.Drawing.Size(65, 36);
            this.buttonLog2.TabIndex = 16;
            this.buttonLog2.Text = "LOG2";
            this.buttonLog2.UseVisualStyleBackColor = true;
            this.buttonLog2.Click += new System.EventHandler(this.buttonLog2_Click);
            // 
            // labelLog2
            // 
            this.labelLog2.AutoSize = true;
            this.labelLog2.Location = new System.Drawing.Point(801, 86);
            this.labelLog2.Name = "labelLog2";
            this.labelLog2.Size = new System.Drawing.Size(17, 15);
            this.labelLog2.TabIndex = 17;
            this.labelLog2.Text = "--";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(744, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "Log1:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(744, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 15);
            this.label6.TabIndex = 19;
            this.label6.Text = "Log2:";
            // 
            // comboBoxTab
            // 
            this.comboBoxTab.FormattingEnabled = true;
            this.comboBoxTab.Location = new System.Drawing.Point(199, 144);
            this.comboBoxTab.Name = "comboBoxTab";
            this.comboBoxTab.Size = new System.Drawing.Size(188, 23);
            this.comboBoxTab.TabIndex = 20;
            this.comboBoxTab.SelectedIndexChanged += new System.EventHandler(this.comboBoxTab_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(36, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 24);
            this.label4.TabIndex = 21;
            this.label4.Text = "Select A Sheet:";
            // 
            // LoggerManagerExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 542);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxTab);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelLog2);
            this.Controls.Add(this.buttonLog2);
            this.Controls.Add(this.labelLog1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonReadMultiDefaultSettings);
            this.Controls.Add(this.buttonSaveMultiDefulatSettings);
            this.Controls.Add(this.buttonReadDefaultSettingsAndDisplay);
            this.Controls.Add(this.buttonSaveDefaultSettingsDataset);
            this.Controls.Add(this.buttonShowDefaultSettingsDataset);
            this.Controls.Add(this.mDataGrid);
            this.Controls.Add(this.buttonSaveResultDataset);
            this.Controls.Add(this.buttonShowResultDataset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMsg);
            this.Controls.Add(this.buttonLog);
            this.Name = "LoggerManagerExample";
            this.Text = "LoggerManagerExample";
            ((System.ComponentModel.ISupportInitialize)(this.mDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.TextBox textBoxMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonShowResultDataset;
        private System.Windows.Forms.Button buttonSaveResultDataset;
        private System.Windows.Forms.DataGridView mDataGrid;
        private System.Windows.Forms.Button buttonShowDefaultSettingsDataset;
        private System.Windows.Forms.Button buttonSaveDefaultSettingsDataset;
        private System.Windows.Forms.Button buttonReadDefaultSettingsAndDisplay;
        private System.Windows.Forms.Button buttonSaveMultiDefulatSettings;
        private System.Windows.Forms.Button buttonReadMultiDefaultSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelLog1;
        private System.Windows.Forms.Button buttonLog2;
        private System.Windows.Forms.Label labelLog2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxTab;
        private System.Windows.Forms.Label label4;
    }
}


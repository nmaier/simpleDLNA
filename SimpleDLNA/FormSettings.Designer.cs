namespace NMaier.SimpleDlna.GUI
{
  partial class FormSettings
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
      if (disposing && (components != null)) {
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
      this.components = new System.ComponentModel.Container();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.numericPort = new System.Windows.Forms.NumericUpDown();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.textCacheFile = new System.Windows.Forms.TextBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.buttonBrowseCacheFile = new System.Windows.Forms.Button();
      this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.checkStartMinimized = new System.Windows.Forms.CheckBox();
      this.checkFileLogging = new System.Windows.Forms.CheckBox();
      this.buttonOK = new System.Windows.Forms.Button();
      this.checkAutoStart = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      //
      // groupBox1
      //
      this.groupBox1.Controls.Add(this.numericPort);
      this.groupBox1.Location = new System.Drawing.Point(14, 14);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(303, 55);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Port";
      //
      // numericPort
      //
      this.numericPort.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::NMaier.SimpleDlna.GUI.Properties.Settings.Default, "port", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.numericPort.Location = new System.Drawing.Point(7, 22);
      this.numericPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.numericPort.Name = "numericPort";
      this.numericPort.Size = new System.Drawing.Size(80, 23);
      this.numericPort.TabIndex = 0;
      this.toolTip.SetToolTip(this.numericPort, "Port of the http server.\r\nLeave at 0 to automatically have a port selected on sta" +
  "rtup.\r\n\r\n(Requires restart)");
      this.numericPort.Value = global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.port;
      //
      // textCacheFile
      //
      this.textCacheFile.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::NMaier.SimpleDlna.GUI.Properties.Settings.Default, "cache", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textCacheFile.Location = new System.Drawing.Point(7, 22);
      this.textCacheFile.Name = "textCacheFile";
      this.textCacheFile.Size = new System.Drawing.Size(194, 23);
      this.textCacheFile.TabIndex = 1;
      this.textCacheFile.Text = global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.cache;
      this.toolTip.SetToolTip(this.textCacheFile, "Location of the cache directory.\r\nLeave blank to use the default location (TEMP)." +
  "\r\n\r\n(Requires restart)");
      //
      // groupBox2
      //
      this.groupBox2.Controls.Add(this.buttonBrowseCacheFile);
      this.groupBox2.Controls.Add(this.textCacheFile);
      this.groupBox2.Location = new System.Drawing.Point(14, 76);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(303, 55);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Cache directory";
      //
      // buttonBrowseCacheFile
      //
      this.buttonBrowseCacheFile.Location = new System.Drawing.Point(209, 20);
      this.buttonBrowseCacheFile.Name = "buttonBrowseCacheFile";
      this.buttonBrowseCacheFile.Size = new System.Drawing.Size(87, 27);
      this.buttonBrowseCacheFile.TabIndex = 0;
      this.buttonBrowseCacheFile.Text = "Browse";
      this.buttonBrowseCacheFile.UseVisualStyleBackColor = true;
      this.buttonBrowseCacheFile.Click += new System.EventHandler(this.buttonBrowseCacheFile_Click);
      //
      // checkStartMinimized
      //
      this.checkStartMinimized.AutoSize = true;
      this.checkStartMinimized.Checked = global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.startminimized;
      this.checkStartMinimized.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NMaier.SimpleDlna.GUI.Properties.Settings.Default, "startminimized", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkStartMinimized.Location = new System.Drawing.Point(14, 165);
      this.checkStartMinimized.Name = "checkStartMinimized";
      this.checkStartMinimized.Size = new System.Drawing.Size(109, 19);
      this.checkStartMinimized.TabIndex = 4;
      this.checkStartMinimized.Text = "Start minimized";
      this.checkStartMinimized.UseVisualStyleBackColor = true;
      //
      // checkFileLogging
      //
      this.checkFileLogging.AutoSize = true;
      this.checkFileLogging.Checked = global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.filelogging;
      this.checkFileLogging.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NMaier.SimpleDlna.GUI.Properties.Settings.Default, "filelogging", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.checkFileLogging.Location = new System.Drawing.Point(14, 138);
      this.checkFileLogging.Name = "checkFileLogging";
      this.checkFileLogging.Size = new System.Drawing.Size(200, 19);
      this.checkFileLogging.TabIndex = 3;
      this.checkFileLogging.Text = "Log diagnostic messages to a file";
      this.checkFileLogging.UseVisualStyleBackColor = true;
      //
      // buttonOK
      //
      this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.buttonOK.Location = new System.Drawing.Point(230, 192);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(87, 27);
      this.buttonOK.TabIndex = 0;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      //
      // checkAutoStart
      //
      this.checkAutoStart.AutoSize = true;
      this.checkAutoStart.Location = new System.Drawing.Point(14, 192);
      this.checkAutoStart.Name = "checkAutoStart";
      this.checkAutoStart.Size = new System.Drawing.Size(203, 19);
      this.checkAutoStart.TabIndex = 5;
      this.checkAutoStart.Text = "Start automatically with Windows";
      this.checkAutoStart.UseVisualStyleBackColor = true;
      this.checkAutoStart.CheckedChanged += new System.EventHandler(this.checkAutoStart_CheckedChanged);
      //
      // FormSettings
      //
      this.AcceptButton = this.buttonOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(331, 232);
      this.Controls.Add(this.checkAutoStart);
      this.Controls.Add(this.buttonOK);
      this.Controls.Add(this.checkFileLogging);
      this.Controls.Add(this.checkStartMinimized);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "FormSettings";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Settings";
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.NumericUpDown numericPort;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Button buttonBrowseCacheFile;
    private System.Windows.Forms.TextBox textCacheFile;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    private System.Windows.Forms.CheckBox checkStartMinimized;
    private System.Windows.Forms.CheckBox checkFileLogging;
    private System.Windows.Forms.Button buttonOK;
    private System.Windows.Forms.CheckBox checkAutoStart;
  }
}

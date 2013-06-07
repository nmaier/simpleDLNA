namespace NMaier.SimpleDlna.GUI
{
  partial class FormMain
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
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
        if (httpServer != null) {
          httpServer.Dispose();
          httpServer = null;
        }
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.listDescriptions = new System.Windows.Forms.ListView();
      this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colDirectories = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.listImages = new System.Windows.Forms.ImageList(this.components);
      this.buttonNewServer = new System.Windows.Forms.Button();
      this.buttonEdit = new System.Windows.Forms.Button();
      this.buttonStartStop = new System.Windows.Forms.Button();
      this.buttonRemove = new System.Windows.Forms.Button();
      this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.notifyContext = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.logger = new System.Windows.Forms.ListView();
      this.colLogLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colLogLogger = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colLogMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyContext.SuspendLayout();
      this.mainMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // listDescriptions
      // 
      this.listDescriptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listDescriptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colDirectories,
            this.colActive});
      this.listDescriptions.FullRowSelect = true;
      this.listDescriptions.HideSelection = false;
      this.listDescriptions.Location = new System.Drawing.Point(12, 27);
      this.listDescriptions.MultiSelect = false;
      this.listDescriptions.Name = "listDescriptions";
      this.listDescriptions.Size = new System.Drawing.Size(599, 184);
      this.listDescriptions.SmallImageList = this.listImages;
      this.listDescriptions.TabIndex = 0;
      this.listDescriptions.UseCompatibleStateImageBehavior = false;
      this.listDescriptions.View = System.Windows.Forms.View.Details;
      this.listDescriptions.SelectedIndexChanged += new System.EventHandler(this.ListDescriptions_SelectedIndexChanged);
      // 
      // colName
      // 
      this.colName.Text = "Name";
      // 
      // colDirectories
      // 
      this.colDirectories.Text = "Directories";
      this.colDirectories.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // colActive
      // 
      this.colActive.Text = "Active";
      // 
      // listImages
      // 
      this.listImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listImages.ImageStream")));
      this.listImages.TransparentColor = System.Drawing.Color.Transparent;
      this.listImages.Images.SetKeyName(0, "server");
      this.listImages.Images.SetKeyName(1, "active");
      this.listImages.Images.SetKeyName(2, "inactive");
      // 
      // buttonNewServer
      // 
      this.buttonNewServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonNewServer.Location = new System.Drawing.Point(536, 217);
      this.buttonNewServer.Name = "buttonNewServer";
      this.buttonNewServer.Size = new System.Drawing.Size(75, 23);
      this.buttonNewServer.TabIndex = 1;
      this.buttonNewServer.Text = "New Server";
      this.buttonNewServer.UseVisualStyleBackColor = true;
      this.buttonNewServer.Click += new System.EventHandler(this.ButtonNewServer_Click);
      // 
      // buttonEdit
      // 
      this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonEdit.Enabled = false;
      this.buttonEdit.Location = new System.Drawing.Point(93, 217);
      this.buttonEdit.Name = "buttonEdit";
      this.buttonEdit.Size = new System.Drawing.Size(75, 23);
      this.buttonEdit.TabIndex = 2;
      this.buttonEdit.Text = "Edit";
      this.buttonEdit.UseVisualStyleBackColor = true;
      this.buttonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
      // 
      // buttonStartStop
      // 
      this.buttonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStartStop.Enabled = false;
      this.buttonStartStop.Location = new System.Drawing.Point(12, 217);
      this.buttonStartStop.Name = "buttonStartStop";
      this.buttonStartStop.Size = new System.Drawing.Size(75, 23);
      this.buttonStartStop.TabIndex = 3;
      this.buttonStartStop.Text = "Start/Stop";
      this.buttonStartStop.UseVisualStyleBackColor = true;
      this.buttonStartStop.Click += new System.EventHandler(this.ButtonStartStop_Click);
      // 
      // buttonRemove
      // 
      this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRemove.Enabled = false;
      this.buttonRemove.Location = new System.Drawing.Point(174, 217);
      this.buttonRemove.Name = "buttonRemove";
      this.buttonRemove.Size = new System.Drawing.Size(75, 23);
      this.buttonRemove.TabIndex = 4;
      this.buttonRemove.Text = "Remove";
      this.buttonRemove.UseVisualStyleBackColor = true;
      this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
      // 
      // notifyIcon
      // 
      this.notifyIcon.ContextMenuStrip = this.notifyContext;
      this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
      // 
      // notifyContext
      // 
      this.notifyContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showContextMenuItem,
            this.toolStripMenuItem1,
            this.exitContextMenuItem});
      this.notifyContext.Name = "notifyContext";
      this.notifyContext.Size = new System.Drawing.Size(104, 54);
      // 
      // showContextMenuItem
      // 
      this.showContextMenuItem.Name = "showContextMenuItem";
      this.showContextMenuItem.Size = new System.Drawing.Size(103, 22);
      this.showContextMenuItem.Text = "Show";
      this.showContextMenuItem.Click += new System.EventHandler(this.notifyIcon_DoubleClick);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
      // 
      // exitContextMenuItem
      // 
      this.exitContextMenuItem.Name = "exitContextMenuItem";
      this.exitContextMenuItem.Size = new System.Drawing.Size(103, 22);
      this.exitContextMenuItem.Text = "Exit";
      this.exitContextMenuItem.Click += new System.EventHandler(this.exitContextMenuItem_Click);
      // 
      // mainMenu
      // 
      this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.mainMenu.Location = new System.Drawing.Point(0, 0);
      this.mainMenu.Name = "mainMenu";
      this.mainMenu.Size = new System.Drawing.Size(623, 24);
      this.mainMenu.TabIndex = 7;
      this.mainMenu.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openInBrowserToolStripMenuItem,
            this.toolStripMenuItem3,
            this.hideToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // openInBrowserToolStripMenuItem
      // 
      this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
      this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.openInBrowserToolStripMenuItem.Text = "Open in Browser";
      this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(158, 6);
      // 
      // hideToolStripMenuItem
      // 
      this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
      this.hideToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.hideToolStripMenuItem.Text = "Hide";
      this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.exitToolStripMenuItem.Text = "&Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitContextMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // logger
      // 
      this.logger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.logger.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLogLevel,
            this.colLogLogger,
            this.colLogMessage});
      this.logger.FullRowSelect = true;
      this.logger.HideSelection = false;
      this.logger.Location = new System.Drawing.Point(12, 246);
      this.logger.MultiSelect = false;
      this.logger.Name = "logger";
      this.logger.Size = new System.Drawing.Size(599, 148);
      this.logger.TabIndex = 8;
      this.logger.UseCompatibleStateImageBehavior = false;
      this.logger.View = System.Windows.Forms.View.Details;
      // 
      // colLogLevel
      // 
      this.colLogLevel.Text = "Level";
      // 
      // colLogLogger
      // 
      this.colLogLogger.Text = "Logger";
      // 
      // colLogMessage
      // 
      this.colLogMessage.Text = "Message";
      this.colLogMessage.Width = 200;
      // 
      // settingsToolStripMenuItem
      // 
      this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
      this.settingsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.settingsToolStripMenuItem.Text = "Settings";
      this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(623, 406);
      this.Controls.Add(this.logger);
      this.Controls.Add(this.mainMenu);
      this.Controls.Add(this.buttonRemove);
      this.Controls.Add(this.buttonStartStop);
      this.Controls.Add(this.buttonEdit);
      this.Controls.Add(this.buttonNewServer);
      this.Controls.Add(this.listDescriptions);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.mainMenu;
      this.Name = "FormMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SimpleDLNA";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
      this.Load += new System.EventHandler(this.FormMain_Load);
      this.Resize += new System.EventHandler(this.FormMain_Resize);
      this.notifyContext.ResumeLayout(false);
      this.mainMenu.ResumeLayout(false);
      this.mainMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView listDescriptions;
    private System.Windows.Forms.Button buttonNewServer;
    private System.Windows.Forms.ColumnHeader colName;
    private System.Windows.Forms.ColumnHeader colDirectories;
    private System.Windows.Forms.ColumnHeader colActive;
    private System.Windows.Forms.Button buttonEdit;
    private System.Windows.Forms.Button buttonStartStop;
    private System.Windows.Forms.Button buttonRemove;
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenuStrip notifyContext;
    private System.Windows.Forms.ToolStripMenuItem showContextMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem exitContextMenuItem;
    private System.Windows.Forms.MenuStrip mainMenu;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    private System.Windows.Forms.ListView logger;
    private System.Windows.Forms.ColumnHeader colLogLevel;
    private System.Windows.Forms.ColumnHeader colLogMessage;
    private System.Windows.Forms.ColumnHeader colLogLogger;
    private System.Windows.Forms.ImageList listImages;
    private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
  }
}


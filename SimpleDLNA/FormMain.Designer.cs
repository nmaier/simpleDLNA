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
        if (appenderTimer != null)
          appenderTimer.Dispose();
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
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.ctxStartStop = new System.Windows.Forms.ToolStripMenuItem();
      this.ctxEdit = new System.Windows.Forms.ToolStripMenuItem();
      this.ctxRemove = new System.Windows.Forms.ToolStripMenuItem();
      this.ctxRescan = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
      this.ctxNewServer = new System.Windows.Forms.ToolStripMenuItem();
      this.listImages = new System.Windows.Forms.ImageList(this.components);
      this.buttonNewServer = new System.Windows.Forms.Button();
      this.buttonEdit = new System.Windows.Forms.Button();
      this.buttonStartStop = new System.Windows.Forms.Button();
      this.buttonRemove = new System.Windows.Forms.Button();
      this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.notifyContext = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.rescanAllContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ContextSeperatorPre = new System.Windows.Forms.ToolStripSeparator();
      this.ContextSeperatorPost = new System.Windows.Forms.ToolStripSeparator();
      this.exitContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.dropCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
      this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.homepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.logger = new System.Windows.Forms.ListView();
      this.colLogTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colLogLogger = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colLogMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.buttonRescan = new System.Windows.Forms.Button();
      this.contextMenu.SuspendLayout();
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
      this.listDescriptions.ContextMenuStrip = this.contextMenu;
      this.listDescriptions.FullRowSelect = true;
      this.listDescriptions.HideSelection = false;
      this.listDescriptions.Location = new System.Drawing.Point(14, 31);
      this.listDescriptions.MultiSelect = false;
      this.listDescriptions.Name = "listDescriptions";
      this.listDescriptions.Size = new System.Drawing.Size(698, 212);
      this.listDescriptions.SmallImageList = this.listImages;
      this.listDescriptions.TabIndex = 5;
      this.listDescriptions.UseCompatibleStateImageBehavior = false;
      this.listDescriptions.View = System.Windows.Forms.View.Details;
      this.listDescriptions.SelectedIndexChanged += new System.EventHandler(this.ListDescriptions_SelectedIndexChanged);
      this.listDescriptions.DoubleClick += new System.EventHandler(this.listDescriptions_DoubleClick);
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
      // contextMenu
      //
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxStartStop,
            this.ctxEdit,
            this.ctxRemove,
            this.ctxRescan,
            this.toolStripMenuItem5,
            this.ctxNewServer});
      this.contextMenu.Name = "contextMenu";
      this.contextMenu.Size = new System.Drawing.Size(134, 120);
      //
      // ctxStartStop
      //
      this.ctxStartStop.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.active;
      this.ctxStartStop.Name = "ctxStartStop";
      this.ctxStartStop.Size = new System.Drawing.Size(133, 22);
      this.ctxStartStop.Text = "Start/Stop";
      this.ctxStartStop.Click += new System.EventHandler(this.ButtonStartStop_Click);
      //
      // ctxEdit
      //
      this.ctxEdit.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.edit;
      this.ctxEdit.Name = "ctxEdit";
      this.ctxEdit.Size = new System.Drawing.Size(133, 22);
      this.ctxEdit.Text = "Edit";
      this.ctxEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
      //
      // ctxRemove
      //
      this.ctxRemove.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.remove;
      this.ctxRemove.Name = "ctxRemove";
      this.ctxRemove.Size = new System.Drawing.Size(133, 22);
      this.ctxRemove.Text = "Remove";
      this.ctxRemove.Click += new System.EventHandler(this.buttonRemove_Click);
      //
      // ctxRescan
      //
      this.ctxRescan.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.refreshing;
      this.ctxRescan.Name = "ctxRescan";
      this.ctxRescan.Size = new System.Drawing.Size(133, 22);
      this.ctxRescan.Text = "Rescan";
      this.ctxRescan.Click += new System.EventHandler(this.buttonRescan_Click);
      //
      // toolStripMenuItem5
      //
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(130, 6);
      //
      // ctxNewServer
      //
      this.ctxNewServer.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.add;
      this.ctxNewServer.Name = "ctxNewServer";
      this.ctxNewServer.Size = new System.Drawing.Size(133, 22);
      this.ctxNewServer.Text = "New Server";
      this.ctxNewServer.Click += new System.EventHandler(this.ButtonNewServer_Click);
      //
      // listImages
      //
      this.listImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.listImages.ImageSize = new System.Drawing.Size(16, 16);
      this.listImages.TransparentColor = System.Drawing.Color.Transparent;
      //
      // buttonNewServer
      //
      this.buttonNewServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonNewServer.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.add;
      this.buttonNewServer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonNewServer.Location = new System.Drawing.Point(625, 250);
      this.buttonNewServer.Name = "buttonNewServer";
      this.buttonNewServer.Size = new System.Drawing.Size(87, 27);
      this.buttonNewServer.TabIndex = 0;
      this.buttonNewServer.Text = "New";
      this.buttonNewServer.UseVisualStyleBackColor = true;
      this.buttonNewServer.Click += new System.EventHandler(this.ButtonNewServer_Click);
      //
      // buttonEdit
      //
      this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonEdit.Enabled = false;
      this.buttonEdit.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.edit;
      this.buttonEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonEdit.Location = new System.Drawing.Point(108, 250);
      this.buttonEdit.Name = "buttonEdit";
      this.buttonEdit.Size = new System.Drawing.Size(87, 27);
      this.buttonEdit.TabIndex = 2;
      this.buttonEdit.Text = "Edit";
      this.buttonEdit.UseVisualStyleBackColor = true;
      this.buttonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
      //
      // buttonStartStop
      //
      this.buttonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStartStop.Enabled = false;
      this.buttonStartStop.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.active;
      this.buttonStartStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonStartStop.Location = new System.Drawing.Point(14, 250);
      this.buttonStartStop.Name = "buttonStartStop";
      this.buttonStartStop.Size = new System.Drawing.Size(87, 27);
      this.buttonStartStop.TabIndex = 1;
      this.buttonStartStop.Text = "Start";
      this.buttonStartStop.UseVisualStyleBackColor = true;
      this.buttonStartStop.Click += new System.EventHandler(this.ButtonStartStop_Click);
      //
      // buttonRemove
      //
      this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRemove.Enabled = false;
      this.buttonRemove.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.remove;
      this.buttonRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonRemove.Location = new System.Drawing.Point(203, 250);
      this.buttonRemove.Name = "buttonRemove";
      this.buttonRemove.Size = new System.Drawing.Size(87, 27);
      this.buttonRemove.TabIndex = 3;
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
            this.rescanAllContextMenuItem,
            this.ContextSeperatorPre,
            this.ContextSeperatorPost,
            this.exitContextMenuItem});
      this.notifyContext.Name = "notifyContext";
      this.notifyContext.Size = new System.Drawing.Size(153, 104);
      this.notifyContext.Opening += new System.ComponentModel.CancelEventHandler(this.notifyContext_Opening);
      //
      // showContextMenuItem
      //
      this.showContextMenuItem.Name = "showContextMenuItem";
      this.showContextMenuItem.Size = new System.Drawing.Size(152, 22);
      this.showContextMenuItem.Text = "Show";
      this.showContextMenuItem.Click += new System.EventHandler(this.notifyIcon_DoubleClick);
      //
      // rescanAllContextMenuItem
      //
      this.rescanAllContextMenuItem.Name = "rescanAllContextMenuItem";
      this.rescanAllContextMenuItem.Size = new System.Drawing.Size(152, 22);
      this.rescanAllContextMenuItem.Text = "Rescan all";
      this.rescanAllContextMenuItem.Click += new System.EventHandler(this.rescanAllContextMenuItem_Click);
      //
      // ContextSeperatorPre
      //
      this.ContextSeperatorPre.Name = "ContextSeperatorPre";
      this.ContextSeperatorPre.Size = new System.Drawing.Size(149, 6);
      //
      // ContextSeperatorPost
      //
      this.ContextSeperatorPost.Name = "ContextSeperatorPost";
      this.ContextSeperatorPost.Size = new System.Drawing.Size(149, 6);
      //
      // exitContextMenuItem
      //
      this.exitContextMenuItem.Name = "exitContextMenuItem";
      this.exitContextMenuItem.Size = new System.Drawing.Size(152, 22);
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
      this.mainMenu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
      this.mainMenu.Size = new System.Drawing.Size(727, 24);
      this.mainMenu.TabIndex = 6;
      this.mainMenu.Text = "menuStrip1";
      //
      // fileToolStripMenuItem
      //
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newServerToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openInBrowserToolStripMenuItem,
            this.toolStripMenuItem3,
            this.dropCacheToolStripMenuItem,
            this.toolStripMenuItem4,
            this.hideToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      //
      // newServerToolStripMenuItem
      //
      this.newServerToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.add;
      this.newServerToolStripMenuItem.Name = "newServerToolStripMenuItem";
      this.newServerToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.newServerToolStripMenuItem.Text = "New Server";
      this.newServerToolStripMenuItem.Click += new System.EventHandler(this.ButtonNewServer_Click);
      //
      // settingsToolStripMenuItem
      //
      this.settingsToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.preferences;
      this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
      this.settingsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.settingsToolStripMenuItem.Text = "Settings";
      this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
      //
      // toolStripMenuItem2
      //
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
      //
      // openInBrowserToolStripMenuItem
      //
      this.openInBrowserToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.go;
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
      // dropCacheToolStripMenuItem
      //
      this.dropCacheToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.delete;
      this.dropCacheToolStripMenuItem.Name = "dropCacheToolStripMenuItem";
      this.dropCacheToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.dropCacheToolStripMenuItem.Text = "Drop cache";
      this.dropCacheToolStripMenuItem.Click += new System.EventHandler(this.dropCacheToolStripMenuItem_Click);
      //
      // toolStripMenuItem4
      //
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(158, 6);
      //
      // hideToolStripMenuItem
      //
      this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
      this.hideToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.hideToolStripMenuItem.Text = "Hide";
      this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
      //
      // exitToolStripMenuItem
      //
      this.exitToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.close;
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.exitToolStripMenuItem.Text = "&Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitContextMenuItem_Click);
      //
      // helpToolStripMenuItem
      //
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homepageToolStripMenuItem,
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      //
      // homepageToolStripMenuItem
      //
      this.homepageToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.homepage;
      this.homepageToolStripMenuItem.Name = "homepageToolStripMenuItem";
      this.homepageToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
      this.homepageToolStripMenuItem.Text = "Homepage";
      this.homepageToolStripMenuItem.Click += new System.EventHandler(this.homepageToolStripMenuItem_Click);
      //
      // aboutToolStripMenuItem
      //
      this.aboutToolStripMenuItem.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.about;
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      //
      // logger
      //
      this.logger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.logger.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLogTime,
            this.colLogLogger,
            this.colLogMessage});
      this.logger.FullRowSelect = true;
      this.logger.HideSelection = false;
      this.logger.Location = new System.Drawing.Point(14, 284);
      this.logger.MultiSelect = false;
      this.logger.Name = "logger";
      this.logger.Size = new System.Drawing.Size(698, 170);
      this.logger.SmallImageList = this.listImages;
      this.logger.TabIndex = 7;
      this.logger.UseCompatibleStateImageBehavior = false;
      this.logger.View = System.Windows.Forms.View.Details;
      //
      // colLogTime
      //
      this.colLogTime.Text = "Time";
      this.colLogTime.Width = 80;
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
      // buttonRescan
      //
      this.buttonRescan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRescan.Enabled = false;
      this.buttonRescan.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.refreshing;
      this.buttonRescan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonRescan.Location = new System.Drawing.Point(297, 250);
      this.buttonRescan.Name = "buttonRescan";
      this.buttonRescan.Size = new System.Drawing.Size(87, 27);
      this.buttonRescan.TabIndex = 4;
      this.buttonRescan.Text = "Rescan";
      this.buttonRescan.UseVisualStyleBackColor = true;
      this.buttonRescan.Click += new System.EventHandler(this.buttonRescan_Click);
      //
      // FormMain
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(727, 468);
      this.Controls.Add(this.buttonRescan);
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
      this.Resize += new System.EventHandler(this.FormMain_Resize);
      this.contextMenu.ResumeLayout(false);
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
    private System.Windows.Forms.ToolStripSeparator ContextSeperatorPost;
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
    private System.Windows.Forms.ColumnHeader colLogMessage;
    private System.Windows.Forms.ColumnHeader colLogLogger;
    private System.Windows.Forms.ImageList listImages;
    private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    private System.Windows.Forms.Button buttonRescan;
    private System.Windows.Forms.ColumnHeader colLogTime;
    private System.Windows.Forms.ToolStripMenuItem dropCacheToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
    private System.Windows.Forms.ToolStripMenuItem homepageToolStripMenuItem;
    private System.Windows.Forms.ContextMenuStrip contextMenu;
    private System.Windows.Forms.ToolStripMenuItem ctxStartStop;
    private System.Windows.Forms.ToolStripMenuItem ctxEdit;
    private System.Windows.Forms.ToolStripMenuItem ctxRemove;
    private System.Windows.Forms.ToolStripMenuItem ctxRescan;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
    private System.Windows.Forms.ToolStripMenuItem ctxNewServer;
    private System.Windows.Forms.ToolStripMenuItem newServerToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator ContextSeperatorPre;
    private System.Windows.Forms.ToolStripMenuItem rescanAllContextMenuItem;
  }
}


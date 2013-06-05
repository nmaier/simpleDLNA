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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.ListDescriptions = new System.Windows.Forms.ListView();
      this.ColName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ColDirectories = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ColActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ButtonNewServer = new System.Windows.Forms.Button();
      this.ButtonEdit = new System.Windows.Forms.Button();
      this.ButtonStartStop = new System.Windows.Forms.Button();
      this.ButtonRemove = new System.Windows.Forms.Button();
      this.Logger = new System.Windows.Forms.ListBox();
      this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.notifyContext = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyContext.SuspendLayout();
      this.mainMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // ListDescriptions
      // 
      this.ListDescriptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ListDescriptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColName,
            this.ColDirectories,
            this.ColActive});
      this.ListDescriptions.FullRowSelect = true;
      this.ListDescriptions.HideSelection = false;
      this.ListDescriptions.Location = new System.Drawing.Point(12, 27);
      this.ListDescriptions.MultiSelect = false;
      this.ListDescriptions.Name = "ListDescriptions";
      this.ListDescriptions.Size = new System.Drawing.Size(599, 184);
      this.ListDescriptions.TabIndex = 0;
      this.ListDescriptions.UseCompatibleStateImageBehavior = false;
      this.ListDescriptions.View = System.Windows.Forms.View.Details;
      this.ListDescriptions.SelectedIndexChanged += new System.EventHandler(this.ListDescriptions_SelectedIndexChanged);
      // 
      // ColName
      // 
      this.ColName.Text = "Name";
      // 
      // ColDirectories
      // 
      this.ColDirectories.Text = "Directories";
      this.ColDirectories.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // ColActive
      // 
      this.ColActive.Text = "Active";
      // 
      // ButtonNewServer
      // 
      this.ButtonNewServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonNewServer.Location = new System.Drawing.Point(536, 217);
      this.ButtonNewServer.Name = "ButtonNewServer";
      this.ButtonNewServer.Size = new System.Drawing.Size(75, 23);
      this.ButtonNewServer.TabIndex = 1;
      this.ButtonNewServer.Text = "New Server";
      this.ButtonNewServer.UseVisualStyleBackColor = true;
      this.ButtonNewServer.Click += new System.EventHandler(this.ButtonNewServer_Click);
      // 
      // ButtonEdit
      // 
      this.ButtonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.ButtonEdit.Enabled = false;
      this.ButtonEdit.Location = new System.Drawing.Point(93, 217);
      this.ButtonEdit.Name = "ButtonEdit";
      this.ButtonEdit.Size = new System.Drawing.Size(75, 23);
      this.ButtonEdit.TabIndex = 2;
      this.ButtonEdit.Text = "Edit";
      this.ButtonEdit.UseVisualStyleBackColor = true;
      this.ButtonEdit.Click += new System.EventHandler(this.ButtonEdit_Click);
      // 
      // ButtonStartStop
      // 
      this.ButtonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.ButtonStartStop.Enabled = false;
      this.ButtonStartStop.Location = new System.Drawing.Point(12, 217);
      this.ButtonStartStop.Name = "ButtonStartStop";
      this.ButtonStartStop.Size = new System.Drawing.Size(75, 23);
      this.ButtonStartStop.TabIndex = 3;
      this.ButtonStartStop.Text = "Start/Stop";
      this.ButtonStartStop.UseVisualStyleBackColor = true;
      this.ButtonStartStop.Click += new System.EventHandler(this.ButtonStartStop_Click);
      // 
      // ButtonRemove
      // 
      this.ButtonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.ButtonRemove.Enabled = false;
      this.ButtonRemove.Location = new System.Drawing.Point(174, 217);
      this.ButtonRemove.Name = "ButtonRemove";
      this.ButtonRemove.Size = new System.Drawing.Size(75, 23);
      this.ButtonRemove.TabIndex = 4;
      this.ButtonRemove.Text = "Remove";
      this.ButtonRemove.UseVisualStyleBackColor = true;
      // 
      // Logger
      // 
      this.Logger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.Logger.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Logger.FormattingEnabled = true;
      this.Logger.Location = new System.Drawing.Point(12, 246);
      this.Logger.Name = "Logger";
      this.Logger.Size = new System.Drawing.Size(599, 147);
      this.Logger.TabIndex = 5;
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
            this.openInBrowserToolStripMenuItem,
            this.toolStripMenuItem3,
            this.hideToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.exitToolStripMenuItem.Text = "&Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitContextMenuItem_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
      // 
      // hideToolStripMenuItem
      // 
      this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
      this.hideToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.hideToolStripMenuItem.Text = "Hide";
      this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
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
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(158, 6);
      // 
      // openInBrowserToolStripMenuItem
      // 
      this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
      this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.openInBrowserToolStripMenuItem.Text = "Open in Browser";
      this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(623, 406);
      this.Controls.Add(this.mainMenu);
      this.Controls.Add(this.Logger);
      this.Controls.Add(this.ButtonRemove);
      this.Controls.Add(this.ButtonStartStop);
      this.Controls.Add(this.ButtonEdit);
      this.Controls.Add(this.ButtonNewServer);
      this.Controls.Add(this.ListDescriptions);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.mainMenu;
      this.Name = "FormMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SimpleDLNA";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
      this.Resize += new System.EventHandler(this.FormMain_Resize);
      this.notifyContext.ResumeLayout(false);
      this.mainMenu.ResumeLayout(false);
      this.mainMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView ListDescriptions;
    private System.Windows.Forms.Button ButtonNewServer;
    private System.Windows.Forms.ColumnHeader ColName;
    private System.Windows.Forms.ColumnHeader ColDirectories;
    private System.Windows.Forms.ColumnHeader ColActive;
    private System.Windows.Forms.Button ButtonEdit;
    private System.Windows.Forms.Button ButtonStartStop;
    private System.Windows.Forms.Button ButtonRemove;
    private System.Windows.Forms.ListBox Logger;
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
  }
}


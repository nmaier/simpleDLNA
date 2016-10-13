namespace NMaier.SimpleDlna.GUI
{
  sealed partial class FormServer
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
      this.groupName = new System.Windows.Forms.GroupBox();
      this.textName = new System.Windows.Forms.TextBox();
      this.groupOrder = new System.Windows.Forms.GroupBox();
      this.checkOrderDescending = new System.Windows.Forms.CheckBox();
      this.comboOrder = new System.Windows.Forms.ComboBox();
      this.groupTypes = new System.Windows.Forms.GroupBox();
      this.checkImages = new System.Windows.Forms.CheckBox();
      this.checkAudio = new System.Windows.Forms.CheckBox();
      this.checkVideo = new System.Windows.Forms.CheckBox();
      this.groupDirectories = new System.Windows.Forms.GroupBox();
      this.listDirectoriesAnchor = new System.Windows.Forms.Label();
      this.buttonRemoveDirectory = new System.Windows.Forms.Button();
      this.buttonAddDirectory = new System.Windows.Forms.Button();
      this.listDirectories = new System.Windows.Forms.ListView();
      this.colDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.buttonCancel = new System.Windows.Forms.Button();
      this.buttonAccept = new System.Windows.Forms.Button();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.tabbedOptions = new System.Windows.Forms.TabControl();
      this.tabPageViews = new System.Windows.Forms.TabPage();
      this.comboNewView = new System.Windows.Forms.ComboBox();
      this.buttonViewDown = new System.Windows.Forms.Button();
      this.buttonViewUp = new System.Windows.Forms.Button();
      this.buttonRemoveView = new System.Windows.Forms.Button();
      this.buttonAddView = new System.Windows.Forms.Button();
      this.listViews = new System.Windows.Forms.ListView();
      this.colViewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colViewDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.tabPageRestrictions = new System.Windows.Forms.TabPage();
      this.textRestriction = new System.Windows.Forms.TextBox();
      this.comboNewRestriction = new System.Windows.Forms.ComboBox();
      this.listRestrictions = new System.Windows.Forms.ListView();
      this.colRestriction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colRestrictionType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.buttonRemoveRestriction = new System.Windows.Forms.Button();
      this.buttonAddRestriction = new System.Windows.Forms.Button();
      this.groupName.SuspendLayout();
      this.groupOrder.SuspendLayout();
      this.groupTypes.SuspendLayout();
      this.groupDirectories.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabbedOptions.SuspendLayout();
      this.tabPageViews.SuspendLayout();
      this.tabPageRestrictions.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupName
      // 
      this.groupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupName.Controls.Add(this.textName);
      this.groupName.Location = new System.Drawing.Point(12, 12);
      this.groupName.Name = "groupName";
      this.groupName.Size = new System.Drawing.Size(520, 46);
      this.groupName.TabIndex = 2;
      this.groupName.TabStop = false;
      this.groupName.Text = "Name";
      // 
      // textName
      // 
      this.textName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textName.Location = new System.Drawing.Point(6, 19);
      this.textName.Name = "textName";
      this.textName.Size = new System.Drawing.Size(485, 20);
      this.textName.TabIndex = 0;
      this.textName.Validating += new System.ComponentModel.CancelEventHandler(this.textName_Validating);
      // 
      // groupOrder
      // 
      this.groupOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupOrder.Controls.Add(this.checkOrderDescending);
      this.groupOrder.Controls.Add(this.comboOrder);
      this.groupOrder.Location = new System.Drawing.Point(12, 64);
      this.groupOrder.Name = "groupOrder";
      this.groupOrder.Size = new System.Drawing.Size(520, 46);
      this.groupOrder.TabIndex = 3;
      this.groupOrder.TabStop = false;
      this.groupOrder.Text = "Order";
      // 
      // checkOrderDescending
      // 
      this.checkOrderDescending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.checkOrderDescending.AutoSize = true;
      this.checkOrderDescending.Location = new System.Drawing.Point(431, 19);
      this.checkOrderDescending.Name = "checkOrderDescending";
      this.checkOrderDescending.Size = new System.Drawing.Size(83, 17);
      this.checkOrderDescending.TabIndex = 1;
      this.checkOrderDescending.Text = "Descending";
      this.checkOrderDescending.UseVisualStyleBackColor = true;
      // 
      // comboOrder
      // 
      this.comboOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboOrder.FormattingEnabled = true;
      this.comboOrder.Location = new System.Drawing.Point(6, 19);
      this.comboOrder.Name = "comboOrder";
      this.comboOrder.Size = new System.Drawing.Size(419, 21);
      this.comboOrder.TabIndex = 0;
      // 
      // groupTypes
      // 
      this.groupTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupTypes.Controls.Add(this.checkImages);
      this.groupTypes.Controls.Add(this.checkAudio);
      this.groupTypes.Controls.Add(this.checkVideo);
      this.groupTypes.Location = new System.Drawing.Point(12, 116);
      this.groupTypes.Name = "groupTypes";
      this.groupTypes.Size = new System.Drawing.Size(520, 46);
      this.groupTypes.TabIndex = 4;
      this.groupTypes.TabStop = false;
      this.groupTypes.Text = "Types";
      this.groupTypes.Validating += new System.ComponentModel.CancelEventHandler(this.checkTypes_Validating);
      // 
      // checkImages
      // 
      this.checkImages.AutoSize = true;
      this.checkImages.Location = new System.Drawing.Point(133, 19);
      this.checkImages.Name = "checkImages";
      this.checkImages.Size = new System.Drawing.Size(60, 17);
      this.checkImages.TabIndex = 2;
      this.checkImages.Text = "Images";
      this.checkImages.UseVisualStyleBackColor = true;
      // 
      // checkAudio
      // 
      this.checkAudio.AutoSize = true;
      this.checkAudio.Location = new System.Drawing.Point(74, 19);
      this.checkAudio.Name = "checkAudio";
      this.checkAudio.Size = new System.Drawing.Size(53, 17);
      this.checkAudio.TabIndex = 1;
      this.checkAudio.Text = "Audio";
      this.checkAudio.UseVisualStyleBackColor = true;
      // 
      // checkVideo
      // 
      this.checkVideo.AutoSize = true;
      this.checkVideo.Location = new System.Drawing.Point(15, 19);
      this.checkVideo.Name = "checkVideo";
      this.checkVideo.Size = new System.Drawing.Size(53, 17);
      this.checkVideo.TabIndex = 0;
      this.checkVideo.Text = "Video";
      this.checkVideo.UseVisualStyleBackColor = true;
      // 
      // groupDirectories
      // 
      this.groupDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupDirectories.Controls.Add(this.listDirectoriesAnchor);
      this.groupDirectories.Controls.Add(this.buttonRemoveDirectory);
      this.groupDirectories.Controls.Add(this.buttonAddDirectory);
      this.groupDirectories.Controls.Add(this.listDirectories);
      this.groupDirectories.Location = new System.Drawing.Point(13, 334);
      this.groupDirectories.Name = "groupDirectories";
      this.groupDirectories.Size = new System.Drawing.Size(519, 138);
      this.groupDirectories.TabIndex = 6;
      this.groupDirectories.TabStop = false;
      this.groupDirectories.Text = "Directories";
      // 
      // listDirectoriesAnchor
      // 
      this.listDirectoriesAnchor.AutoSize = true;
      this.listDirectoriesAnchor.Location = new System.Drawing.Point(441, 74);
      this.listDirectoriesAnchor.Name = "listDirectoriesAnchor";
      this.listDirectoriesAnchor.Size = new System.Drawing.Size(0, 13);
      this.listDirectoriesAnchor.TabIndex = 2;
      // 
      // buttonRemoveDirectory
      // 
      this.buttonRemoveDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRemoveDirectory.Location = new System.Drawing.Point(438, 48);
      this.buttonRemoveDirectory.Name = "buttonRemoveDirectory";
      this.buttonRemoveDirectory.Size = new System.Drawing.Size(75, 23);
      this.buttonRemoveDirectory.TabIndex = 2;
      this.buttonRemoveDirectory.Text = "Remove";
      this.buttonRemoveDirectory.UseVisualStyleBackColor = true;
      this.buttonRemoveDirectory.Click += new System.EventHandler(this.buttonRemoveDirectory_Click);
      // 
      // buttonAddDirectory
      // 
      this.buttonAddDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAddDirectory.Location = new System.Drawing.Point(438, 19);
      this.buttonAddDirectory.Name = "buttonAddDirectory";
      this.buttonAddDirectory.Size = new System.Drawing.Size(75, 23);
      this.buttonAddDirectory.TabIndex = 0;
      this.buttonAddDirectory.Text = "Add";
      this.buttonAddDirectory.UseVisualStyleBackColor = true;
      this.buttonAddDirectory.Click += new System.EventHandler(this.buttonAddDirectory_Click);
      // 
      // listDirectories
      // 
      this.listDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listDirectories.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDirectory});
      this.listDirectories.Location = new System.Drawing.Point(6, 19);
      this.listDirectories.Name = "listDirectories";
      this.listDirectories.Size = new System.Drawing.Size(429, 113);
      this.listDirectories.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.listDirectories.TabIndex = 3;
      this.listDirectories.UseCompatibleStateImageBehavior = false;
      this.listDirectories.View = System.Windows.Forms.View.Details;
      this.listDirectories.Validating += new System.ComponentModel.CancelEventHandler(this.listDirectories_Validating);
      // 
      // colDirectory
      // 
      this.colDirectory.Text = "Directory";
      this.colDirectory.Width = 200;
      // 
      // buttonCancel
      // 
      this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCancel.Location = new System.Drawing.Point(457, 478);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 1;
      this.buttonCancel.Text = "&Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      // 
      // buttonAccept
      // 
      this.buttonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.buttonAccept.Location = new System.Drawing.Point(376, 478);
      this.buttonAccept.Name = "buttonAccept";
      this.buttonAccept.Size = new System.Drawing.Size(75, 23);
      this.buttonAccept.TabIndex = 0;
      this.buttonAccept.Text = "&OK";
      this.buttonAccept.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // tabbedOptions
      // 
      this.tabbedOptions.Controls.Add(this.tabPageViews);
      this.tabbedOptions.Controls.Add(this.tabPageRestrictions);
      this.tabbedOptions.Location = new System.Drawing.Point(12, 168);
      this.tabbedOptions.Name = "tabbedOptions";
      this.tabbedOptions.SelectedIndex = 0;
      this.tabbedOptions.Size = new System.Drawing.Size(520, 160);
      this.tabbedOptions.TabIndex = 6;
      // 
      // tabPageViews
      // 
      this.tabPageViews.Controls.Add(this.comboNewView);
      this.tabPageViews.Controls.Add(this.buttonViewDown);
      this.tabPageViews.Controls.Add(this.buttonViewUp);
      this.tabPageViews.Controls.Add(this.buttonRemoveView);
      this.tabPageViews.Controls.Add(this.buttonAddView);
      this.tabPageViews.Controls.Add(this.listViews);
      this.tabPageViews.Location = new System.Drawing.Point(4, 22);
      this.tabPageViews.Name = "tabPageViews";
      this.tabPageViews.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageViews.Size = new System.Drawing.Size(512, 134);
      this.tabPageViews.TabIndex = 0;
      this.tabPageViews.Text = "Views";
      this.tabPageViews.UseVisualStyleBackColor = true;
      // 
      // comboNewView
      // 
      this.comboNewView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboNewView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboNewView.FormattingEnabled = true;
      this.comboNewView.Location = new System.Drawing.Point(6, 6);
      this.comboNewView.Name = "comboNewView";
      this.comboNewView.Size = new System.Drawing.Size(419, 21);
      this.comboNewView.TabIndex = 0;
      // 
      // buttonViewDown
      // 
      this.buttonViewDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonViewDown.Location = new System.Drawing.Point(431, 102);
      this.buttonViewDown.Name = "buttonViewDown";
      this.buttonViewDown.Size = new System.Drawing.Size(75, 23);
      this.buttonViewDown.TabIndex = 4;
      this.buttonViewDown.Text = "Down";
      this.buttonViewDown.UseVisualStyleBackColor = true;
      // 
      // buttonViewUp
      // 
      this.buttonViewUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonViewUp.Location = new System.Drawing.Point(431, 73);
      this.buttonViewUp.Name = "buttonViewUp";
      this.buttonViewUp.Size = new System.Drawing.Size(75, 23);
      this.buttonViewUp.TabIndex = 3;
      this.buttonViewUp.Text = "Up";
      this.buttonViewUp.UseVisualStyleBackColor = true;
      // 
      // buttonRemoveView
      // 
      this.buttonRemoveView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRemoveView.Location = new System.Drawing.Point(431, 35);
      this.buttonRemoveView.Name = "buttonRemoveView";
      this.buttonRemoveView.Size = new System.Drawing.Size(75, 23);
      this.buttonRemoveView.TabIndex = 2;
      this.buttonRemoveView.Text = "Remove";
      this.buttonRemoveView.UseVisualStyleBackColor = true;
      this.buttonRemoveView.Click += new System.EventHandler(this.buttonRemoveView_Click);
      // 
      // buttonAddView
      // 
      this.buttonAddView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAddView.Location = new System.Drawing.Point(431, 6);
      this.buttonAddView.Name = "buttonAddView";
      this.buttonAddView.Size = new System.Drawing.Size(75, 23);
      this.buttonAddView.TabIndex = 1;
      this.buttonAddView.Text = "Add";
      this.buttonAddView.UseVisualStyleBackColor = true;
      this.buttonAddView.Click += new System.EventHandler(this.buttonAddView_Click);
      // 
      // listViews
      // 
      this.listViews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listViews.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colViewName,
            this.colViewDesc});
      this.listViews.FullRowSelect = true;
      this.listViews.Location = new System.Drawing.Point(6, 33);
      this.listViews.Name = "listViews";
      this.listViews.Size = new System.Drawing.Size(419, 95);
      this.listViews.TabIndex = 5;
      this.listViews.UseCompatibleStateImageBehavior = false;
      this.listViews.View = System.Windows.Forms.View.Details;
      // 
      // colViewName
      // 
      this.colViewName.Text = "Name";
      this.colViewName.Width = 200;
      // 
      // colViewDesc
      // 
      this.colViewDesc.Text = "Description";
      this.colViewDesc.Width = 200;
      // 
      // tabPageRestrictions
      // 
      this.tabPageRestrictions.Controls.Add(this.textRestriction);
      this.tabPageRestrictions.Controls.Add(this.comboNewRestriction);
      this.tabPageRestrictions.Controls.Add(this.listRestrictions);
      this.tabPageRestrictions.Controls.Add(this.buttonRemoveRestriction);
      this.tabPageRestrictions.Controls.Add(this.buttonAddRestriction);
      this.tabPageRestrictions.Location = new System.Drawing.Point(4, 22);
      this.tabPageRestrictions.Name = "tabPageRestrictions";
      this.tabPageRestrictions.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageRestrictions.Size = new System.Drawing.Size(512, 134);
      this.tabPageRestrictions.TabIndex = 1;
      this.tabPageRestrictions.Text = "Restrictions";
      this.tabPageRestrictions.UseVisualStyleBackColor = true;
      // 
      // textRestriction
      // 
      this.textRestriction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textRestriction.Location = new System.Drawing.Point(6, 6);
      this.textRestriction.Name = "textRestriction";
      this.textRestriction.Size = new System.Drawing.Size(274, 20);
      this.textRestriction.TabIndex = 0;
      // 
      // comboNewRestriction
      // 
      this.comboNewRestriction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboNewRestriction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboNewRestriction.Items.AddRange(new object[] {
            "MAC",
            "IP",
            "User-Agent"});
      this.comboNewRestriction.Location = new System.Drawing.Point(305, 6);
      this.comboNewRestriction.Name = "comboNewRestriction";
      this.comboNewRestriction.Size = new System.Drawing.Size(120, 21);
      this.comboNewRestriction.TabIndex = 1;
      // 
      // listRestrictions
      // 
      this.listRestrictions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listRestrictions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRestriction,
            this.colRestrictionType});
      this.listRestrictions.FullRowSelect = true;
      this.listRestrictions.Location = new System.Drawing.Point(6, 33);
      this.listRestrictions.Name = "listRestrictions";
      this.listRestrictions.Size = new System.Drawing.Size(419, 95);
      this.listRestrictions.TabIndex = 4;
      this.listRestrictions.UseCompatibleStateImageBehavior = false;
      this.listRestrictions.View = System.Windows.Forms.View.Details;
      // 
      // colRestriction
      // 
      this.colRestriction.Text = "Restriction";
      this.colRestriction.Width = 200;
      // 
      // colRestrictionType
      // 
      this.colRestrictionType.Text = "Type";
      this.colRestrictionType.Width = 200;
      // 
      // buttonRemoveRestriction
      // 
      this.buttonRemoveRestriction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRemoveRestriction.Location = new System.Drawing.Point(431, 35);
      this.buttonRemoveRestriction.Name = "buttonRemoveRestriction";
      this.buttonRemoveRestriction.Size = new System.Drawing.Size(75, 23);
      this.buttonRemoveRestriction.TabIndex = 3;
      this.buttonRemoveRestriction.Text = "Remove";
      this.buttonRemoveRestriction.UseVisualStyleBackColor = true;
      this.buttonRemoveRestriction.Click += new System.EventHandler(this.buttonRemoveRestriction_Click);
      // 
      // buttonAddRestriction
      // 
      this.buttonAddRestriction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAddRestriction.Location = new System.Drawing.Point(431, 6);
      this.buttonAddRestriction.Name = "buttonAddRestriction";
      this.buttonAddRestriction.Size = new System.Drawing.Size(75, 23);
      this.buttonAddRestriction.TabIndex = 2;
      this.buttonAddRestriction.Text = "Add";
      this.buttonAddRestriction.UseVisualStyleBackColor = true;
      this.buttonAddRestriction.Click += new System.EventHandler(this.buttonAddRestriction_Click);
      // 
      // FormServer
      // 
      this.AcceptButton = this.buttonAccept;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
      this.CancelButton = this.buttonCancel;
      this.ClientSize = new System.Drawing.Size(544, 508);
      this.Controls.Add(this.tabbedOptions);
      this.Controls.Add(this.buttonAccept);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.groupDirectories);
      this.Controls.Add(this.groupTypes);
      this.Controls.Add(this.groupOrder);
      this.Controls.Add(this.groupName);
      this.MaximizeBox = false;
      this.Name = "FormServer";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Edit Server";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormServer_FormClosing);
      this.groupName.ResumeLayout(false);
      this.groupName.PerformLayout();
      this.groupOrder.ResumeLayout(false);
      this.groupOrder.PerformLayout();
      this.groupTypes.ResumeLayout(false);
      this.groupTypes.PerformLayout();
      this.groupDirectories.ResumeLayout(false);
      this.groupDirectories.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabbedOptions.ResumeLayout(false);
      this.tabPageViews.ResumeLayout(false);
      this.tabPageRestrictions.ResumeLayout(false);
      this.tabPageRestrictions.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupName;
    private System.Windows.Forms.TextBox textName;
    private System.Windows.Forms.GroupBox groupOrder;
    private System.Windows.Forms.CheckBox checkOrderDescending;
    private System.Windows.Forms.ComboBox comboOrder;
    private System.Windows.Forms.GroupBox groupTypes;
    private System.Windows.Forms.CheckBox checkImages;
    private System.Windows.Forms.CheckBox checkAudio;
    private System.Windows.Forms.CheckBox checkVideo;
    private System.Windows.Forms.GroupBox groupDirectories;
    private System.Windows.Forms.Button buttonRemoveDirectory;
    private System.Windows.Forms.Button buttonAddDirectory;
    private System.Windows.Forms.ListView listDirectories;
    private System.Windows.Forms.ColumnHeader colDirectory;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonAccept;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Label listDirectoriesAnchor;
    private System.Windows.Forms.FolderBrowserDialog folderDialog;
    private System.Windows.Forms.TabControl tabbedOptions;
    private System.Windows.Forms.TabPage tabPageViews;
    private System.Windows.Forms.ComboBox comboNewView;
    private System.Windows.Forms.Button buttonViewDown;
    private System.Windows.Forms.Button buttonViewUp;
    private System.Windows.Forms.Button buttonRemoveView;
    private System.Windows.Forms.Button buttonAddView;
    private System.Windows.Forms.ListView listViews;
    private System.Windows.Forms.ColumnHeader colViewName;
    private System.Windows.Forms.ColumnHeader colViewDesc;
    private System.Windows.Forms.TabPage tabPageRestrictions;
    private System.Windows.Forms.TextBox textRestriction;
    private System.Windows.Forms.ComboBox comboNewRestriction;
    private System.Windows.Forms.ListView listRestrictions;
    private System.Windows.Forms.ColumnHeader colRestriction;
    private System.Windows.Forms.ColumnHeader colRestrictionType;
    private System.Windows.Forms.Button buttonRemoveRestriction;
    private System.Windows.Forms.Button buttonAddRestriction;
  }
}
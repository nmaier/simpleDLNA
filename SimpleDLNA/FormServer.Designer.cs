namespace NMaier.SimpleDlna.GUI
{
  partial class FormServer
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormServer));
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.TextName = new System.Windows.Forms.TextBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.CheckOrderDescending = new System.Windows.Forms.CheckBox();
      this.ComboOrder = new System.Windows.Forms.ComboBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.CheckImages = new System.Windows.Forms.CheckBox();
      this.CheckAudio = new System.Windows.Forms.CheckBox();
      this.CheckVideo = new System.Windows.Forms.CheckBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.ComboNewView = new System.Windows.Forms.ComboBox();
      this.ButtonViewDown = new System.Windows.Forms.Button();
      this.ButtonViewUp = new System.Windows.Forms.Button();
      this.ButtonRemoveView = new System.Windows.Forms.Button();
      this.ButtonAddView = new System.Windows.Forms.Button();
      this.ListViews = new System.Windows.Forms.ListView();
      this.ColViewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ColViewDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.ListDirectoriesAnchor = new System.Windows.Forms.Label();
      this.ButtonRemoveDirectory = new System.Windows.Forms.Button();
      this.ButtonAddDirectory = new System.Windows.Forms.Button();
      this.ListDirectories = new System.Windows.Forms.ListView();
      this.ColDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ButtonCancel = new System.Windows.Forms.Button();
      this.ButtonAccept = new System.Windows.Forms.Button();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.groupBox5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.TextName);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(520, 46);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Name";
      // 
      // TextName
      // 
      this.TextName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TextName.Location = new System.Drawing.Point(6, 19);
      this.TextName.Name = "TextName";
      this.TextName.Size = new System.Drawing.Size(485, 20);
      this.TextName.TabIndex = 0;
      this.TextName.Validating += new System.ComponentModel.CancelEventHandler(this.TextName_Validating);
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add(this.CheckOrderDescending);
      this.groupBox2.Controls.Add(this.ComboOrder);
      this.groupBox2.Location = new System.Drawing.Point(12, 64);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(520, 46);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Order";
      // 
      // CheckOrderDescending
      // 
      this.CheckOrderDescending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.CheckOrderDescending.AutoSize = true;
      this.CheckOrderDescending.Location = new System.Drawing.Point(431, 19);
      this.CheckOrderDescending.Name = "CheckOrderDescending";
      this.CheckOrderDescending.Size = new System.Drawing.Size(83, 17);
      this.CheckOrderDescending.TabIndex = 1;
      this.CheckOrderDescending.Text = "Descending";
      this.CheckOrderDescending.UseVisualStyleBackColor = true;
      // 
      // ComboOrder
      // 
      this.ComboOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ComboOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ComboOrder.FormattingEnabled = true;
      this.ComboOrder.Location = new System.Drawing.Point(6, 19);
      this.ComboOrder.Name = "ComboOrder";
      this.ComboOrder.Size = new System.Drawing.Size(419, 21);
      this.ComboOrder.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox3.Controls.Add(this.CheckImages);
      this.groupBox3.Controls.Add(this.CheckAudio);
      this.groupBox3.Controls.Add(this.CheckVideo);
      this.groupBox3.Location = new System.Drawing.Point(12, 116);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(520, 46);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Types";
      this.groupBox3.Validating += new System.ComponentModel.CancelEventHandler(this.CheckTypes_Validating);
      // 
      // CheckImages
      // 
      this.CheckImages.AutoSize = true;
      this.CheckImages.Location = new System.Drawing.Point(133, 19);
      this.CheckImages.Name = "CheckImages";
      this.CheckImages.Size = new System.Drawing.Size(60, 17);
      this.CheckImages.TabIndex = 2;
      this.CheckImages.Text = "Images";
      this.CheckImages.UseVisualStyleBackColor = true;
      // 
      // CheckAudio
      // 
      this.CheckAudio.AutoSize = true;
      this.CheckAudio.Location = new System.Drawing.Point(74, 19);
      this.CheckAudio.Name = "CheckAudio";
      this.CheckAudio.Size = new System.Drawing.Size(53, 17);
      this.CheckAudio.TabIndex = 1;
      this.CheckAudio.Text = "Audio";
      this.CheckAudio.UseVisualStyleBackColor = true;
      // 
      // CheckVideo
      // 
      this.CheckVideo.AutoSize = true;
      this.CheckVideo.Location = new System.Drawing.Point(15, 19);
      this.CheckVideo.Name = "CheckVideo";
      this.CheckVideo.Size = new System.Drawing.Size(53, 17);
      this.CheckVideo.TabIndex = 0;
      this.CheckVideo.Text = "Video";
      this.CheckVideo.UseVisualStyleBackColor = true;
      // 
      // groupBox4
      // 
      this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox4.Controls.Add(this.ComboNewView);
      this.groupBox4.Controls.Add(this.ButtonViewDown);
      this.groupBox4.Controls.Add(this.ButtonViewUp);
      this.groupBox4.Controls.Add(this.ButtonRemoveView);
      this.groupBox4.Controls.Add(this.ButtonAddView);
      this.groupBox4.Controls.Add(this.ListViews);
      this.groupBox4.Location = new System.Drawing.Point(13, 169);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(519, 159);
      this.groupBox4.TabIndex = 3;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Views";
      // 
      // ComboNewView
      // 
      this.ComboNewView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ComboNewView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ComboNewView.FormattingEnabled = true;
      this.ComboNewView.Location = new System.Drawing.Point(6, 21);
      this.ComboNewView.Name = "ComboNewView";
      this.ComboNewView.Size = new System.Drawing.Size(429, 21);
      this.ComboNewView.TabIndex = 6;
      // 
      // ButtonViewDown
      // 
      this.ButtonViewDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonViewDown.Location = new System.Drawing.Point(438, 115);
      this.ButtonViewDown.Name = "ButtonViewDown";
      this.ButtonViewDown.Size = new System.Drawing.Size(75, 23);
      this.ButtonViewDown.TabIndex = 5;
      this.ButtonViewDown.Text = "Down";
      this.ButtonViewDown.UseVisualStyleBackColor = true;
      // 
      // ButtonViewUp
      // 
      this.ButtonViewUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonViewUp.Location = new System.Drawing.Point(438, 86);
      this.ButtonViewUp.Name = "ButtonViewUp";
      this.ButtonViewUp.Size = new System.Drawing.Size(75, 23);
      this.ButtonViewUp.TabIndex = 4;
      this.ButtonViewUp.Text = "Up";
      this.ButtonViewUp.UseVisualStyleBackColor = true;
      // 
      // ButtonRemoveView
      // 
      this.ButtonRemoveView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonRemoveView.Location = new System.Drawing.Point(438, 48);
      this.ButtonRemoveView.Name = "ButtonRemoveView";
      this.ButtonRemoveView.Size = new System.Drawing.Size(75, 23);
      this.ButtonRemoveView.TabIndex = 2;
      this.ButtonRemoveView.Text = "Remove";
      this.ButtonRemoveView.UseVisualStyleBackColor = true;
      this.ButtonRemoveView.Click += new System.EventHandler(this.ButtonRemoveView_Click);
      // 
      // ButtonAddView
      // 
      this.ButtonAddView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonAddView.Location = new System.Drawing.Point(438, 19);
      this.ButtonAddView.Name = "ButtonAddView";
      this.ButtonAddView.Size = new System.Drawing.Size(75, 23);
      this.ButtonAddView.TabIndex = 1;
      this.ButtonAddView.Text = "Add";
      this.ButtonAddView.UseVisualStyleBackColor = true;
      this.ButtonAddView.Click += new System.EventHandler(this.ButtonAddView_Click);
      // 
      // ListViews
      // 
      this.ListViews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ListViews.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColViewName,
            this.ColViewDesc});
      this.ListViews.FullRowSelect = true;
      this.ListViews.Location = new System.Drawing.Point(6, 48);
      this.ListViews.Name = "ListViews";
      this.ListViews.Size = new System.Drawing.Size(429, 105);
      this.ListViews.TabIndex = 0;
      this.ListViews.UseCompatibleStateImageBehavior = false;
      this.ListViews.View = System.Windows.Forms.View.Details;
      // 
      // ColViewName
      // 
      this.ColViewName.Text = "Name";
      this.ColViewName.Width = 200;
      // 
      // ColViewDesc
      // 
      this.ColViewDesc.Text = "Description";
      this.ColViewDesc.Width = 200;
      // 
      // groupBox5
      // 
      this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox5.Controls.Add(this.ListDirectoriesAnchor);
      this.groupBox5.Controls.Add(this.ButtonRemoveDirectory);
      this.groupBox5.Controls.Add(this.ButtonAddDirectory);
      this.groupBox5.Controls.Add(this.ListDirectories);
      this.groupBox5.Location = new System.Drawing.Point(13, 334);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(519, 138);
      this.groupBox5.TabIndex = 4;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Directories";
      // 
      // ListDirectoriesAnchor
      // 
      this.ListDirectoriesAnchor.AutoSize = true;
      this.ListDirectoriesAnchor.Location = new System.Drawing.Point(441, 74);
      this.ListDirectoriesAnchor.Name = "ListDirectoriesAnchor";
      this.ListDirectoriesAnchor.Size = new System.Drawing.Size(0, 13);
      this.ListDirectoriesAnchor.TabIndex = 3;
      // 
      // ButtonRemoveDirectory
      // 
      this.ButtonRemoveDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonRemoveDirectory.Location = new System.Drawing.Point(438, 48);
      this.ButtonRemoveDirectory.Name = "ButtonRemoveDirectory";
      this.ButtonRemoveDirectory.Size = new System.Drawing.Size(75, 23);
      this.ButtonRemoveDirectory.TabIndex = 2;
      this.ButtonRemoveDirectory.Text = "Remove";
      this.ButtonRemoveDirectory.UseVisualStyleBackColor = true;
      this.ButtonRemoveDirectory.Click += new System.EventHandler(this.ButtonRemoveDirectory_Click);
      // 
      // ButtonAddDirectory
      // 
      this.ButtonAddDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonAddDirectory.Location = new System.Drawing.Point(438, 19);
      this.ButtonAddDirectory.Name = "ButtonAddDirectory";
      this.ButtonAddDirectory.Size = new System.Drawing.Size(75, 23);
      this.ButtonAddDirectory.TabIndex = 1;
      this.ButtonAddDirectory.Text = "Add";
      this.ButtonAddDirectory.UseVisualStyleBackColor = true;
      this.ButtonAddDirectory.Click += new System.EventHandler(this.ButtonAddDirectory_Click);
      // 
      // ListDirectories
      // 
      this.ListDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ListDirectories.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColDirectory});
      this.ListDirectories.Location = new System.Drawing.Point(6, 19);
      this.ListDirectories.Name = "ListDirectories";
      this.ListDirectories.Size = new System.Drawing.Size(429, 113);
      this.ListDirectories.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.ListDirectories.TabIndex = 0;
      this.ListDirectories.UseCompatibleStateImageBehavior = false;
      this.ListDirectories.View = System.Windows.Forms.View.Details;
      this.ListDirectories.Validating += new System.ComponentModel.CancelEventHandler(this.ListDirectories_Validating);
      // 
      // ColDirectory
      // 
      this.ColDirectory.Text = "Directory";
      this.ColDirectory.Width = 200;
      // 
      // ButtonCancel
      // 
      this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.ButtonCancel.Location = new System.Drawing.Point(457, 478);
      this.ButtonCancel.Name = "ButtonCancel";
      this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
      this.ButtonCancel.TabIndex = 5;
      this.ButtonCancel.Text = "&Cancel";
      this.ButtonCancel.UseVisualStyleBackColor = true;
      // 
      // ButtonAccept
      // 
      this.ButtonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.ButtonAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.ButtonAccept.Location = new System.Drawing.Point(376, 478);
      this.ButtonAccept.Name = "ButtonAccept";
      this.ButtonAccept.Size = new System.Drawing.Size(75, 23);
      this.ButtonAccept.TabIndex = 6;
      this.ButtonAccept.Text = "&OK";
      this.ButtonAccept.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // FormServer
      // 
      this.AcceptButton = this.ButtonAccept;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
      this.CancelButton = this.ButtonCancel;
      this.ClientSize = new System.Drawing.Size(544, 508);
      this.Controls.Add(this.ButtonAccept);
      this.Controls.Add(this.ButtonCancel);
      this.Controls.Add(this.groupBox5);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "FormServer";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Edit Server";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormServer_FormClosing);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox TextName;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckBox CheckOrderDescending;
    private System.Windows.Forms.ComboBox ComboOrder;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.CheckBox CheckImages;
    private System.Windows.Forms.CheckBox CheckAudio;
    private System.Windows.Forms.CheckBox CheckVideo;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.Button ButtonRemoveView;
    private System.Windows.Forms.Button ButtonAddView;
    private System.Windows.Forms.ListView ListViews;
    private System.Windows.Forms.ColumnHeader ColViewName;
    private System.Windows.Forms.ColumnHeader ColViewDesc;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.Button ButtonRemoveDirectory;
    private System.Windows.Forms.Button ButtonAddDirectory;
    private System.Windows.Forms.ListView ListDirectories;
    private System.Windows.Forms.ColumnHeader ColDirectory;
    private System.Windows.Forms.Button ButtonCancel;
    private System.Windows.Forms.Button ButtonAccept;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Label ListDirectoriesAnchor;
    private System.Windows.Forms.FolderBrowserDialog FolderDialog;
    private System.Windows.Forms.Button ButtonViewDown;
    private System.Windows.Forms.Button ButtonViewUp;
    private System.Windows.Forms.ComboBox ComboNewView;
  }
}
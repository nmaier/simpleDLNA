namespace NMaier.SimpleDlna.GUI
{
  sealed partial class FormAbout
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.logoPictureBox = new System.Windows.Forms.PictureBox();
      this.Product = new System.Windows.Forms.Label();
      this.Version = new System.Windows.Forms.Label();
      this.Copyright = new System.Windows.Forms.Label();
      this.License = new System.Windows.Forms.TextBox();
      this.okButton = new System.Windows.Forms.Button();
      this.tableLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.ColumnCount = 2;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
      this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
      this.tableLayoutPanel.Controls.Add(this.Product, 1, 0);
      this.tableLayoutPanel.Controls.Add(this.Version, 1, 1);
      this.tableLayoutPanel.Controls.Add(this.Copyright, 1, 2);
      this.tableLayoutPanel.Controls.Add(this.License, 1, 4);
      this.tableLayoutPanel.Controls.Add(this.okButton, 1, 5);
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(10, 10);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 6;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(487, 307);
      this.tableLayoutPanel.TabIndex = 0;
      // 
      // logoPictureBox
      // 
      this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logoPictureBox.Image = global::NMaier.SimpleDlna.GUI.Properties.Resources.banner;
      this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
      this.logoPictureBox.Name = "logoPictureBox";
      this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
      this.logoPictureBox.Size = new System.Drawing.Size(154, 301);
      this.logoPictureBox.TabIndex = 12;
      this.logoPictureBox.TabStop = false;
      // 
      // Product
      // 
      this.Product.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Product.Location = new System.Drawing.Point(167, 0);
      this.Product.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
      this.Product.MaximumSize = new System.Drawing.Size(0, 20);
      this.Product.Name = "Product";
      this.Product.Size = new System.Drawing.Size(317, 20);
      this.Product.TabIndex = 19;
      this.Product.Text = "Product Name";
      this.Product.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // Version
      // 
      this.Version.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Version.Location = new System.Drawing.Point(167, 30);
      this.Version.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
      this.Version.MaximumSize = new System.Drawing.Size(0, 20);
      this.Version.Name = "Version";
      this.Version.Size = new System.Drawing.Size(317, 20);
      this.Version.TabIndex = 0;
      this.Version.Text = "Version";
      this.Version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // Copyright
      // 
      this.Copyright.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Copyright.Location = new System.Drawing.Point(167, 60);
      this.Copyright.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
      this.Copyright.MaximumSize = new System.Drawing.Size(0, 20);
      this.Copyright.Name = "Copyright";
      this.Copyright.Size = new System.Drawing.Size(317, 20);
      this.Copyright.TabIndex = 21;
      this.Copyright.Text = "Copyright";
      this.Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // License
      // 
      this.License.AcceptsReturn = true;
      this.License.AcceptsTab = true;
      this.License.Dock = System.Windows.Forms.DockStyle.Fill;
      this.License.Location = new System.Drawing.Point(167, 123);
      this.License.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
      this.License.Multiline = true;
      this.License.Name = "License";
      this.License.ReadOnly = true;
      this.License.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.License.Size = new System.Drawing.Size(317, 147);
      this.License.TabIndex = 23;
      this.License.TabStop = false;
      this.License.Text = "Description";
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.okButton.Location = new System.Drawing.Point(397, 277);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(87, 27);
      this.okButton.TabIndex = 24;
      this.okButton.Text = "&OK";
      // 
      // FormAbout
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(507, 327);
      this.Controls.Add(this.tableLayoutPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormAbout";
      this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "FormAbout";
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.PictureBox logoPictureBox;
    private System.Windows.Forms.Label Product;
    private System.Windows.Forms.Label Version;
    private System.Windows.Forms.Label Copyright;
    private System.Windows.Forms.TextBox License;
    private System.Windows.Forms.Button okButton;
  }
}

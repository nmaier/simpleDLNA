using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Views;

namespace NMaier.SimpleDlna.GUI
{
  internal partial class FormServer : Form
  {
    public FormServer()
    {
      Init();
      Text = "New Server";
      CheckVideo.Checked = true;
      SizeDirectoryColumn();
      SizeViewsColumns();
    }

    public FormServer(ServerDescription description)
    {
      Init();

      TextName.Text = description.Name;

      CheckVideo.Checked = (description.Types & DlnaMediaTypes.Video) == DlnaMediaTypes.Video;
      CheckAudio.Checked = (description.Types & DlnaMediaTypes.Audio) == DlnaMediaTypes.Audio;
      CheckImages.Checked = (description.Types & DlnaMediaTypes.Image) == DlnaMediaTypes.Image;

      foreach (var i in ComboOrder.Items) {
        if (((IItemComparer)i).Name == description.Order) {
          ComboOrder.SelectedItem = i;
          break;
        }
      }
      CheckOrderDescending.Checked = description.OrderDescending;

      foreach (var v in description.Views) {
        var i = new ListViewItem(new string[] { v, ViewRepository.Lookup(v).Description });
        ListViews.Items.Add(i);
      }
      foreach (var d in description.Directories) {
        var i = new ListViewItem(d);
        ListDirectories.Items.Add(i);
      }

      SizeDirectoryColumn();
      SizeViewsColumns();
    }


    public ServerDescription Description
    {
      get
      {
        var rv = new ServerDescription()
        {
          Name = TextName.Text,
          Order = ((IItemComparer)ComboOrder.SelectedItem).Name,
          OrderDescending = CheckOrderDescending.Checked,
          Views = (from ListViewItem i in ListViews.Items
                   select i.Text).ToArray(),
          Directories = (from ListViewItem i in ListDirectories.Items
                         select i.Text).ToArray()
        };
        if (CheckVideo.Checked) {
          rv.Types |= DlnaMediaTypes.Video;
        }
        if (CheckAudio.Checked) {
          rv.Types |= DlnaMediaTypes.Audio;
        }
        if (CheckImages.Checked) {
          rv.Types |= DlnaMediaTypes.Image;
        }
        return rv;
      }
    }


    private void ButtonAddDirectory_Click(object sender, EventArgs e)
    {
      if (FolderDialog.ShowDialog() == DialogResult.OK) {
        var path = FolderDialog.SelectedPath;
        var found = from ListViewItem i in ListDirectories.Items
                    where StringComparer.InvariantCulture.Equals(path, i.Text)
                    select i;
        if (found.Count() == 0) {
          ListDirectories.Items.Add(path);
        }
      }
      SizeDirectoryColumn();
    }

    private void ButtonAddView_Click(object sender, EventArgs e)
    {
      var i = ComboNewView.SelectedItem as IView;
      if (i == null) {
        return;
      }
      ListViews.Items.Add(new ListViewItem(new string[] { i.Name, i.Description }));
      SizeViewsColumns();
    }

    private void ButtonRemoveDirectory_Click(object sender, EventArgs e)
    {
      foreach (var i in ListDirectories.SelectedItems) {
        ListDirectories.Items.Remove(i as ListViewItem);
      }
      SizeDirectoryColumn();
    }

    private void SizeDirectoryColumn()
    {
      var mode = ColumnHeaderAutoResizeStyle.ColumnContent;
      if (ListDirectories.Items.Count == 0) {
        mode = ColumnHeaderAutoResizeStyle.HeaderSize;
      }
      ColDirectory.AutoResize(mode);
    }

    private void ButtonRemoveView_Click(object sender, EventArgs e)
    {
      foreach (var i in ListViews.SelectedItems) {
        ListViews.Items.Remove(i as ListViewItem);
      }
      SizeViewsColumns();
    }

    private void SizeViewsColumns()
    {
      var mode = ColumnHeaderAutoResizeStyle.ColumnContent;
      if (ListViews.Items.Count == 0) {
        mode = ColumnHeaderAutoResizeStyle.HeaderSize;
      }
      ColViewName.AutoResize(mode);
      ColViewDesc.AutoResize(mode);
    }

    private void CheckTypes_Validating(object sender, CancelEventArgs e)
    {
      if (!CheckVideo.Checked && !CheckAudio.Checked && !CheckImages.Checked) {
        errorProvider.SetError(CheckImages, "Must select at least one");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(CheckImages, string.Empty);
      }
    }

    private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = DialogResult == DialogResult.OK && !ValidateChildren();
    }

    private void Init()
    {
      InitializeComponent();
      AddOrderItems();
      AddViewItems();
    }

    private void AddOrderItems()
    {
      var items = from v in ComparerRepository.ListItems()
                  orderby v.Key
                  select v.Value;
      foreach (var v in items) {
        var i = ComboOrder.Items.Add(v);
        if (v.Name == "title") {
          ComboOrder.SelectedIndex = i;
        }
      }
    }
    private void AddViewItems()
    {
      var items = from v in ViewRepository.ListItems()
                  orderby v.Key
                  select v.Value;
      foreach (var v in items) {
        var i = ComboNewView.Items.Add(v);
      }
    }

    private void ListDirectories_Validating(object sender, CancelEventArgs e)
    {
      if (ListDirectories.Items.Count == 0) {
        errorProvider.SetError(ListDirectoriesAnchor, "Must specify at least one directory");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(ListDirectoriesAnchor, string.Empty);
      }
    }

    private void TextName_Validating(object sender, CancelEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(TextName.Text)) {
        errorProvider.SetError(TextName, "Must specify a name");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(TextName, string.Empty);
      }
    }
  }
}

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
      checkVideo.Checked = true;
      SizeDirectoryColumn();
      SizeViewsColumns();
    }
    public FormServer(ServerDescription description)
    {
      Init();

      textName.Text = description.Name;

      checkVideo.Checked = (description.Types & DlnaMediaTypes.Video) == DlnaMediaTypes.Video;
      checkAudio.Checked = (description.Types & DlnaMediaTypes.Audio) == DlnaMediaTypes.Audio;
      checkImages.Checked = (description.Types & DlnaMediaTypes.Image) == DlnaMediaTypes.Image;

      foreach (var i in comboOrder.Items) {
        if (((IItemComparer)i).Name == description.Order) {
          comboOrder.SelectedItem = i;
          break;
        }
      }
      checkOrderDescending.Checked = description.OrderDescending;

      foreach (var v in description.Views) {
        var i = new ListViewItem(new string[] { v, ViewRepository.Lookup(v).Description });
        listViews.Items.Add(i);
      }
      foreach (var d in description.Directories) {
        var i = new ListViewItem(d);
        listDirectories.Items.Add(i);
      }

      SizeDirectoryColumn();
      SizeViewsColumns();
    }


    public ServerDescription Description
    {
      get
      {
        DlnaMediaTypes types = 0;
        if (checkVideo.Checked) {
          types |= DlnaMediaTypes.Video;
        }
        if (checkAudio.Checked) {
          types |= DlnaMediaTypes.Audio;
        }
        if (checkImages.Checked) {
          types |= DlnaMediaTypes.Image;
        }
        var views = (from ListViewItem i in listViews.Items
                     select i.Text).ToList();
        var dirs = (from ListViewItem i in listDirectories.Items
                    select i.Text).ToList();
        var rv = new ServerDescription(
          textName.Text,
          ((IItemComparer)comboOrder.SelectedItem).Name,
          checkOrderDescending.Checked,
          types,
          views,
          dirs);
        return rv;
      }
    }


    private void AddOrderItems()
    {
      var items = from v in ComparerRepository.ListItems()
                  orderby v.Key
                  select v.Value;
      foreach (var v in items) {
        var i = comboOrder.Items.Add(v);
        if (v.Name == "title") {
          comboOrder.SelectedIndex = i;
        }
      }
    }

    private void AddViewItems()
    {
      var items = from v in ViewRepository.ListItems()
                  orderby v.Key
                  select v.Value;
      foreach (var v in items) {
        comboNewView.Items.Add(v);
      }
    }

    private void buttonAddDirectory_Click(object sender, EventArgs e)
    {
      if (folderDialog.ShowDialog() == DialogResult.OK) {
        var path = folderDialog.SelectedPath;
        var found = from ListViewItem i in listDirectories.Items
                    where StringComparer.InvariantCulture.Equals(path, i.Text)
                    select i;
        if (found.Count() == 0) {
          listDirectories.Items.Add(path);
        }
      }
      SizeDirectoryColumn();
    }

    private void buttonAddView_Click(object sender, EventArgs e)
    {
      var i = comboNewView.SelectedItem as IView;
      if (i == null) {
        return;
      }
      listViews.Items.Add(new ListViewItem(new string[] { i.Name, i.Description }));
      SizeViewsColumns();
    }

    private void buttonRemoveDirectory_Click(object sender, EventArgs e)
    {
      foreach (var i in listDirectories.SelectedItems) {
        listDirectories.Items.Remove(i as ListViewItem);
      }
      SizeDirectoryColumn();
    }

    private void buttonRemoveView_Click(object sender, EventArgs e)
    {
      foreach (var i in listViews.SelectedItems) {
        listViews.Items.Remove(i as ListViewItem);
      }
      SizeViewsColumns();
    }

    private void checkTypes_Validating(object sender, CancelEventArgs e)
    {
      if (!checkVideo.Checked && !checkAudio.Checked && !checkImages.Checked) {
        errorProvider.SetError(checkImages, "Must select at least one");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(checkImages, string.Empty);
      }
    }

    private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = DialogResult == DialogResult.OK && !ValidateChildren();
    }

    private void Init()
    {
      InitializeComponent();
      Icon = Properties.Resources.server;
      AddOrderItems();
      AddViewItems();
    }

    private void listDirectories_Validating(object sender, CancelEventArgs e)
    {
      if (listDirectories.Items.Count == 0) {
        errorProvider.SetError(listDirectoriesAnchor, "Must specify at least one directory");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(listDirectoriesAnchor, string.Empty);
      }
    }

    private void SizeDirectoryColumn()
    {
      var mode = ColumnHeaderAutoResizeStyle.ColumnContent;
      if (listDirectories.Items.Count == 0) {
        mode = ColumnHeaderAutoResizeStyle.HeaderSize;
      }
      colDirectory.AutoResize(mode);
    }

    private void SizeViewsColumns()
    {
      var mode = ColumnHeaderAutoResizeStyle.ColumnContent;
      if (listViews.Items.Count == 0) {
        mode = ColumnHeaderAutoResizeStyle.HeaderSize;
      }
      colViewName.AutoResize(mode);
      colViewDesc.AutoResize(mode);
    }

    private void textName_Validating(object sender, CancelEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(textName.Text)) {
        errorProvider.SetError(textName, "Must specify a name");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(textName, string.Empty);
      }
    }
  }
}

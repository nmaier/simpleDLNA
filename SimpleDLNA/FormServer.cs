using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using NMaier.SimpleDlna.GUI.Properties;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Views;
using NMaier.SimpleDlna.Utilities;
using Form = NMaier.Windows.Forms.Form;

namespace NMaier.SimpleDlna.GUI
{
  internal sealed partial class FormServer : Form
  {
    public FormServer()
    {
      Init();
      Text = "New Server";
      checkVideo.Checked = true;
      SizeDirectoryColumn();
      SizeColumns(listViews);
      SizeColumns(listRestrictions);
    }

    public FormServer(ServerDescription description)
    {
      Init();

      textName.Text = description.Name;

      checkVideo.Checked =
        (description.Types & DlnaMediaTypes.Video) == DlnaMediaTypes.Video;
      checkAudio.Checked =
        (description.Types & DlnaMediaTypes.Audio) == DlnaMediaTypes.Audio;
      checkImages.Checked =
        (description.Types & DlnaMediaTypes.Image) == DlnaMediaTypes.Image;

      foreach (var i in comboOrder.Items) {
        if (((IItemComparer)i).Name == description.Order) {
          comboOrder.SelectedItem = i;
          break;
        }
      }
      checkOrderDescending.Checked = description.OrderDescending;

      foreach (var v in description.Views) {
        var i = new ListViewItem(new[]
        {
          v, ViewRepository.Lookup(v).Description
        });
        listViews.Items.Add(i);
      }
      foreach (var d in description.Directories) {
        var i = new ListViewItem(d);
        listDirectories.Items.Add(i);
      }
      var t = comboNewRestriction.Items[0].ToString();
      foreach (var r in description.Macs) {
        var i = listRestrictions.Items.Add(r);
        i.Tag = 0;
        i.SubItems.Add(t);
      }
      t = comboNewRestriction.Items[1].ToString();
      foreach (var r in description.Ips) {
        var i = listRestrictions.Items.Add(r);
        i.Tag = 1;
        i.SubItems.Add(t);
      }
      t = comboNewRestriction.Items[2].ToString();
      foreach (var r in description.UserAgents) {
        var i = listRestrictions.Items.Add(r);
        i.Tag = 2;
        i.SubItems.Add(t);
      }

      SizeDirectoryColumn();
      SizeColumns(listViews);
    }

    public ServerDescription Description
    {
      get {
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
                     select i.Text).ToArray();
        var dirs = (from ListViewItem i in listDirectories.Items
                    select i.Text).ToArray();
        var macs = (from ListViewItem i in listRestrictions.Items
                    where (int)i.Tag == 0
                    select i.Text).ToArray();
        var ips = (from ListViewItem i in listRestrictions.Items
                   where (int)i.Tag == 1
                   select i.Text).ToArray();
        var uas = (from ListViewItem i in listRestrictions.Items
                   where (int)i.Tag == 2
                   select i.Text).ToArray();
        var rv = new ServerDescription
        {
          Name = textName.Text,
          Order = ((IItemComparer)comboOrder.SelectedItem).Name,
          OrderDescending = checkOrderDescending.Checked,
          Types = types,
          Views = views,
          Ips = ips,
          Macs = macs,
          UserAgents = uas,
          Directories = dirs.ToArray()
        };
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
        if (!found.Any()) {
          listDirectories.Items.Add(path);
        }
      }
      SizeDirectoryColumn();
    }

    private void buttonAddRestriction_Click(object sender, EventArgs e)
    {
      var valid = false;
      var re = textRestriction.Text;
      switch (comboNewRestriction.SelectedIndex) {
      case 0:
        valid = IP.IsAcceptedMAC(re);
        break;
      case 1:
        IPAddress o;
        valid = IPAddress.TryParse(re, out o);
        break;
      case 2:
        valid = !string.IsNullOrWhiteSpace(re);
        break;
      default:
        break;
      }
      if (!valid) {
        errorProvider.SetError(
          textRestriction, "You must provide a valid value");
        return;
      }
      var item = listRestrictions.Items.Add(re);
      item.SubItems.Add(comboNewRestriction.SelectedItem.ToString());
      item.Tag = comboNewRestriction.SelectedIndex;
      SizeColumns(listRestrictions);
    }

    private void buttonAddView_Click(object sender, EventArgs e)
    {
      var i = comboNewView.SelectedItem as IView;
      if (i == null) {
        return;
      }
      listViews.Items.Add(
        new ListViewItem(new[] {i.Name, i.Description}));
      SizeColumns(listViews);
    }

    private void buttonRemoveDirectory_Click(object sender, EventArgs e)
    {
      foreach (var i in listDirectories.SelectedItems) {
        listDirectories.Items.Remove((ListViewItem)i);
      }
      SizeDirectoryColumn();
    }

    private void buttonRemoveRestriction_Click(object sender, EventArgs e)
    {
      foreach (var i in listRestrictions.SelectedItems) {
        var item = (ListViewItem)i;
        var index = (int)item.Tag;
        textRestriction.Text = item.Text;
        comboNewRestriction.SelectedIndex = index;
        listRestrictions.Items.Remove(item);
      }
      SizeColumns(listRestrictions);
    }

    private void buttonRemoveView_Click(object sender, EventArgs e)
    {
      foreach (var i in listViews.SelectedItems) {
        listViews.Items.Remove((ListViewItem)i);
      }
      SizeColumns(listViews);
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
      Icon = Resources.server;
      AddOrderItems();
      AddViewItems();
      comboNewRestriction.SelectedIndex = 0;
    }

    private void listDirectories_Validating(object sender, CancelEventArgs e)
    {
      if (listDirectories.Items.Count == 0) {
        errorProvider.SetError(
          listDirectoriesAnchor, "Must specify at least one directory");
        e.Cancel = true;
      }
      else {
        errorProvider.SetError(listDirectoriesAnchor, string.Empty);
      }
    }

    private static void SizeColumns(ListView lv)
    {
      var mode = ColumnHeaderAutoResizeStyle.ColumnContent;
      if (lv.Items.Count == 0) {
        mode = ColumnHeaderAutoResizeStyle.HeaderSize;
      }
      for (var c = 0; c < lv.Columns.Count; ++c) {
        lv.Columns[c].AutoResize(mode);
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

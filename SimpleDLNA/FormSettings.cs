using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NMaier.SimpleDlna.GUI
{
    public partial class FormSettings : NMaier.Windows.Forms.Form
    {
        public FormSettings()
        {
            InitializeComponent();
            Icon = Properties.Resources.preferencesIcon;
        }

        private void buttonBrowseCacheFile_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textCacheFile.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            comboInterfaceList.Items.Add("Auto");
            if (global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.listeninterface == "Auto")
            {
                comboInterfaceList.SelectedItem = ("Auto");
            }

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        comboInterfaceList.Items.Add(addr.Address.ToString() + " (" + netInterface.Name + ")");
                    }

                    if (netInterface.Id == global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.listeninterface)
                    {
                        comboInterfaceList.SelectedItem = (addr.Address.ToString() + " (" + netInterface.Name + ")");
                    }
                }
            }

        }

        private void comboInterfaceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < comboInterfaceList.Items.Count; i++)
            {
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (comboInterfaceList.Text.Contains(netInterface.Name))
                    {
                        global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.listeninterface = (netInterface.Id);
                        return;
                    }
                }
            }
            global::NMaier.SimpleDlna.GUI.Properties.Settings.Default.listeninterface = ("Auto");

        }
    }
}

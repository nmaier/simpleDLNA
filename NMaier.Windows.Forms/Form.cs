using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using win = System.Windows.Forms;

namespace NMaier.Windows.Forms
{
  public class Form : win::Form
  {
    static Form()
    {
      if (LicenseManager.UsageMode != LicenseUsageMode.Runtime) {
        return;
      }
      win::Application.EnableVisualStyles();
      win::Application.VisualStyleState =
        VisualStyleState.ClientAndNonClientAreasEnabled;
      win::Application.SetCompatibleTextRenderingDefault(false);

      win::ToolStripManager.VisualStylesEnabled = true;
      win::ToolStripManager.Renderer = new ToolStripRealSystemRenderer();
    }

    public Form()
    {
      Font = SystemFonts.MessageBoxFont;
      BoldFont = new Font(Font, FontStyle.Bold);
      ItalicFont = new Font(Font, FontStyle.Italic);
      SetFlatStyle(this);
      SetStyle(
        win::ControlStyles.OptimizedDoubleBuffer |
        win::ControlStyles.AllPaintingInWmPaint,
        true);
    }

    public virtual Font BoldFont { get; set; }

    public virtual Font ItalicFont { get; set; }

    public static void SetFlatStyle(object control)
    {
      if (!(control is win::Button)) {
        var t = control.GetType();
        var p = t.GetProperty("FlatStyle", typeof(win::FlatStyle));
        if (p != null && p.CanWrite) {
          p.SetValue(control, win::FlatStyle.System, null);
        }
      }
      var ctrl = control as win::Control;
      if (ctrl != null) {
        foreach (var sc in ctrl.Controls) {
          SetFlatStyle(sc);
        }
      }
    }
  }
}

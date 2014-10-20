/***
The MIT License (MIT)
Copyright © 2008 Lee Houghton
Copyright © 2014 Nils Maier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
***/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace NMaier.Windows.Forms
{
  public class ToolStripRealSystemRenderer : ToolStripSystemRenderer
  {
    private const int RebarBackground = 6;

    private static readonly Dictionary<VSEInternal, bool> defined =
      new Dictionary<VSEInternal, bool>();

    public static readonly bool IsSupported = new Func<bool>(() =>
    {
      if (!VisualStyleRenderer.IsSupported) {
        return false;
      }
      return IsElementDefined(
        "Menu",
        (int)MenuParts.BarBackground,
        (int)MenuBarStates.Active);
    })();

    private VisualStyleRenderer renderer;

    public ToolStripRealSystemRenderer()
      : this(ToolbarTheme.Toolbar)
    {
    }

    public ToolStripRealSystemRenderer(ToolbarTheme theme)
    {
      Theme = theme;
    }

    private enum MenuBarItemStates : int
    {
      Normal = 1,
      Hover = 2,
      Pushed = 3,
      Disabled = 4,
      DisabledHover = 5,
      DisabledPushed = 6
    }

    private enum MenuBarStates : int
    {
      Active = 1,
      Inactive = 2
    }

    private enum MenuParts : int
    {
      ItemTMSchema = 1,
      DropDownTMSchema = 2,
      BarItemTMSchema = 3,
      BarDropDownTMSchema = 4,
      ChevronTMSchema = 5,
      SeparatorTMSchema = 6,
      BarBackground = 7,
      BarItem = 8,
      PopupBackground = 9,
      PopupBorders = 10,
      PopupCheck = 11,
      PopupCheckBackground = 12,
      PopupGutter = 13,
      PopupItem = 14,
      PopupSeparator = 15,
      PopupSubmenu = 16,
      SystemClose = 17,
      SystemMaximize = 18,
      SystemMinimize = 19,
      SystemRestore = 20
    }

    private enum MenuPopupCheckBackgroundStates : int
    {
      Disabled = 1,
      Normal = 2,
      Bitmap = 3
    }

    private enum MenuPopupCheckStates : int
    {
      CheckmarkNormal = 1,
      CheckmarkDisabled = 2,
      BulletNormal = 3,
      BulletDisabled = 4
    }

    private enum MenuPopupItemStates : int
    {
      Normal = 1,
      Hover = 2,
      Disabled = 3,
      DisabledHover = 4
    }

    private enum MenuPopupSubMenuStates : int
    {
      Normal = 1,
      Disabled = 2
    }

    public enum ToolbarTheme
    {
      BrowserTabBar,
      CommunicationsToolbar,
      HelpBar,
      MediaToolbar,
      Toolbar
    }

    private string MenuClass
    {
      get
      {
        return SubclassPrefix + "Menu";
      }
    }

    private string RebarClass
    {
      get
      {
        return SubclassPrefix + "Rebar";
      }
    }

    private string SubclassPrefix
    {
      get
      {
        switch (Theme) {
          case ToolbarTheme.MediaToolbar:
            return "Media::";
          case ToolbarTheme.CommunicationsToolbar:
            return "Communications::";
          case ToolbarTheme.BrowserTabBar:
            return "BrowserTabBar::";
          case ToolbarTheme.HelpBar:
            return "Help::";
          default:
            return string.Empty;
        }
      }
    }

    public ToolbarTheme Theme
    {
      get;
      set;
    }

    private bool EnsureRenderer()
    {
      if (!IsSupported) {
        return false;
      }
      if (renderer == null) {
        renderer =
          new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
      }
      return true;
    }

    private static Rectangle GetBackgroundRectangle(ToolStripItem item)
    {
      if (!item.IsOnDropDown) {
        return new Rectangle(new Point(), item.Bounds.Size);
      }

      var rect = item.Bounds;
      rect.X = item.ContentRectangle.X + 1;
      rect.Width = item.ContentRectangle.Width - 1;
      rect.Y = 0;
      return rect;
    }

    private static int GetItemState(ToolStripItem item)
    {
      var selected = item.Selected;

      if (item.IsOnDropDown) {
        if (item.Enabled) {
          return selected ?
            (int)MenuPopupItemStates.Hover :
            (int)MenuPopupItemStates.Normal;
        }
        return selected ?
          (int)MenuPopupItemStates.DisabledHover :
          (int)MenuPopupItemStates.Disabled;
      }

      if (item.Pressed) {
        return item.Enabled ?
          (int)MenuBarItemStates.Pushed :
          (int)MenuBarItemStates.DisabledPushed;
      }

      if (item.Enabled) {
        return selected ?
          (int)MenuBarItemStates.Hover :
          (int)MenuBarItemStates.Normal;
      }

      return selected ?
        (int)MenuBarItemStates.DisabledHover :
        (int)MenuBarItemStates.Disabled;
    }

    private Color GetItemTextColor(ToolStripItem item)
    {
      var partId = item.IsOnDropDown ?
        (int)MenuParts.PopupItem :
        (int)MenuParts.BarItem;
      renderer.SetParameters(MenuClass, partId, (int)GetItemState(item));
      return renderer.GetColor(ColorProperty.TextColor);
    }

    private Padding GetThemeMargins(IDeviceContext dc,
                                    MarginProperty marginType)
    {
      NativeMethods.MARGINS margins;
      try {
        var hDC = dc.GetHdc();
        var rv = NativeMethods.GetThemeMargins(
          renderer.Handle,
          hDC,
          renderer.Part,
          renderer.State,
          (int)marginType,
          IntPtr.Zero,
          out margins);
        if (rv == 0) {
          return new Padding(
            margins.cxLeftWidth,
            margins.cyTopHeight,
            margins.cxRightWidth,
            margins.cyBottomHeight);
        }
        return new Padding(0);
      }
      catch (Exception) {
        return renderer.GetMargins(dc, marginType);
      }
      finally {
        dc.ReleaseHdc();
      }
    }

    private static bool IsElementDefined(VSEInternal e)
    {
      var el = VisualStyleElement.CreateElement(e.cls, e.part, e.state);
      return VisualStyleRenderer.IsElementDefined(el);
    }

    protected override void Initialize(ToolStrip toolStrip)
    {
      if (toolStrip.Parent is ToolStripPanel) {
        toolStrip.BackColor = Color.Transparent;
      }
      base.Initialize(toolStrip);
    }

    protected override void InitializePanel(ToolStripPanel toolStripPanel)
    {
      foreach (Control control in toolStripPanel.Controls) {
        if (control is ToolStrip) {
          Initialize((ToolStrip)control);
        }
      }
      base.InitializePanel(toolStripPanel);
    }

    protected static bool IsElementDefined(string className, int part,
                                           int state)
    {
      var el = new VSEInternal(className, part, state);
      bool rv;
      if (!defined.TryGetValue(el, out rv)) {
        defined.Add(el, rv = IsElementDefined(el));
      }
      return rv;
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
      if (EnsureRenderer()) {
        e.ArrowColor = GetItemTextColor(e.Item);
      }
      base.OnRenderArrow(e);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
      if (!EnsureRenderer()) {
        base.OnRenderImageMargin(e);
        return;
      }
      if (!e.ToolStrip.IsDropDown) {
        return;
      }
      renderer.SetParameters(MenuClass, (int)MenuParts.PopupGutter, 0);
      var margins = GetThemeMargins(e.Graphics, MarginProperty.SizingMargins);
      var ts = e.ToolStrip;
      var extraWidth =
        (ts.Width - ts.DisplayRectangle.Width - margins.Left - margins.Right - 1) -
        e.AffectedBounds.Width;
      var rect = e.AffectedBounds;
      rect.Y += 2;
      rect.Height -= 4;
      var sepWidth =
        renderer.GetPartSize(e.Graphics, ThemeSizeType.True).Width;
      if (e.ToolStrip.RightToLeft == RightToLeft.Yes) {
        rect = new Rectangle(
          rect.X - extraWidth, rect.Y, sepWidth, rect.Height);
        rect.X += sepWidth;
      }
      else {
        rect = new Rectangle(
          rect.Width + extraWidth - sepWidth,
          rect.Y,
          sepWidth,
          rect.Height);
      }
      renderer.DrawBackground(e.Graphics, rect);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
      if (!EnsureRenderer()) {
        base.OnRenderItemCheck(e);
        return;
      }
      var bgRect = GetBackgroundRectangle(e.Item);
      bgRect.Width = bgRect.Height;

      if (e.Item.RightToLeft == RightToLeft.Yes) {
        bgRect = new Rectangle(
          e.ToolStrip.ClientSize.Width - bgRect.X - bgRect.Width,
          bgRect.Y,
          bgRect.Width,
          bgRect.Height);
      }
      var state = e.Item.Enabled ?
          (int)MenuPopupCheckBackgroundStates.Normal :
          (int)MenuPopupCheckBackgroundStates.Disabled;
      renderer.SetParameters(
        MenuClass,
        (int)MenuParts.PopupCheckBackground,
        state);
      renderer.DrawBackground(e.Graphics, bgRect);

      var checkRect = e.ImageRectangle;
      checkRect.X = bgRect.X + bgRect.Width / 2 - checkRect.Width / 2;
      checkRect.Y = bgRect.Y + bgRect.Height / 2 - checkRect.Height / 2;

      state = e.Item.Enabled ?
          (int)MenuPopupCheckStates.CheckmarkNormal :
          (int)MenuPopupCheckStates.CheckmarkDisabled;
      renderer.SetParameters(MenuClass, (int)MenuParts.PopupCheck, state);

      renderer.DrawBackground(e.Graphics, checkRect);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
      if (EnsureRenderer()) {
        e.TextColor = GetItemTextColor(e.Item);
      }
      base.OnRenderItemText(e);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
      if (EnsureRenderer()) {
        var partID = e.Item.IsOnDropDown ?
          (int)MenuParts.PopupItem :
          (int)MenuParts.BarItem;
        renderer.SetParameters(MenuClass, partID, GetItemState(e.Item));

        var bgRect = GetBackgroundRectangle(e.Item);
        renderer.DrawBackground(e.Graphics, bgRect, bgRect);
      }
      else {
        base.OnRenderMenuItemBackground(e);
      }
    }

    protected override void OnRenderOverflowButtonBackground(
      ToolStripItemRenderEventArgs e)
    {
      if (EnsureRenderer()) {
        var rebarClass = RebarClass;
        if (Theme == ToolbarTheme.BrowserTabBar) {
          rebarClass = "Rebar";
        }
        var state = VisualStyleElement.Rebar.Chevron.Normal.State;
        if (e.Item.Pressed) {
          state = VisualStyleElement.Rebar.Chevron.Pressed.State;
        }
        else {
          if (e.Item.Selected) {
            state = VisualStyleElement.Rebar.Chevron.Hot.State;
          }
        }
        renderer.SetParameters(
          rebarClass,
          VisualStyleElement.Rebar.Chevron.Normal.Part,
          state);
        renderer.DrawBackground(
          e.Graphics, new Rectangle(Point.Empty, e.Item.Size));
      }
      else {
        base.OnRenderOverflowButtonBackground(e);
      }
    }

    protected override void OnRenderSeparator(
      ToolStripSeparatorRenderEventArgs e)
    {
      if (e.ToolStrip.IsDropDown && EnsureRenderer()) {
        renderer.SetParameters(MenuClass, (int)MenuParts.PopupSeparator, 0);
        var rect = new Rectangle(
          e.ToolStrip.DisplayRectangle.Left,
          0,
          e.ToolStrip.DisplayRectangle.Width,
          e.Item.Height);
        renderer.DrawBackground(e.Graphics, rect, rect);
      }
      else {
        base.OnRenderSeparator(e);
      }
    }

    protected override void OnRenderSplitButtonBackground(
      ToolStripItemRenderEventArgs e)
    {
      if (!EnsureRenderer()) {
        base.OnRenderSplitButtonBackground(e);
        return;
      }
      var sb = (ToolStripSplitButton)e.Item;
      base.OnRenderSplitButtonBackground(e);
      OnRenderArrow(new ToolStripArrowRenderEventArgs(
        e.Graphics,
        sb,
        sb.DropDownButtonBounds,
        Color.Red,
        ArrowDirection.Down));
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
      if (!EnsureRenderer()) {
        base.OnRenderToolStripBackground(e);
        return;
      }
      if (e.ToolStrip.IsDropDown) {
        renderer.SetParameters(MenuClass, (int)MenuParts.PopupBackground, 0);
      }
      else {
        if (e.ToolStrip.Parent is ToolStripPanel) {
          return;
        }
        if (IsElementDefined(RebarClass, RebarBackground, 0)) {
          renderer.SetParameters(RebarClass, RebarBackground, 0);
        }
        else {
          renderer.SetParameters(RebarClass, 0, 0);
        }
      }

      if (renderer.IsBackgroundPartiallyTransparent()) {
        renderer.DrawParentBackground(
          e.Graphics,
          e.ToolStrip.ClientRectangle,
          e.ToolStrip);
      }
      renderer.DrawBackground(
        e.Graphics,
        e.ToolStrip.ClientRectangle,
        e.AffectedBounds);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
      if (!EnsureRenderer()) {
        base.OnRenderToolStripBorder(e);
        return;
      }
      renderer.SetParameters(MenuClass, (int)MenuParts.PopupBorders, 0);
      if (e.ToolStrip.IsDropDown) {
        var oldClip = e.Graphics.Clip;
        var insideRect = e.ToolStrip.ClientRectangle;
        insideRect.Inflate(-1, -1);
        e.Graphics.ExcludeClip(insideRect);
        renderer.DrawBackground(
          e.Graphics,
          e.ToolStrip.ClientRectangle,
          e.AffectedBounds);
        e.Graphics.Clip = oldClip;
      }
    }

    protected override void OnRenderToolStripPanelBackground(
      ToolStripPanelRenderEventArgs e)
    {
      if (!EnsureRenderer()) {
        base.OnRenderToolStripPanelBackground(e);
        return;
      }
      if (IsElementDefined(RebarClass, RebarBackground, 0)) {
        renderer.SetParameters(RebarClass, RebarBackground, 0);
      }
      else {
        renderer.SetParameters(RebarClass, 0, 0);
      }

      if (renderer.IsBackgroundPartiallyTransparent()) {
        renderer.DrawParentBackground(
          e.Graphics,
          e.ToolStripPanel.ClientRectangle,
          e.ToolStripPanel);
      }
      renderer.DrawBackground(e.Graphics, e.ToolStripPanel.ClientRectangle);

      e.Handled = true;
    }

    private struct VSEInternal
    {
      public readonly string cls;

      public readonly int part;

      public readonly int state;

      public VSEInternal(string cls, int part, int state)
      {
        this.cls = cls;
        this.part = part;
        this.state = state;
      }
    }

    private static class NativeMethods
    {
      [DllImport("uxtheme.dll")]
      public extern static int GetThemeMargins(
        IntPtr hTheme,
        IntPtr hdc,
        int iPartId,
        int iStateId,
        int iPropId,
        IntPtr rect,
      out MARGINS pMargins);

      [StructLayout(LayoutKind.Sequential)]
      public struct MARGINS
      {
        public int cxLeftWidth;

        public int cxRightWidth;

        public int cyTopHeight;

        public int cyBottomHeight;
      }
    }
  }
}

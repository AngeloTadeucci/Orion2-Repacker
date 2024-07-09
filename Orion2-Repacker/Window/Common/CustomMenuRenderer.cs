
namespace Orion.Window.Common;
public class CustomMenuRenderer : ToolStripProfessionalRenderer {
    private readonly ITheme _currentTheme;
    public CustomMenuRenderer(ITheme currentTheme) : base(new CustomColorTable(currentTheme)) {
        _currentTheme = currentTheme;
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
        ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;

        if (menuItem != null && (menuItem.Selected || (menuItem.DropDown.Visible && menuItem.OwnerItem == null))) {
            // Define the colors for the selected item
            Color selectedColor = _currentTheme.MenuItemHover;
            Color borderColor = _currentTheme.ForeColor2;

            // Fill the background with the selected color
            e.Graphics.FillRectangle(new SolidBrush(selectedColor), new Rectangle(Point.Empty, e.Item.Size));

            // Draw the border
            e.Graphics.DrawRectangle(new Pen(borderColor), new Rectangle(Point.Empty, e.Item.Size - new Size(1, 1)));
        } else {
            base.OnRenderMenuItemBackground(e);
        }
    }

    // Override the method to render the image margin (removes the white margin)
    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
        // Do nothing to remove the margin
    }

    private class CustomColorTable() : ProfessionalColorTable {
        private readonly ITheme _currentTheme;

        public CustomColorTable(ITheme currentTheme) : this() {
            _currentTheme = currentTheme;
        }

        public override Color MenuStripGradientBegin {
            get { return Color.Yellow; }
        }
        public override Color MenuStripGradientEnd {
            get { return Color.Yellow; }
        }

        public override Color MenuItemSelectedGradientBegin {
            get { return _currentTheme.MenuItemHover; }
        }
        public override Color MenuItemSelectedGradientEnd {
            get { return _currentTheme.MenuItemHover; }
        }

        public override Color MenuItemBorder {
            get { return _currentTheme.ForeColor2; }
        }
    }
}



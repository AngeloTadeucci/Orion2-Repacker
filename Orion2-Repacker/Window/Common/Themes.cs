namespace Orion.Window.Common;

public interface ITheme {
    Color BackColor { get; }
    Color ForeColor { get; }
    Color BackColor2 { get; }
    Color ForeColor2 { get; }

    Color MenuItemHover { get; }
}

public class LightTheme : ITheme {
    public Color BackColor => Color.FromArgb(255, 255, 254);
    public Color ForeColor => Color.FromArgb(33, 33, 33);
    public Color BackColor2 => Color.FromArgb(240, 240, 240);
    public Color ForeColor2 => Color.FromArgb(33, 33, 33);

    public Color MenuItemHover => Color.LightCyan;
}

public class DarkTheme : ITheme {
    public Color BackColor => Color.FromArgb(30, 30, 30);
    public Color ForeColor => Color.FromArgb(255, 255, 255);
    public Color BackColor2 => Color.FromArgb(60, 60, 60);
    public Color ForeColor2 => Color.FromArgb(240, 240, 240);

    public Color MenuItemHover => Color.FromArgb(70, 70, 70);
}
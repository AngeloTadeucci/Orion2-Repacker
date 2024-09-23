
using Orion.Window.Common;

namespace Orion.Window; 
public partial class ConvertConfirmation : Form {
    public ConvertConfirmation(ITheme currentTheme) {
        InitializeComponent();

        BackColor = currentTheme.BackColor2;
        ForeColor = currentTheme.ForeColor2;

        // Define an array of controls (e.g., buttons, panels) to apply the same color settings
        Control[] controls = [buttonAlways, buttonCancel, buttonYes, label1, label2];

        // Apply color settings to all controls
        foreach (Control control in controls) {
            control.BackColor = currentTheme.BackColor2;
            control.ForeColor = currentTheme.ForeColor2;
        }
    }

    private void buttonCancel_Click(object sender, EventArgs e) {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void buttonYes_Click(object sender, EventArgs e) {
        DialogResult = DialogResult.Yes;
        Close();
    }

    private void buttonAlways_Click(object sender, EventArgs e) {
        DialogResult = DialogResult.OK;
        Close();
    }
}

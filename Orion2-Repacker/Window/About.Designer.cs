namespace Orion.Window;

partial class About
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
        if (disposing && (components != null))
        {
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
        okButton = new Button();
        textBoxDescription = new TextBox();
        labelSpecialThanks = new Label();
        labelCreator = new Label();
        labelVersion = new Label();
        labelProductName = new Label();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // okButton
        // 
        okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        okButton.DialogResult = DialogResult.Cancel;
        okButton.ForeColor = Color.Black;
        okButton.Location = new Point(464, 277);
        okButton.Margin = new Padding(4, 3, 4, 3);
        okButton.Name = "okButton";
        okButton.Size = new Size(88, 27);
        okButton.TabIndex = 24;
        okButton.Text = "&OK";
        // 
        // textBoxDescription
        // 
        textBoxDescription.BackColor = Color.FromArgb(180, 180, 180);
        textBoxDescription.Dock = DockStyle.Fill;
        textBoxDescription.ForeColor = Color.Black;
        textBoxDescription.Location = new Point(7, 123);
        textBoxDescription.Margin = new Padding(7, 3, 4, 3);
        textBoxDescription.Multiline = true;
        textBoxDescription.Name = "textBoxDescription";
        textBoxDescription.ReadOnly = true;
        textBoxDescription.ScrollBars = ScrollBars.Both;
        textBoxDescription.Size = new Size(545, 147);
        textBoxDescription.TabIndex = 23;
        textBoxDescription.TabStop = false;
        textBoxDescription.Text = "About Orion2 Repacker";
        // 
        // labelSpecialThanks
        // 
        labelSpecialThanks.Dock = DockStyle.Fill;
        labelSpecialThanks.ForeColor = Color.Black;
        labelSpecialThanks.Location = new Point(7, 90);
        labelSpecialThanks.Margin = new Padding(7, 0, 4, 0);
        labelSpecialThanks.MaximumSize = new Size(0, 20);
        labelSpecialThanks.Name = "labelSpecialThanks";
        labelSpecialThanks.Size = new Size(545, 20);
        labelSpecialThanks.TabIndex = 22;
        labelSpecialThanks.Text = "Special Thanks";
        labelSpecialThanks.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // labelCreator
        // 
        labelCreator.Dock = DockStyle.Fill;
        labelCreator.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
        labelCreator.ForeColor = Color.Black;
        labelCreator.Location = new Point(7, 60);
        labelCreator.Margin = new Padding(7, 0, 4, 0);
        labelCreator.MaximumSize = new Size(0, 20);
        labelCreator.Name = "labelCreator";
        labelCreator.Size = new Size(545, 20);
        labelCreator.TabIndex = 21;
        labelCreator.Text = "Created by Eric";
        labelCreator.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // labelVersion
        // 
        labelVersion.Dock = DockStyle.Fill;
        labelVersion.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
        labelVersion.ForeColor = Color.Black;
        labelVersion.Location = new Point(7, 30);
        labelVersion.Margin = new Padding(7, 0, 4, 0);
        labelVersion.MaximumSize = new Size(0, 20);
        labelVersion.Name = "labelVersion";
        labelVersion.Size = new Size(545, 20);
        labelVersion.TabIndex = 0;
        labelVersion.Text = "    Version";
        labelVersion.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // labelProductName
        // 
        labelProductName.Dock = DockStyle.Fill;
        labelProductName.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point);
        labelProductName.ForeColor = Color.Black;
        labelProductName.Location = new Point(7, 0);
        labelProductName.Margin = new Padding(7, 0, 4, 0);
        labelProductName.MaximumSize = new Size(0, 24);
        labelProductName.Name = "labelProductName";
        labelProductName.Size = new Size(545, 24);
        labelProductName.TabIndex = 19;
        labelProductName.Text = "Orion2 Repacker";
        labelProductName.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        tableLayoutPanel.Controls.Add(labelProductName, 0, 0);
        tableLayoutPanel.Controls.Add(labelVersion, 0, 1);
        tableLayoutPanel.Controls.Add(labelCreator, 0, 2);
        tableLayoutPanel.Controls.Add(labelSpecialThanks, 0, 3);
        tableLayoutPanel.Controls.Add(textBoxDescription, 0, 4);
        tableLayoutPanel.Controls.Add(okButton, 0, 5);
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.Location = new Point(10, 10);
        tableLayoutPanel.Margin = new Padding(4, 3, 4, 3);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 6;
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel.Size = new Size(556, 307);
        tableLayoutPanel.TabIndex = 0;
        // 
        // About
        // 
        AcceptButton = okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(180, 180, 180);
        ClientSize = new Size(576, 327);
        Controls.Add(tableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "About";
        Padding = new Padding(10);
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "About";
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.TextBox textBoxDescription;
    private System.Windows.Forms.Label labelSpecialThanks;
    private System.Windows.Forms.Label labelCreator;
    private System.Windows.Forms.Label labelVersion;
    private System.Windows.Forms.Label labelProductName;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
}

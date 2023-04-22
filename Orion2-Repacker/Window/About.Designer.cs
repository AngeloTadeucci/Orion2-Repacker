namespace Orion.Window
{
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
            okButton = new System.Windows.Forms.Button();
            textBoxDescription = new System.Windows.Forms.TextBox();
            labelSpecialThanks = new System.Windows.Forms.Label();
            labelCreator = new System.Windows.Forms.Label();
            labelVersion = new System.Windows.Forms.Label();
            labelProductName = new System.Windows.Forms.Label();
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            okButton.ForeColor = System.Drawing.Color.Black;
            okButton.Location = new System.Drawing.Point(464, 277);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(88, 27);
            okButton.TabIndex = 24;
            okButton.Text = "&OK";
            // 
            // textBoxDescription
            // 
            textBoxDescription.BackColor = System.Drawing.Color.FromArgb(180, 180, 180);
            textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxDescription.ForeColor = System.Drawing.Color.Black;
            textBoxDescription.Location = new System.Drawing.Point(7, 123);
            textBoxDescription.Margin = new System.Windows.Forms.Padding(7, 3, 4, 3);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBoxDescription.Size = new System.Drawing.Size(545, 147);
            textBoxDescription.TabIndex = 23;
            textBoxDescription.TabStop = false;
            textBoxDescription.Text = "About Orion2 Repacker";
            // 
            // labelSpecialThanks
            // 
            labelSpecialThanks.Dock = System.Windows.Forms.DockStyle.Fill;
            labelSpecialThanks.ForeColor = System.Drawing.Color.Black;
            labelSpecialThanks.Location = new System.Drawing.Point(7, 90);
            labelSpecialThanks.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelSpecialThanks.MaximumSize = new System.Drawing.Size(0, 20);
            labelSpecialThanks.Name = "labelSpecialThanks";
            labelSpecialThanks.Size = new System.Drawing.Size(545, 20);
            labelSpecialThanks.TabIndex = 22;
            labelSpecialThanks.Text = "Special Thanks";
            labelSpecialThanks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCreator
            // 
            labelCreator.Dock = System.Windows.Forms.DockStyle.Fill;
            labelCreator.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelCreator.ForeColor = System.Drawing.Color.Black;
            labelCreator.Location = new System.Drawing.Point(7, 60);
            labelCreator.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelCreator.MaximumSize = new System.Drawing.Size(0, 20);
            labelCreator.Name = "labelCreator";
            labelCreator.Size = new System.Drawing.Size(545, 20);
            labelCreator.TabIndex = 21;
            labelCreator.Text = "Created by Eric";
            labelCreator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelVersion.ForeColor = System.Drawing.Color.Black;
            labelVersion.Location = new System.Drawing.Point(7, 30);
            labelVersion.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelVersion.MaximumSize = new System.Drawing.Size(0, 20);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new System.Drawing.Size(545, 20);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "    Version";
            labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelProductName
            // 
            labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            labelProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelProductName.ForeColor = System.Drawing.Color.Black;
            labelProductName.Location = new System.Drawing.Point(7, 0);
            labelProductName.Margin = new System.Windows.Forms.Padding(7, 0, 4, 0);
            labelProductName.MaximumSize = new System.Drawing.Size(0, 24);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new System.Drawing.Size(545, 24);
            labelProductName.TabIndex = 19;
            labelProductName.Text = "Orion2 Repacker";
            labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel.Controls.Add(labelProductName, 0, 0);
            tableLayoutPanel.Controls.Add(labelVersion, 0, 1);
            tableLayoutPanel.Controls.Add(labelCreator, 0, 2);
            tableLayoutPanel.Controls.Add(labelSpecialThanks, 0, 3);
            tableLayoutPanel.Controls.Add(textBoxDescription, 0, 4);
            tableLayoutPanel.Controls.Add(okButton, 0, 5);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(10, 10);
            tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel.Size = new System.Drawing.Size(556, 307);
            tableLayoutPanel.TabIndex = 0;
            // 
            // About
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(180, 180, 180);
            ClientSize = new System.Drawing.Size(576, 327);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "About";
            Padding = new System.Windows.Forms.Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
}

namespace Orion.Window {
    partial class ConvertConfirmation {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertConfirmation));
            labelConfirmation = new Label();
            buttonCancel = new Button();
            buttonYes = new Button();
            buttonAlways = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // labelConfirmation
            // 
            labelConfirmation.AutoSize = true;
            labelConfirmation.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelConfirmation.Location = new Point(12, 9);
            labelConfirmation.Name = "labelConfirmation";
            labelConfirmation.Size = new Size(240, 21);
            labelConfirmation.TabIndex = 0;
            labelConfirmation.Text = "Convert this .usm file to .mp4?";
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(12, 63);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "No";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonYes
            // 
            buttonYes.Location = new Point(93, 63);
            buttonYes.Name = "buttonYes";
            buttonYes.Size = new Size(75, 23);
            buttonYes.TabIndex = 2;
            buttonYes.Text = "Yes";
            buttonYes.UseVisualStyleBackColor = true;
            buttonYes.Click += buttonYes_Click;
            // 
            // buttonAlways
            // 
            buttonAlways.Location = new Point(174, 63);
            buttonAlways.Name = "buttonAlways";
            buttonAlways.Size = new Size(75, 23);
            buttonAlways.TabIndex = 3;
            buttonAlways.Text = "Always";
            buttonAlways.UseVisualStyleBackColor = true;
            buttonAlways.Click += buttonAlways_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 30);
            label1.Name = "label1";
            label1.Size = new Size(178, 15);
            label1.TabIndex = 4;
            label1.Text = "Converted files will be cached to";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 45);
            label2.Name = "label2";
            label2.Size = new Size(114, 15);
            label2.TabIndex = 5;
            label2.Text = "your appdata folder.";
            // 
            // ConvertConfirmation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(264, 97);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(buttonAlways);
            Controls.Add(buttonYes);
            Controls.Add(buttonCancel);
            Controls.Add(labelConfirmation);
            Icon = (Icon) resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConvertConfirmation";
            Text = "Convert to mp4?";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelConfirmation;
        private Button buttonCancel;
        private Button buttonYes;
        private Button buttonAlways;
        private Label label1;
        private Label label2;
    }
}
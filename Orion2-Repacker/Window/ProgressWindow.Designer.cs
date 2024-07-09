/*
 *      This file is part of Orion2, a MapleStory2 Packaging Library Project.
 *      Copyright (C) 2018 Eric Smith <notericsoft@gmail.com>
 * 
 *      This program is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 * 
 *      This program is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 * 
 *      You should have received a copy of the GNU General Public License
 */

namespace Orion.Window;

partial class ProgressWindow
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
    private void InitializeComponent() {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressWindow));
        pSaveInfo = new Label();
        pProgressBar = new ProgressBar();
        SuspendLayout();
        // 
        // pSaveInfo
        // 
        pSaveInfo.Location = new Point(14, 10);
        pSaveInfo.Margin = new Padding(4, 0, 4, 0);
        pSaveInfo.Name = "pSaveInfo";
        pSaveInfo.Size = new Size(349, 25);
        pSaveInfo.TabIndex = 0;
        pSaveInfo.Text = "Saving ...";
        pSaveInfo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pProgressBar
        // 
        pProgressBar.Location = new Point(14, 39);
        pProgressBar.Margin = new Padding(4, 3, 4, 3);
        pProgressBar.Name = "pProgressBar";
        pProgressBar.Size = new Size(349, 38);
        pProgressBar.Step = 1;
        pProgressBar.Style = ProgressBarStyle.Continuous;
        pProgressBar.TabIndex = 1;
        // 
        // ProgressWindow
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(377, 91);
        ControlBox = false;
        Controls.Add(pProgressBar);
        Controls.Add(pSaveInfo);
        Icon = (Icon) resources.GetObject("$this.Icon");
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProgressWindow";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Saving...";
        TopMost = true;
        Load += ProgressWindow_Load;
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Label pSaveInfo;
    private System.Windows.Forms.ProgressBar pProgressBar;
}
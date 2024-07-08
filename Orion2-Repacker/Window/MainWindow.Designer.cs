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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Orion.Window;

partial class MainWindow {
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

    private class MenuRenderer : ToolStripProfessionalRenderer {
        public MenuRenderer() : base(new MenuColors()) { }
    }

    private class MenuColors : ProfessionalColorTable {
        private readonly Color SYS_COLOR = Color.FromArgb(((int) (((byte) (181)))), ((int) (((byte) (215)))), ((int) (((byte) (243)))));

        /* Top gradient of selected upper menu items */
        public override Color MenuItemSelectedGradientBegin {
            get { return SYS_COLOR; }
        }

        /* Bottom gradient of selected upper menu items */
        public override Color MenuItemSelectedGradientEnd {
            get { return SYS_COLOR; }
        }

        /* Top gradient of pressed upper menu items */
        public override Color MenuItemPressedGradientBegin {
            get { return SYS_COLOR; }
        }

        /* Bottom gradient of pressed upper menu items */
        public override Color MenuItemPressedGradientEnd {
            get { return SYS_COLOR; }
        }

        /* Global menu item border coloring */
        public override Color MenuItemBorder {
            get { return SYS_COLOR; }
        }

        /* Color of sub-menu items */
        public override Color MenuItemSelected {
            get { return SYS_COLOR; }
        }
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
        pMenuStrip = new MenuStrip();
        pFileMenuStripItem = new ToolStripMenuItem();
        pOpenMenuItem = new ToolStripMenuItem();
        pSaveMenuItem = new ToolStripMenuItem();
        pReloadMenuItem = new ToolStripMenuItem();
        pUnloadMenuItem = new ToolStripMenuItem();
        exitToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem = new ToolStripMenuItem();
        addToolStripMenuItem = new ToolStripMenuItem();
        addFolderToolStripMenuItem = new ToolStripMenuItem();
        removeToolStripMenuItem = new ToolStripMenuItem();
        copyToolStripMenuItem = new ToolStripMenuItem();
        pasteToolStripMenuItem = new ToolStripMenuItem();
        allNodesToolStripMenuItem = new ToolStripMenuItem();
        expandToolStripMenuItem = new ToolStripMenuItem();
        collapseToolStripMenuItem = new ToolStripMenuItem();
        toolsToolStripMenuItem = new ToolStripMenuItem();
        exportToolStripMenuItem = new ToolStripMenuItem();
        searchToolStripMenuItem = new ToolStripMenuItem();
        createItemToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem = new ToolStripMenuItem();
        aboutToolStripMenuItem = new ToolStripMenuItem();
        editorSettingsToolStripMenuItem = new ToolStripMenuItem();
        themeToolStripMenuItem = new ToolStripMenuItem();
        lightToolStripMenuItem = new ToolStripMenuItem();
        darkToolStripMenuItem = new ToolStripMenuItem();
        wordWrapToolStripMenuItem = new ToolStripMenuItem();
        testToolStripMenuItem = new ToolStripMenuItem();
        lightToolStripTheme = new ToolStripMenuItem();
        darkToolStripTheme = new ToolStripMenuItem();
        pTreeView = new TreeView();
        pEntryName = new TextBox();
        pImageData = new PictureBox();
        pImagePanel = new Panel();
        webView = new Microsoft.Web.WebView2.WinForms.WebView2();
        pUpdateDataBtn = new Button();
        pChangeImageBtn = new Button();
        pSaveWorkerThread = new System.ComponentModel.BackgroundWorker();
        pEntryValue = new Label();
        extractWorkerThread = new System.ComponentModel.BackgroundWorker();
        pMenuStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize) pImageData).BeginInit();
        pImagePanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize) webView).BeginInit();
        SuspendLayout();
        // 
        // pMenuStrip
        // 
        pMenuStrip.BackColor = Color.FromArgb(  240,   240,   240);
        pMenuStrip.Items.AddRange(new ToolStripItem[] { pFileMenuStripItem, editToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem, editorSettingsToolStripMenuItem, testToolStripMenuItem });
        pMenuStrip.Location = new Point(0, 0);
        pMenuStrip.Name = "pMenuStrip";
        pMenuStrip.Padding = new Padding(7, 2, 0, 2);
        pMenuStrip.Size = new Size(1112, 24);
        pMenuStrip.TabIndex = 0;
        pMenuStrip.Text = "menuStrip1";
        // 
        // pFileMenuStripItem
        // 
        pFileMenuStripItem.DropDownItems.AddRange(new ToolStripItem[] { pOpenMenuItem, pSaveMenuItem, pReloadMenuItem, pUnloadMenuItem, exitToolStripMenuItem });
        pFileMenuStripItem.ForeColor = Color.Black;
        pFileMenuStripItem.Name = "pFileMenuStripItem";
        pFileMenuStripItem.Size = new Size(37, 20);
        pFileMenuStripItem.Text = "File";
        // 
        // pOpenMenuItem
        // 
        pOpenMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        pOpenMenuItem.ForeColor = Color.Black;
        pOpenMenuItem.Name = "pOpenMenuItem";
        pOpenMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
        pOpenMenuItem.ShortcutKeys =  Keys.Control | Keys.O;
        pOpenMenuItem.Size = new Size(154, 22);
        pOpenMenuItem.Text = "Open";
        pOpenMenuItem.Click += OnLoadFile;
        // 
        // pSaveMenuItem
        // 
        pSaveMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        pSaveMenuItem.ForeColor = Color.Black;
        pSaveMenuItem.Name = "pSaveMenuItem";
        pSaveMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
        pSaveMenuItem.ShortcutKeys =  Keys.Control | Keys.S;
        pSaveMenuItem.Size = new Size(154, 22);
        pSaveMenuItem.Text = "Save";
        pSaveMenuItem.Click += OnSaveFile;
        // 
        // pReloadMenuItem
        // 
        pReloadMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        pReloadMenuItem.ForeColor = Color.Black;
        pReloadMenuItem.Name = "pReloadMenuItem";
        pReloadMenuItem.ShortcutKeyDisplayString = "Ctrl+R";
        pReloadMenuItem.ShortcutKeys =  Keys.Control | Keys.R;
        pReloadMenuItem.Size = new Size(154, 22);
        pReloadMenuItem.Text = "Reload";
        pReloadMenuItem.Click += OnReloadFile;
        // 
        // pUnloadMenuItem
        // 
        pUnloadMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        pUnloadMenuItem.ForeColor = Color.Black;
        pUnloadMenuItem.Name = "pUnloadMenuItem";
        pUnloadMenuItem.ShortcutKeyDisplayString = "Ctrl+U";
        pUnloadMenuItem.ShortcutKeys =  Keys.Control | Keys.U;
        pUnloadMenuItem.Size = new Size(154, 22);
        pUnloadMenuItem.Text = "Unload";
        pUnloadMenuItem.Click += OnUnloadFile;
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        exitToolStripMenuItem.ForeColor = Color.Black;
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(154, 22);
        exitToolStripMenuItem.Text = "Exit";
        exitToolStripMenuItem.Click += OnExit;
        // 
        // editToolStripMenuItem
        // 
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addToolStripMenuItem, addFolderToolStripMenuItem, removeToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, allNodesToolStripMenuItem });
        editToolStripMenuItem.ForeColor = Color.Black;
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.Size = new Size(39, 20);
        editToolStripMenuItem.Text = "Edit";
        // 
        // addToolStripMenuItem
        // 
        addToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        addToolStripMenuItem.ForeColor = Color.Black;
        addToolStripMenuItem.Name = "addToolStripMenuItem";
        addToolStripMenuItem.Size = new Size(141, 22);
        addToolStripMenuItem.Text = "Add items";
        addToolStripMenuItem.Click += OnAddFile;
        // 
        // addFolderToolStripMenuItem
        // 
        addFolderToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        addFolderToolStripMenuItem.ForeColor = Color.Black;
        addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
        addFolderToolStripMenuItem.Size = new Size(141, 22);
        addFolderToolStripMenuItem.Text = "Add folder";
        addFolderToolStripMenuItem.Click += OnAddFolder;
        // 
        // removeToolStripMenuItem
        // 
        removeToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        removeToolStripMenuItem.ForeColor = Color.Black;
        removeToolStripMenuItem.Name = "removeToolStripMenuItem";
        removeToolStripMenuItem.ShortcutKeyDisplayString = "";
        removeToolStripMenuItem.ShortcutKeys = Keys.Delete;
        removeToolStripMenuItem.Size = new Size(141, 22);
        removeToolStripMenuItem.Text = "Remove";
        removeToolStripMenuItem.Click += OnRemoveFile;
        // 
        // copyToolStripMenuItem
        // 
        copyToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        copyToolStripMenuItem.ForeColor = Color.Black;
        copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        copyToolStripMenuItem.ShortcutKeyDisplayString = "";
        copyToolStripMenuItem.Size = new Size(141, 22);
        copyToolStripMenuItem.Text = "Copy";
        copyToolStripMenuItem.Click += OnCopyNode;
        // 
        // pasteToolStripMenuItem
        // 
        pasteToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        pasteToolStripMenuItem.ForeColor = Color.Black;
        pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
        pasteToolStripMenuItem.ShortcutKeyDisplayString = "";
        pasteToolStripMenuItem.Size = new Size(141, 22);
        pasteToolStripMenuItem.Text = "Paste";
        pasteToolStripMenuItem.Click += OnPasteNode;
        // 
        // allNodesToolStripMenuItem
        // 
        allNodesToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        allNodesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { expandToolStripMenuItem, collapseToolStripMenuItem });
        allNodesToolStripMenuItem.ForeColor = Color.Black;
        allNodesToolStripMenuItem.Name = "allNodesToolStripMenuItem";
        allNodesToolStripMenuItem.Size = new Size(141, 22);
        allNodesToolStripMenuItem.Text = "All Nodes";
        // 
        // expandToolStripMenuItem
        // 
        expandToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        expandToolStripMenuItem.ForeColor = Color.Black;
        expandToolStripMenuItem.Name = "expandToolStripMenuItem";
        expandToolStripMenuItem.Size = new Size(119, 22);
        expandToolStripMenuItem.Text = "Expand";
        expandToolStripMenuItem.Click += OnExpandNodes;
        // 
        // collapseToolStripMenuItem
        // 
        collapseToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        collapseToolStripMenuItem.ForeColor = Color.Black;
        collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
        collapseToolStripMenuItem.Size = new Size(119, 22);
        collapseToolStripMenuItem.Text = "Collapse";
        collapseToolStripMenuItem.Click += OnCollapseNodes;
        // 
        // toolsToolStripMenuItem
        // 
        toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportToolStripMenuItem, searchToolStripMenuItem, createItemToolStripMenuItem });
        toolsToolStripMenuItem.ForeColor = Color.Black;
        toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        toolsToolStripMenuItem.Size = new Size(46, 20);
        toolsToolStripMenuItem.Text = "Tools";
        // 
        // exportToolStripMenuItem
        // 
        exportToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        exportToolStripMenuItem.ForeColor = Color.Black;
        exportToolStripMenuItem.Name = "exportToolStripMenuItem";
        exportToolStripMenuItem.Size = new Size(159, 22);
        exportToolStripMenuItem.Text = "Export";
        exportToolStripMenuItem.Click += OnExport;
        // 
        // searchToolStripMenuItem
        // 
        searchToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        searchToolStripMenuItem.ForeColor = Color.Black;
        searchToolStripMenuItem.Name = "searchToolStripMenuItem";
        searchToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F";
        searchToolStripMenuItem.ShortcutKeys =  Keys.Control | Keys.F;
        searchToolStripMenuItem.Size = new Size(159, 22);
        searchToolStripMenuItem.Text = "Search";
        searchToolStripMenuItem.Click += OnSearch;
        // 
        // createItemToolStripMenuItem
        // 
        createItemToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        createItemToolStripMenuItem.ForeColor = Color.Black;
        createItemToolStripMenuItem.Name = "createItemToolStripMenuItem";
        createItemToolStripMenuItem.Size = new Size(159, 22);
        createItemToolStripMenuItem.Text = "Add item helper";
        createItemToolStripMenuItem.Click += OnCreateItem;
        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
        helpToolStripMenuItem.ForeColor = Color.Black;
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(44, 20);
        helpToolStripMenuItem.Text = "Help";
        // 
        // aboutToolStripMenuItem
        // 
        aboutToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        aboutToolStripMenuItem.ForeColor = Color.Black;
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Size = new Size(107, 22);
        aboutToolStripMenuItem.Text = "About";
        aboutToolStripMenuItem.Click += OnAbout;
        // 
        // editorSettingsToolStripMenuItem
        // 
        editorSettingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { themeToolStripMenuItem, wordWrapToolStripMenuItem });
        editorSettingsToolStripMenuItem.ForeColor = Color.Black;
        editorSettingsToolStripMenuItem.Name = "editorSettingsToolStripMenuItem";
        editorSettingsToolStripMenuItem.Size = new Size(95, 20);
        editorSettingsToolStripMenuItem.Text = "Editor Settings";
        // 
        // themeToolStripMenuItem
        // 
        themeToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        themeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { lightToolStripMenuItem, darkToolStripMenuItem });
        themeToolStripMenuItem.ForeColor = Color.Black;
        themeToolStripMenuItem.Name = "themeToolStripMenuItem";
        themeToolStripMenuItem.Size = new Size(134, 22);
        themeToolStripMenuItem.Text = "Themes";
        // 
        // lightToolStripMenuItem
        // 
        lightToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        lightToolStripMenuItem.ForeColor = Color.Black;
        lightToolStripMenuItem.Name = "lightToolStripMenuItem";
        lightToolStripMenuItem.Size = new Size(101, 22);
        lightToolStripMenuItem.Text = "Light";
        lightToolStripMenuItem.Click += lightToolStripMenuItem_Click;
        // 
        // darkToolStripMenuItem
        // 
        darkToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        darkToolStripMenuItem.ForeColor = Color.Black;
        darkToolStripMenuItem.Name = "darkToolStripMenuItem";
        darkToolStripMenuItem.Size = new Size(101, 22);
        darkToolStripMenuItem.Text = "Dark";
        darkToolStripMenuItem.Click += darkToolStripMenuItem_Click;
        // 
        // wordWrapToolStripMenuItem
        // 
        wordWrapToolStripMenuItem.BackColor = Color.FromArgb(  240,   240,   240);
        wordWrapToolStripMenuItem.Checked = true;
        wordWrapToolStripMenuItem.CheckState = CheckState.Checked;
        wordWrapToolStripMenuItem.ForeColor = Color.Black;
        wordWrapToolStripMenuItem.Name = "wordWrapToolStripMenuItem";
        wordWrapToolStripMenuItem.Size = new Size(134, 22);
        wordWrapToolStripMenuItem.Text = "Word Wrap";
        wordWrapToolStripMenuItem.Click += wordWrapToolStripMenuItem_Click;
        // 
        // testToolStripMenuItem
        // 
        testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { lightToolStripTheme, darkToolStripTheme });
        testToolStripMenuItem.ForeColor = Color.Black;
        testToolStripMenuItem.Name = "testToolStripMenuItem";
        testToolStripMenuItem.Size = new Size(55, 20);
        testToolStripMenuItem.Text = "Theme";
        // 
        // lightToolStripTheme
        // 
        lightToolStripTheme.Checked = true;
        lightToolStripTheme.CheckState = CheckState.Checked;
        lightToolStripTheme.Name = "lightToolStripTheme";
        lightToolStripTheme.Size = new Size(180, 22);
        lightToolStripTheme.Text = "Light";
        lightToolStripTheme.Click += lightToolStripTheme_Click;
        // 
        // darkToolStripTheme
        // 
        darkToolStripTheme.Name = "darkToolStripTheme";
        darkToolStripTheme.Size = new Size(180, 22);
        darkToolStripTheme.Text = "Dark";
        darkToolStripTheme.Click += darkToolStripTheme_Click;
        // 
        // pTreeView
        // 
        pTreeView.AllowDrop = true;
        pTreeView.BackColor = Color.White;
        pTreeView.ForeColor = Color.Black;
        pTreeView.Location = new Point(0, 28);
        pTreeView.Margin = new Padding(4, 3, 4, 3);
        pTreeView.Name = "pTreeView";
        pTreeView.Size = new Size(521, 612);
        pTreeView.TabIndex = 1;
        pTreeView.AfterSelect += OnSelectNode;
        pTreeView.NodeMouseDoubleClick += OnDoubleClickNode;
        pTreeView.DragDrop += pTreeView_DragDrop;
        pTreeView.DragEnter += pTreeView_DragEnter;
        pTreeView.DragOver += pTreeView_DragOver;
        pTreeView.DragLeave += pTreeView_DragLeave;
        pTreeView.MouseClick += pTreeView_MouseClick;
        // 
        // pEntryName
        // 
        pEntryName.BackColor = Color.White;
        pEntryName.Font = new Font("Microsoft Sans Serif", 10F);
        pEntryName.ForeColor = Color.Black;
        pEntryName.Location = new Point(528, 31);
        pEntryName.Margin = new Padding(4, 3, 4, 3);
        pEntryName.Name = "pEntryName";
        pEntryName.ReadOnly = true;
        pEntryName.Size = new Size(331, 23);
        pEntryName.TabIndex = 2;
        pEntryName.Visible = false;
        pEntryName.WordWrap = false;
        // 
        // pImageData
        // 
        pImageData.Location = new Point(0, 0);
        pImageData.Margin = new Padding(4, 3, 4, 3);
        pImageData.Name = "pImageData";
        pImageData.Size = new Size(580, 579);
        pImageData.SizeMode = PictureBoxSizeMode.AutoSize;
        pImageData.TabIndex = 4;
        pImageData.TabStop = false;
        pImageData.Visible = false;
        // 
        // pImagePanel
        // 
        pImagePanel.Controls.Add(pImageData);
        pImagePanel.Location = new Point(528, 61);
        pImagePanel.Margin = new Padding(4, 3, 4, 3);
        pImagePanel.Name = "pImagePanel";
        pImagePanel.Size = new Size(580, 579);
        pImagePanel.TabIndex = 5;
        // 
        // webView
        // 
        webView.AllowExternalDrop = true;
        webView.CreationProperties = null;
        webView.DefaultBackgroundColor = Color.White;
        webView.Location = new Point(531, 61);
        webView.Margin = new Padding(4, 3, 4, 3);
        webView.Name = "webView";
        webView.Size = new Size(577, 579);
        webView.TabIndex = 20;
        webView.ZoomFactor = 1D;
        // 
        // pUpdateDataBtn
        // 
        pUpdateDataBtn.BackColor = Color.White;
        pUpdateDataBtn.FlatStyle = FlatStyle.Flat;
        pUpdateDataBtn.ForeColor = Color.Black;
        pUpdateDataBtn.Location = new Point(867, 30);
        pUpdateDataBtn.Margin = new Padding(4, 3, 4, 3);
        pUpdateDataBtn.Name = "pUpdateDataBtn";
        pUpdateDataBtn.Size = new Size(115, 29);
        pUpdateDataBtn.TabIndex = 6;
        pUpdateDataBtn.Text = "Save Changes";
        pUpdateDataBtn.UseVisualStyleBackColor = false;
        pUpdateDataBtn.Visible = false;
        pUpdateDataBtn.Click += OnSaveChanges;
        // 
        // pChangeImageBtn
        // 
        pChangeImageBtn.BackColor = Color.White;
        pChangeImageBtn.FlatStyle = FlatStyle.Flat;
        pChangeImageBtn.ForeColor = Color.Black;
        pChangeImageBtn.Location = new Point(867, 30);
        pChangeImageBtn.Margin = new Padding(4, 3, 4, 3);
        pChangeImageBtn.Name = "pChangeImageBtn";
        pChangeImageBtn.Size = new Size(115, 29);
        pChangeImageBtn.TabIndex = 7;
        pChangeImageBtn.Text = "Change Image";
        pChangeImageBtn.UseVisualStyleBackColor = false;
        pChangeImageBtn.Visible = false;
        pChangeImageBtn.Click += OnChangeImage;
        // 
        // pSaveWorkerThread
        // 
        pSaveWorkerThread.WorkerReportsProgress = true;
        pSaveWorkerThread.DoWork += OnSaveBegin;
        pSaveWorkerThread.ProgressChanged += OnSaveProgress;
        pSaveWorkerThread.RunWorkerCompleted += OnSaveComplete;
        // 
        // pEntryValue
        // 
        pEntryValue.BackColor = Color.FromArgb(  240,   240,   240);
        pEntryValue.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
        pEntryValue.Location = new Point(977, 0);
        pEntryValue.Margin = new Padding(4, 0, 4, 0);
        pEntryValue.Name = "pEntryValue";
        pEntryValue.Size = new Size(122, 24);
        pEntryValue.TabIndex = 8;
        pEntryValue.Text = "Empty";
        pEntryValue.TextAlign = ContentAlignment.MiddleRight;
        // 
        // extractWorkerThread
        // 
        extractWorkerThread.DoWork += extractWorkerThread_DoWork;
        extractWorkerThread.ProgressChanged += OnExtractProgress;
        extractWorkerThread.RunWorkerCompleted += OnExtractComplete;
        // 
        // MainWindow
        // 
        AllowDrop = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(  180,   180,   180);
        ClientSize = new Size(1112, 647);
        Controls.Add(webView);
        Controls.Add(pEntryValue);
        Controls.Add(pChangeImageBtn);
        Controls.Add(pUpdateDataBtn);
        Controls.Add(pImagePanel);
        Controls.Add(pEntryName);
        Controls.Add(pTreeView);
        Controls.Add(pMenuStrip);
        ForeColor = SystemColors.ControlDarkDark;
        Icon = (Icon) resources.GetObject("$this.Icon");
        Margin = new Padding(4, 3, 4, 3);
        Name = "MainWindow";
        Text = "Orion2 Repacker";
        FormClosing += OnWindowClosing;
        SizeChanged += OnChangeWindowSize;
        pMenuStrip.ResumeLayout(false);
        pMenuStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize) pImageData).EndInit();
        pImagePanel.ResumeLayout(false);
        pImagePanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize) webView).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.MenuStrip pMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem pFileMenuStripItem;
    private System.Windows.Forms.ToolStripMenuItem pOpenMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pSaveMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pReloadMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pUnloadMenuItem;
    private System.Windows.Forms.TreeView pTreeView;
    private System.Windows.Forms.TextBox pEntryName;
    private System.Windows.Forms.PictureBox pImageData;
    private System.Windows.Forms.Panel pImagePanel;
    private System.Drawing.Size pPrevSize;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem toolsToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem addToolStripMenuItem;
    private ToolStripMenuItem removeToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripMenuItem allNodesToolStripMenuItem;
    private ToolStripMenuItem expandToolStripMenuItem;
    private ToolStripMenuItem collapseToolStripMenuItem;
    private ToolStripMenuItem exportToolStripMenuItem;
    private ToolStripMenuItem searchToolStripMenuItem;
    private Button pUpdateDataBtn;
    private Button pChangeImageBtn;
    private System.ComponentModel.BackgroundWorker pSaveWorkerThread;
    private Label pEntryValue;
    private ToolStripMenuItem addFolderToolStripMenuItem;
    private ToolStripMenuItem createItemToolStripMenuItem;
    private System.ComponentModel.BackgroundWorker extractWorkerThread;
    private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    private ToolStripMenuItem editorSettingsToolStripMenuItem;
    private ToolStripMenuItem wordWrapToolStripMenuItem;
    private ToolStripMenuItem themeToolStripMenuItem;
    private ToolStripMenuItem lightToolStripMenuItem;
    private ToolStripMenuItem darkToolStripMenuItem;
    private ToolStripMenuItem testToolStripMenuItem;
    private ToolStripMenuItem lightToolStripTheme;
    private ToolStripMenuItem darkToolStripTheme;
}


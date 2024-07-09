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

using System.Diagnostics;
using Orion.Crypto.Stream;
using Orion.Window.Common;

namespace Orion.Window;
public partial class ProgressWindow : Form {
    private Stopwatch pStopWatch;
    private string sPath;

    public ProgressWindow(ITheme currentTheme) {
        InitializeComponent();

        BackColor = currentTheme.BackColor2;
        ForeColor = currentTheme.ForeColor2;
    }

    public string FileName { get; set; }
    public IPackStreamVerBase Stream { get; set; }
    public PackNode PackNode { get; set; }
    public long ElapsedTime { get; set; }

    public string Path {
        get => sPath;
        set {
            sPath = value;

            FileName = sPath[(sPath.LastIndexOf('/') + 1)..].Split('.')[0];
        }
    }

    public void UpdateProgressBar(int nProgress) {
        pProgressBar.Value = nProgress;
        if (PackNode is null) {
            pSaveInfo.Text = $"Saving {FileName}... {pProgressBar.Value}%";
        } else {
            pSaveInfo.Text = "Extracting...";
        }
    }

    public void Start() {
        pStopWatch = Stopwatch.StartNew();
    }

    public void Finish() {
        ElapsedTime = pStopWatch.ElapsedMilliseconds;
        pStopWatch.Stop();
    }

    public void SetProgressBarSize(int size) {
        pProgressBar.Maximum = size;
        pProgressBar.Step = 1;
        pProgressBar.Value = 0;
    }
}
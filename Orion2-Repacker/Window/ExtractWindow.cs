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
using Orion.Window.Common;

namespace Orion.Window;
public partial class ExtractWindow : Form {
    private Stopwatch pStopWatch;

    public ExtractWindow() {
        InitializeComponent();
    }

    public PackNode PackNode { get; set; }
    public long ElapsedTime { get; private set; }
    public string Path { get; set; }

    public void UpdateProgressBar(int nProgress) {
        pProgressBar.Value = nProgress;
        pSaveInfo.Text = "Extracting ...";
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
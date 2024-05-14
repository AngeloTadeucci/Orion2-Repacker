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

namespace Orion.Window; 
public partial class ProgressWindow : Form {
    private Stopwatch pStopWatch;
    private string sPath;

    public ProgressWindow() {
        InitializeComponent();
    }

    public string FileName { get; set; }
    public IPackStreamVerBase Stream { get; set; }
    public long ElapsedTime { get; set; }

    public string Path {
        get => sPath;
        set {
            sPath = value;

            FileName = sPath.Substring(sPath.LastIndexOf('/') + 1).Split('.')[0];
        }
    }

    public void UpdateProgressBar(int nProgress) {
        pProgressBar.Value = nProgress;
        pSaveInfo.Text = $"Saving {FileName} ... {pProgressBar.Value}%";
    }

    public void Start() {
        pStopWatch = Stopwatch.StartNew();
    }

    public void Finish() {
        ElapsedTime = pStopWatch.ElapsedMilliseconds;
        pStopWatch.Stop();
    }
}
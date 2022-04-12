﻿/*
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

using Orion.Window.Common;
using System.Diagnostics;
using System.Windows.Forms;

namespace Orion.Window
{
    public partial class ExtractWindow : Form
    {
        private string sPath;
        private Stopwatch pStopWatch;

        public string FileName { get; set; }
        public PackNode PackNode { get; set; }
        public long ElapsedTime { get; set; }

        public ExtractWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgressBar(int nProgress)
        {
            this.pProgressBar.Value = nProgress;
            this.pSaveInfo.Text = string.Format("Extracting ...");
        }

        public void Start()
        {
            this.pStopWatch = Stopwatch.StartNew();
        }

        public void Finish()
        {
            this.ElapsedTime = this.pStopWatch.ElapsedMilliseconds;
            this.pStopWatch.Stop();
        }

        public void SetProgressBarSize(int size)
        {
            pProgressBar.Maximum = size;
            pProgressBar.Step = 1;
            pProgressBar.Value = 0;
        }

        public string Path
        {
            get
            {
                return sPath;
            }
            set
            {
                this.sPath = value;

                this.FileName = this.sPath.Substring(this.sPath.LastIndexOf('/') + 1).Split('.')[0];
            }
        }
    }
}

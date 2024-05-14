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

using Orion.Crypto.Common;

namespace Orion.Window.Common; 
[Serializable]
public class PackNodeList {
    public const string DATA_FORMAT = "Pack.Node.FileList";

    public PackNodeList(string sDir) {
        Directory = sDir;
        Children = new Dictionary<string, PackNodeList>();
        Entries = new Dictionary<string, PackFileEntry>();
    }

    public Dictionary<string, PackNodeList> Children { get; }
    public Dictionary<string, PackFileEntry> Entries { get; }
    public string Directory { get; private set; }

    /*
     * Recursively clear all children/entries within this node list.
     * 
    */
    public void InternalRelease() {
        Entries.Clear();

        foreach (PackNodeList pChild in Children.Values) pChild.InternalRelease();
        Children.Clear();
    }
}
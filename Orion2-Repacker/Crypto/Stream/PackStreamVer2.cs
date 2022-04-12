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

using Orion.Crypto.Common;
using System.Collections.Generic;
using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackStreamVer2 : IPackStreamVerBase
    {
        private uint dwFileListCount;
        private ulong dwCompressedDataSize;
        private ulong dwEncodedDataSize;
        private ulong dwHeaderSize;
        private ulong dwCompressedHeaderSize;
        private ulong dwEncodedHeaderSize;
        private ulong dwDataSize;
        private readonly List<PackFileEntry> aFileList;

        private PackStreamVer2()
        {
            this.aFileList = new List<PackFileEntry>();
        }

        public static PackStreamVer2 ParseHeader(BinaryReader pReader)
        {
            return new PackStreamVer2
            {
                dwFileListCount = pReader.ReadUInt32(),
                dwCompressedDataSize = pReader.ReadUInt64(),
                dwEncodedDataSize = pReader.ReadUInt64(),
                dwHeaderSize = pReader.ReadUInt64(),
                dwCompressedHeaderSize = pReader.ReadUInt64(),
                dwEncodedHeaderSize = pReader.ReadUInt64(),
                dwDataSize = pReader.ReadUInt64()
            };
        }

        public void Encode(BinaryWriter pWriter)
        {
            pWriter.Write(this.dwFileListCount);
            pWriter.Write(this.dwCompressedDataSize);
            pWriter.Write(this.dwEncodedDataSize);
            pWriter.Write(this.dwHeaderSize);
            pWriter.Write(this.dwCompressedHeaderSize);
            pWriter.Write(this.dwEncodedHeaderSize);
            pWriter.Write(this.dwDataSize);
        }

        public uint GetVer()
        {
            return PackVer.NS2F;
        }

        public ulong GetCompressedHeaderSize()
        {
            return dwCompressedHeaderSize;
        }

        public ulong GetEncodedHeaderSize()
        {
            return dwEncodedHeaderSize;
        }

        public ulong GetHeaderSize()
        {
            return dwHeaderSize;
        }

        public ulong GetCompressedDataSize()
        {
            return dwCompressedDataSize;
        }

        public ulong GetEncodedDataSize()
        {
            return dwEncodedDataSize;
        }

        public ulong GetDataSize()
        {
            return dwDataSize;
        }

        public ulong GetFileListCount()
        {
            return dwFileListCount;
        }

        public List<PackFileEntry> GetFileList()
        {
            return aFileList;
        }

        public void SetCompressedHeaderSize(ulong uCompressed)
        {
            this.dwCompressedHeaderSize = uCompressed;
        }

        public void SetEncodedHeaderSize(ulong uEncoded)
        {
            this.dwEncodedHeaderSize = uEncoded;
        }

        public void SetHeaderSize(ulong uSize)
        {
            this.dwHeaderSize = uSize;
        }

        public void SetCompressedDataSize(ulong uCompressed)
        {
            this.dwCompressedDataSize = uCompressed;
        }

        public void SetEncodedDataSize(ulong uEncoded)
        {
            this.dwEncodedDataSize = uEncoded;
        }

        public void SetDataSize(ulong uSize)
        {
            this.dwDataSize = uSize;
        }

        public void SetFileListCount(ulong uCount)
        {
            this.dwFileListCount = (uint)uCount;
        }
    }
}

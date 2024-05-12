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

namespace Orion.Crypto.Stream {
    public class PackStreamVer1 : IPackStreamVerBase {
        private readonly List<PackFileEntry> aFileList;
        private ulong dwCompressedDataSize;
        private ulong dwCompressedHeaderSize;
        private ulong dwDataSize;
        private ulong dwEncodedDataSize;
        private ulong dwEncodedHeaderSize;
        private ulong dwFileListCount;
        private ulong dwHeaderSize;
        private uint uReserved;

        private PackStreamVer1() {
            aFileList = new List<PackFileEntry>();
        }

        public void Encode(BinaryWriter pWriter) {
            pWriter.Write(uReserved);
            pWriter.Write(dwCompressedDataSize);
            pWriter.Write(dwEncodedDataSize);
            pWriter.Write(dwHeaderSize);
            pWriter.Write(dwCompressedHeaderSize);
            pWriter.Write(dwEncodedHeaderSize);
            pWriter.Write(dwFileListCount);
            pWriter.Write(dwDataSize);
        }

        public uint GetVer() {
            return PackVer.MS2F;
        }

        public ulong GetCompressedHeaderSize() {
            return dwCompressedHeaderSize;
        }

        public ulong GetEncodedHeaderSize() {
            return dwEncodedHeaderSize;
        }

        public ulong GetHeaderSize() {
            return dwHeaderSize;
        }

        public ulong GetCompressedDataSize() {
            return dwCompressedDataSize;
        }

        public ulong GetEncodedDataSize() {
            return dwEncodedDataSize;
        }

        public ulong GetDataSize() {
            return dwDataSize;
        }

        public ulong GetFileListCount() {
            return dwFileListCount;
        }

        public List<PackFileEntry> GetFileList() {
            return aFileList;
        }

        public void SetCompressedHeaderSize(ulong uCompressed) {
            dwCompressedHeaderSize = uCompressed;
        }

        public void SetEncodedHeaderSize(ulong uEncoded) {
            dwEncodedHeaderSize = uEncoded;
        }

        public void SetHeaderSize(ulong uSize) {
            dwHeaderSize = uSize;
        }

        public void SetCompressedDataSize(ulong uCompressed) {
            dwCompressedDataSize = uCompressed;
        }

        public void SetEncodedDataSize(ulong uEncoded) {
            dwEncodedDataSize = uEncoded;
        }

        public void SetDataSize(ulong uSize) {
            dwDataSize = uSize;
        }

        public void SetFileListCount(ulong uCount) {
            dwFileListCount = uCount;
        }

        public static PackStreamVer1 ParseHeader(BinaryReader pReader) {
            return new PackStreamVer1 {
                uReserved = pReader.ReadUInt32(),
                dwCompressedDataSize = pReader.ReadUInt64(),
                dwEncodedDataSize = pReader.ReadUInt64(),
                dwHeaderSize = pReader.ReadUInt64(),
                dwCompressedHeaderSize = pReader.ReadUInt64(),
                dwEncodedHeaderSize = pReader.ReadUInt64(),
                dwFileListCount = pReader.ReadUInt64(),
                dwDataSize = pReader.ReadUInt64()
            };
        }
    }
}
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

namespace Orion.Crypto.Stream
{
    public class PackFileHeaderVer3 : IPackFileHeaderVerBase
    {
        private readonly uint uVer;
        private uint dwBufferFlag;
        private int nFileIndex;
        private readonly int[] Reserved;
        private ulong uCompressedFileSize;
        private uint uEncodedFileSize;
        private ulong uFileSize;
        private ulong uOffset;

        private PackFileHeaderVer3(uint uVer)
        {
            this.uVer = uVer;
            Reserved = new int[1];
        }

        public PackFileHeaderVer3(uint uVer, BinaryReader pReader)
            : this(uVer)
        {
            dwBufferFlag = pReader.ReadUInt32(); //[ecx+8]
            nFileIndex = pReader.ReadInt32(); //[ecx+12]
            uEncodedFileSize = pReader.ReadUInt32(); //[ecx+16]
            Reserved[0] = pReader.ReadInt32(); //[ecx+20]
            uCompressedFileSize = pReader.ReadUInt64(); //[ecx+24] | [ecx+28]
            uFileSize = pReader.ReadUInt64(); //[ecx+32] | [ecx+36]
            uOffset = pReader.ReadUInt64(); //[ecx+40] | [ecx+44]
        }

        public void Encode(BinaryWriter pWriter)
        {
            pWriter.Write(dwBufferFlag);
            pWriter.Write(nFileIndex);
            pWriter.Write(uEncodedFileSize);
            pWriter.Write(Reserved[0]);
            pWriter.Write(uCompressedFileSize);
            pWriter.Write(uFileSize);
            pWriter.Write(uOffset);
        }

        public uint GetVer()
        {
            return uVer;
        }

        public int GetFileIndex()
        {
            return nFileIndex;
        }

        public uint GetBufferFlag()
        {
            return dwBufferFlag;
        }

        public ulong GetOffset()
        {
            return uOffset;
        }

        public uint GetEncodedFileSize()
        {
            return uEncodedFileSize;
        }

        public ulong GetCompressedFileSize()
        {
            return uCompressedFileSize;
        }

        public ulong GetFileSize()
        {
            return uFileSize;
        }

        public void SetFileIndex(int nIndex)
        {
            nFileIndex = nIndex;
        }

        public void SetOffset(ulong uOffset)
        {
            this.uOffset = uOffset;
        }

        public void SetEncodedFileSize(uint uEncoded)
        {
            uEncodedFileSize = uEncoded;
        }

        public void SetCompressedFileSize(ulong uCompressed)
        {
            uCompressedFileSize = uCompressed;
        }

        public void SetFileSize(ulong uSize)
        {
            uFileSize = uSize;
        }

        public static PackFileHeaderVer3 CreateHeader(uint uVer, int nIndex, uint dwFlag, ulong uOffset, byte[] pData)
        {
            uint uLen, uCompressedLen, uEncodedLen;

            CryptoMan.Encrypt(uVer, pData, dwFlag, out uLen, out uCompressedLen, out uEncodedLen);

            return new PackFileHeaderVer3(uVer)
            {
                dwBufferFlag = dwFlag,
                nFileIndex = nIndex,
                uEncodedFileSize = uEncodedLen,
                uCompressedFileSize = uCompressedLen,
                uFileSize = uLen,
                uOffset = uOffset
            };
        }
    }
}
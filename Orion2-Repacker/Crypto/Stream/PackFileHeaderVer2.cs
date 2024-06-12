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

namespace Orion.Crypto.Stream;
public class PackFileHeaderVer2 : IPackFileHeaderVerBase {
    private uint dwBufferFlag;
    private int nFileIndex;
    private ulong uCompressedFileSize;
    private uint uEncodedFileSize;
    private ulong uFileSize;
    private ulong uOffset;

    private PackFileHeaderVer2() {
        // Interesting.. no reserved bytes stored in Ver2.
    }

    public PackFileHeaderVer2(BinaryReader pReader)
        : this() {
        dwBufferFlag = pReader.ReadUInt32(); //[ecx+8]
        nFileIndex = pReader.ReadInt32(); //[ecx+12]
        uEncodedFileSize = pReader.ReadUInt32(); //[ecx+16]
        uCompressedFileSize = pReader.ReadUInt64(); //[ecx+20] | [ecx+24]
        uFileSize = pReader.ReadUInt64(); //[ecx+28] | [ecx+32]
        uOffset = pReader.ReadUInt64(); //[ecx+36] | [ecx+40]
    }

    public void Encode(BinaryWriter pWriter) {
        pWriter.Write(dwBufferFlag);
        pWriter.Write(nFileIndex);
        pWriter.Write(uEncodedFileSize);
        pWriter.Write(uCompressedFileSize);
        pWriter.Write(uFileSize);
        pWriter.Write(uOffset);
    }

    public uint GetVer() {
        return PackVer.NS2F;
    }

    public int GetFileIndex() {
        return nFileIndex;
    }

    public uint GetBufferFlag() {
        return dwBufferFlag;
    }

    public ulong GetOffset() {
        return uOffset;
    }

    public uint GetEncodedFileSize() {
        return uEncodedFileSize;
    }

    public ulong GetCompressedFileSize() {
        return uCompressedFileSize;
    }

    public ulong GetFileSize() {
        return uFileSize;
    }

    public void SetFileIndex(int nIndex) {
        nFileIndex = nIndex;
    }

    public void SetOffset(ulong uOffset) {
        this.uOffset = uOffset;
    }

    public void SetEncodedFileSize(uint uEncoded) {
        uEncodedFileSize = uEncoded;
    }

    public void SetCompressedFileSize(ulong uCompressed) {
        uCompressedFileSize = uCompressed;
    }

    public void SetFileSize(ulong uSize) {
        uFileSize = uSize;
    }

    public static PackFileHeaderVer2 CreateHeader(int nIndex, uint dwFlag, ulong uOffset, byte[] pData) {
        uint uLen, uCompressedLen, uEncodedLen;

        CryptoMan.Encrypt(PackVer.NS2F, pData, dwFlag, out uLen, out uCompressedLen, out uEncodedLen);

        return new PackFileHeaderVer2 {
            dwBufferFlag = dwFlag,
            nFileIndex = nIndex,
            uEncodedFileSize = uEncodedLen,
            uCompressedFileSize = uCompressedLen,
            uFileSize = uLen,
            uOffset = uOffset
        };
    }
}
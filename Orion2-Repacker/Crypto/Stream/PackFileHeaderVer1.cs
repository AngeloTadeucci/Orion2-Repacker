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
public class PackFileHeaderVer1 : IPackFileHeaderVerBase {
    private readonly byte[] aPackingDef; //A "Packing Definition", unused.
    private readonly int[] Reserved;
    private uint dwBufferFlag;
    private int nFileIndex;
    private ulong uCompressedFileSize;
    private uint uEncodedFileSize;
    private ulong uFileSize;
    private ulong uOffset;

    private PackFileHeaderVer1() {
        aPackingDef = new byte[4];
        Reserved = new int[2];
    }

    public PackFileHeaderVer1(BinaryReader pReader)
        : this() {
        aPackingDef = pReader.ReadBytes(4); //[ecx+16]
        nFileIndex = pReader.ReadInt32(); //[ecx+20]
        dwBufferFlag = pReader.ReadUInt32(); //[ecx+24]
        Reserved[0] = pReader.ReadInt32(); //[ecx+28]
        uOffset = pReader.ReadUInt64(); //[ecx+32] | [ecx+36]
        uEncodedFileSize = pReader.ReadUInt32(); //[ecx+40]
        Reserved[1] = pReader.ReadInt32(); //[ecx+44]
        uCompressedFileSize = pReader.ReadUInt64(); //[ecx+48] | [ecx+52]
        uFileSize = pReader.ReadUInt64(); //[ecx+56] | [ecx+60]
    }

    public void Encode(BinaryWriter pWriter) {
        pWriter.Write(aPackingDef);
        pWriter.Write(nFileIndex);
        pWriter.Write(dwBufferFlag);
        pWriter.Write(Reserved[0]);
        pWriter.Write(uOffset);
        pWriter.Write(uEncodedFileSize);
        pWriter.Write(Reserved[1]);
        pWriter.Write(uCompressedFileSize);
        pWriter.Write(uFileSize);
    }

    public uint GetVer() {
        return PackVer.MS2F;
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

    public static PackFileHeaderVer1 CreateHeader(int nIndex, uint dwFlag, ulong uOffset, byte[] pData) {
        CryptoMan.Encrypt(PackVer.MS2F, pData, dwFlag, out uint uLen, out uint uCompressedLen, out uint uEncodedLen);

        return new PackFileHeaderVer1 {
            nFileIndex = nIndex,
            dwBufferFlag = dwFlag,
            uOffset = uOffset,
            uEncodedFileSize = uEncodedLen,
            uCompressedFileSize = uCompressedLen,
            uFileSize = uLen
        };
    }
}
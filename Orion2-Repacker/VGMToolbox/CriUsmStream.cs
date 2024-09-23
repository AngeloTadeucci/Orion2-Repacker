using VGMToolbox.util;

namespace Orion.VGMToolbox;

public class CriUsmStream : MpegStream {
    public const string DefaultAudioExtension = ".adx";
    public const string DefaultVideoExtension = ".m2v";
    public const string HcaAudioExtension = ".hca";

    static readonly byte[] HCA_SIG_BYTES = [0x48, 0x43, 0x41, 0x00];

    protected static readonly byte[] ALP_BYTES = [0x40, 0x41, 0x4C, 0x50];
    protected static readonly byte[] CRID_BYTES = [0x43, 0x52, 0x49, 0x44];
    protected static readonly byte[] SFV_BYTES = [0x40, 0x53, 0x46, 0x56];
    protected static readonly byte[] SFA_BYTES = [0x40, 0x53, 0x46, 0x41];
    protected static readonly byte[] SBT_BYTES = [0x40, 0x53, 0x42, 0x54];
    protected static readonly byte[] CUE_BYTES = [0x40, 0x43, 0x55, 0x45];

    protected static readonly byte[] UTF_BYTES = [0x40, 0x55, 0x54, 0x46];

    protected static readonly byte[] HEADER_END_BYTES = [
        0x23, 0x48, 0x45, 0x41, 0x44, 0x45, 0x52, 0x20,
        0x45, 0x4E, 0x44, 0x20, 0x20, 0x20, 0x20, 0x20,
        0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D,
        0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x00
    ];

    protected static readonly byte[] METADATA_END_BYTES = [
        0x23, 0x4D, 0x45, 0x54, 0x41, 0x44, 0x41, 0x54,
        0x41, 0x20, 0x45, 0x4E, 0x44, 0x20, 0x20, 0x20,
        0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D,
        0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x00
    ];

    protected static readonly byte[] CONTENTS_END_BYTES = [
        0x23, 0x43, 0x4F, 0x4E, 0x54, 0x45, 0x4E, 0x54,
        0x53, 0x20, 0x45, 0x4E, 0x44, 0x20, 0x20, 0x20,
        0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D,
        0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x00
    ];

    public CriUsmStream(string path) : base(path) {
        UsesSameIdForMultipleAudioTracks = true;
        FileExtensionAudio = DefaultAudioExtension;
        FileExtensionVideo = DefaultVideoExtension;

        BlockIdDictionary.Clear();
        BlockIdDictionary[BitConverter.ToUInt32(ALP_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @ALP
        BlockIdDictionary[BitConverter.ToUInt32(CRID_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // CRID
        BlockIdDictionary[BitConverter.ToUInt32(SFV_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @SFV
        BlockIdDictionary[BitConverter.ToUInt32(SFA_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @SFA
        BlockIdDictionary[BitConverter.ToUInt32(SBT_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @SBT
        BlockIdDictionary[BitConverter.ToUInt32(CUE_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @CUE
    }

    protected override byte[] GetPacketStartBytes() {
        return CRID_BYTES;
    }

    protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset) {
        OffsetDescription od = new OffsetDescription {
            OffsetByteOrder = Constants.BigEndianByteOrder,
            OffsetSize = "2",
            OffsetValue = "8"
        };

        ushort checkBytes = (ushort) ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

        return checkBytes;
    }

    protected override int GetVideoPacketHeaderSize(Stream readStream, long currentOffset) {
        OffsetDescription od = new OffsetDescription {
            OffsetByteOrder = Constants.BigEndianByteOrder,
            OffsetSize = "2",
            OffsetValue = "8"
        };

        var checkBytes = (ushort) ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

        return checkBytes;
    }

    protected override bool IsThisAnAudioBlock(byte[] blockToCheck) {
        return ParseFile.CompareSegment(blockToCheck, 0, SFA_BYTES);
    }

    protected override bool IsThisAVideoBlock(byte[] blockToCheck) {
        return ParseFile.CompareSegment(blockToCheck, 0, SFV_BYTES);
    }

    protected override byte GetStreamId(Stream readStream, long currentOffset) {
        var streamId = ParseFile.ParseSimpleOffset(readStream, currentOffset + 0xC, 1)[0];

        return streamId;
    }

    protected override int GetAudioPacketFooterSize(Stream readStream, long currentOffset) {
        OffsetDescription od = new OffsetDescription {
            OffsetByteOrder = Constants.BigEndianByteOrder,
            OffsetSize = "2",
            OffsetValue = "0xA"
        };

        var checkBytes = (ushort) ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

        return checkBytes;
    }

    protected override int GetVideoPacketFooterSize(Stream readStream, long currentOffset) {
        OffsetDescription od = new OffsetDescription {
            OffsetByteOrder = Constants.BigEndianByteOrder,
            OffsetSize = "2",
            OffsetValue = "0xA"
        };

        var checkBytes = (ushort) ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

        return checkBytes;
    }

    protected override void DoFinalTasks(FileStream sourceFileStream, Dictionary<uint, FileStream> outputFiles,
        bool addHeader) {
        foreach (uint streamId in outputFiles.Keys) {
            var sourceFileName = outputFiles[streamId].Name;

            //--------------------------
            // get header size
            //--------------------------
            var headerEndOffset = ParseFile.GetNextOffset(outputFiles[streamId], 0, HEADER_END_BYTES);
            var metadataEndOffset = ParseFile.GetNextOffset(outputFiles[streamId], 0, METADATA_END_BYTES);

            long headerSize;
            if (metadataEndOffset > headerEndOffset) {
                headerSize = metadataEndOffset + METADATA_END_BYTES.Length;
            } else {
                headerSize = headerEndOffset + METADATA_END_BYTES.Length;
            }

            //-----------------
            // get footer size
            //-----------------
            var footerOffset = ParseFile.GetNextOffset(outputFiles[streamId], 0, CONTENTS_END_BYTES) - headerSize;
            var footerSize = outputFiles[streamId].Length - footerOffset;

            //------------------------------------------
            // check data to adjust extension if needed
            //------------------------------------------
            string fileExtension;
            // may need to change mask if more than 0xF streams
            if (IsThisAnAudioBlock(BitConverter.GetBytes(streamId & 0xFFFFFFF0))) {
                byte[] checkBytes = ParseFile.ParseSimpleOffset(outputFiles[streamId], headerSize, 4);

                if (ParseFile.CompareSegment(checkBytes, 0, SofdecStream.AixSignatureBytes)) {
                    fileExtension = SofdecStream.AixAudioExtension;
                } else if (checkBytes[0] == 0x80) {
                    fileExtension = SofdecStream.AdxAudioExtension;
                } else if (ParseFile.CompareSegment(checkBytes, 0, HCA_SIG_BYTES)) {
                    fileExtension = HcaAudioExtension;
                } else {
                    fileExtension = ".bin";
                }

                FinalAudioExtension = fileExtension;
                HasAudio = true;
            } else {
                fileExtension = Path.GetExtension(sourceFileName);
            }

            outputFiles[streamId].Close();
            outputFiles[streamId].Dispose();

            var workingFile = FileUtil.RemoveChunkFromFile(sourceFileName, 0, headerSize);
            File.Copy(workingFile, sourceFileName, true);
            File.Delete(workingFile);

            workingFile = FileUtil.RemoveChunkFromFile(sourceFileName, footerOffset, footerSize);
            var destinationFileName = Path.ChangeExtension(sourceFileName, fileExtension);
            destinationFileName = string.Concat(destinationFileName.AsSpan(0, destinationFileName.LastIndexOf('_')),
                fileExtension);
            File.Copy(workingFile, destinationFileName, true);
            File.Delete(workingFile);

            if ((sourceFileName != destinationFileName) && (File.Exists(sourceFileName))) {
                File.Delete(sourceFileName);
            }
        }
    }
}
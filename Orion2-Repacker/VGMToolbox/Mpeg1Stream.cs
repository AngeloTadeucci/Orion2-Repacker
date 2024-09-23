namespace Orion.VGMToolbox;

public class Mpeg1Stream : MpegStream {
    private const string DefaultAudioExtension = ".mp2";
    private const string DefaultVideoExtension = ".m1v";

    public Mpeg1Stream(string path)
        : base(path) {
        FileExtensionAudio = DefaultAudioExtension;
        FileExtensionVideo = DefaultVideoExtension;

        BlockIdDictionary[BitConverter.ToUInt32(PacketStartBytes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xC); // Pack Header
    }

    protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset) {
        int paddingByteCount = 0;
        readStream.Position = currentOffset + 6;

        // skip stuffing bytes
        while (readStream.ReadByte() == 0xFF) {
            paddingByteCount++;
        }

        return paddingByteCount + 7;
    }

    protected override int GetVideoPacketHeaderSize(Stream readStream, long currentOffset) {
        return 0xC;
    }
}
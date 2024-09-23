using VGMToolbox.util;

namespace Orion.VGMToolbox;

public class SofdecStream : Mpeg1Stream {
    private const string DefaultVideoExtension = ".m2v";

    public const string AdxAudioExtension = ".adx";
    public const string AixAudioExtension = ".aix";
    public const string Ac3AudioExtension = ".ac3";

    public static readonly byte[] AixSignatureBytes = [0x41, 0x49, 0x58, 0x46];
    public static readonly byte[] Ac3SignatureBytes = [0x0B, 0x77];

    public SofdecStream(string path) : base(path) {
        FileExtensionAudio = AdxAudioExtension;
        FileExtensionVideo = DefaultVideoExtension;
    }

    protected override string GetAudioFileExtension(Stream readStream, long currentOffset) {
        string fileExtension;

        int headerSize = GetAudioPacketHeaderSize(readStream, currentOffset);
        var checkBytes = ParseFile.ParseSimpleOffset(readStream, (currentOffset + 6 + headerSize), 4);

        if (ParseFile.CompareSegment(checkBytes, 0, AixSignatureBytes)) {
            fileExtension = AixAudioExtension;
        } else if (checkBytes[0] == 0x80) {
            fileExtension = AdxAudioExtension;
        } else {
            var checkBytesAc3 = ParseFile.ParseSimpleOffset(readStream, (currentOffset + 6 + headerSize), 2);

            if (ParseFile.CompareSegment(checkBytesAc3, 0, Ac3SignatureBytes)) {
                fileExtension = Ac3AudioExtension;
            } else {
                fileExtension = ".bin";
            }
        }

        return fileExtension;
    }
}
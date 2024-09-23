using Newtonsoft.Json;
using Orion.VGMToolbox;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Orion.Window.Common;
public static class Helpers {
    public static string CreateHash(string sHeaderUOL) {
        if (!File.Exists(sHeaderUOL)) return "";

        using (MD5 md5 = MD5.Create()) {
            using (FileStream stream = File.OpenRead(sHeaderUOL)) {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public static void ExecuteProcess(string fileName, string arguments) {
        ProcessStartInfo startInfo = new ProcessStartInfo() {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        Process process = new Process {
            StartInfo = startInfo
        };

        process.Start();
        process.WaitForExit();
    }

    public static string CreateFFmpegParameters(CriUsmStream usmStream, string pureFileName, string outputDir) {
        StringBuilder sb = new StringBuilder();
        sb.Append($"-i \"{Path.ChangeExtension(usmStream.FilePath, usmStream.FileExtensionVideo)}\" ");

        if (usmStream.HasAudio)
            sb.Append($"-i \"{Path.ChangeExtension(usmStream.FilePath, usmStream.FinalAudioExtension)}\" ");

        sb.Append($"-c:v copy ");

        if (usmStream.HasAudio)
            sb.Append($"-c:a ac3 -b:a 640k -af pan='stereo|FL=FL+FC+0.5*BL+BR|FR=FR+LFE+0.5*BL+BR' ");

        sb.Append($"\"{Path.Combine(outputDir ?? string.Empty, $"{pureFileName}.mp4")}\"");

        return sb.ToString();
    }

    public static bool ConvertUSMToMP4(string inputFile) {
        string outputDir = GetAppDataFolder();
        var usmStream = new CriUsmStream(inputFile);

        usmStream.DemultiplexStreams(new MpegStream.DemuxOptionsStruct() {
            AddHeader = false,
            AddPlaybackHacks = false,
            ExtractAudio = true,
            ExtractVideo = true,
            SplitAudioStreams = false
        });


        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir)) {
            Directory.CreateDirectory(outputDir);
        }

        var audioFormat = usmStream.FinalAudioExtension;
        var pureFileName = Path.GetFileNameWithoutExtension(usmStream.FilePath);

        if (audioFormat == ".adx") {
            //ffmpeg can not handle .adx from 0.2 for whatever reason
            //need vgmstream to format that to wav
            if (!Directory.Exists("vgmstream")) {
                return false;
            }

            ExecuteProcess("vgmstream/vgmstream-cli.exe", $"\"{Path.ChangeExtension(usmStream.FilePath, usmStream.FinalAudioExtension)}\" -o \"{Path.ChangeExtension(usmStream.FilePath, "wav")}\"");
            usmStream.FinalAudioExtension = ".wav";
        }

        ExecuteProcess("ffmpeg", CreateFFmpegParameters(usmStream, pureFileName, outputDir));

        File.Delete(Path.ChangeExtension(usmStream.FilePath, "wav"));
        File.Delete(Path.ChangeExtension(usmStream.FilePath, "adx"));
        File.Delete(Path.ChangeExtension(usmStream.FilePath, "hca"));
        File.Delete(Path.ChangeExtension(usmStream.FilePath, "m2v"));
        return true;
    }

    public static bool CheckIfmpegInstalled() {
        if (Properties.Settings.Default.FfmpegInstalled) {
            return true;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo() {
            FileName = "ffmpeg",
            Arguments = "-version",
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        Process process = new Process {
            StartInfo = startInfo
        };

        try {
            process.Start();
            process.WaitForExit();

            bool success = process.ExitCode == 0;
            Properties.Settings.Default.FfmpegInstalled = true;
            Properties.Settings.Default.Save();
            return success;
        } catch {
            return false;
        }
    }

    public static string GetAppDataFolder() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Orion2-Repacker", "UsmConversion");
}
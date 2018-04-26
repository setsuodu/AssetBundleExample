using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Threading;

/// <summary>
/// 添加递归方式压缩/解压缩文件夹
/// important: 
/// 1.文件/文件夹不支持中文
/// 2.会阻塞主线程
/// </summary>
public class MyZip
{
    /// <summary>
    /// 进度刷新时间
    /// </summary>
    public float progressUpdateTime = 0.2f;
    /// <summary>
    /// 每个文件的压缩/解压进度
    /// </summary>
    public float progress { private set; get; }
    /// <summary>
    /// 文件的压缩/解压总进度
    /// </summary>
    public float progressOverall { private set; get; }
    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="_fileForlder">文件夹路径</param>
    /// <param name="_outZip">zip文件路径+名字</param>
    public void ZipFolder(string _fileFolder, string _outZip)
    {
        string directory = _outZip.Substring(0, _outZip.LastIndexOf('/'));
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(_outZip))
        {
            File.Delete(_outZip);
        }

        progress = progressOverall = 0;

        Thread thread = new Thread(delegate ()
        {
            int fileCount = FileCount(_fileFolder);
            int fileCompleted = 0;

            FastZipEvents events = new FastZipEvents();
            events.Progress = new ProgressHandler((object sender, ProgressEventArgs e) =>
            {
                progress = e.PercentComplete;
                if (progress == 100)
                {
                    fileCompleted ++;
                    progressOverall = 100 * fileCompleted / fileCount;
                }
            });

            events.ProgressInterval = TimeSpan.FromSeconds(progressUpdateTime);
            events.ProcessFile = new ProcessFileHandler(
                (object sender, ScanEventArgs e) => { });
            FastZip fastZip = new FastZip(events);
            fastZip.CreateZip(_outZip, _fileFolder, true, "");
        });

        thread.IsBackground = true;
        thread.Start();
    }

    /// <summary>
    /// 解压zip文件
    /// </summary>
    /// <param name="_zipFile">需要解压的zip路径+名字</param>
    /// <param name="_outForlder">解压路径</param>
    public void UnZipFile(string _zipFile, string _outForlder)
    {
        if (!Directory.Exists(_outForlder))
        {
            Directory.CreateDirectory(_outForlder);
        }

        progress = progressOverall = 0;
        Thread thread = new Thread(delegate ()
        {
            int fileCount = (int)new ZipFile(_zipFile).Count;
            int fileCompleted = 0;
            FastZipEvents events = new FastZipEvents();
            events.Progress = new ProgressHandler((object sender, ProgressEventArgs e) =>
            {
                progress = e.PercentComplete;
                if (progress == 100)
                {
                    fileCompleted++;
                    progressOverall = 100 * fileCompleted / fileCount;
                }
            });
            events.ProgressInterval = TimeSpan.FromSeconds(progressUpdateTime);
            events.ProcessFile = new ProcessFileHandler(
                (object sender, ScanEventArgs e) => { });
            FastZip fastZip = new FastZip(events);
            fastZip.ExtractZip(_zipFile, _outForlder, "");
        });
        thread.IsBackground = true;
        thread.Start();
    }

    int FileCount(string path)
    {
        int result = Directory.GetFiles(path).Length;
        string[] subFolders = Directory.GetDirectories(path);
        foreach (string subFolder in subFolders)
        {
            result += FileCount(subFolder);
        }
        return result;
    }
}

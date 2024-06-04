using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger
{
    private readonly static HashSet<string> accessingFilePaths = new();

    private readonly string filePath;
    private Queue<string> writeBuffer = new();
    private bool disposed = false;

    /// <summary>
    /// DataLoggerを生成する。同時に対応するファイルを作成する。同じfilepathに対して、1つしかインスタンスを生成できないことを保証する。
    /// </summary>
    /// <param name="fileName">ログを保存するファイルのファイル名</param>
    /// <param name="subdirectoryHierarchy">ユーザーが指定するサブディレクトリの階層のリスト。システムが指定したファイル保存ディレクトリの配下にサブディレクトリを生成して、そのサブディレクトリにログのファイルを保存したいときに指定する。</param>
    public DataLogger(string fileName, params string[] subdirectoryHierarchy)
    {
        string combinedSubdirectoryPath = CombineSubdirectoryHierarchy(subdirectoryHierarchy);
        string directoryPath = Path.Combine(Application.dataPath, "Data", combinedSubdirectoryPath);
        
        filePath = Path.Combine(directoryPath, fileName);

        if (accessingFilePaths.Contains(filePath))
        {
            Debug.LogWarning("同名で複数のファイルを作成することはできません。先頭にタイムスタンプを付加した別名のファイルを作成しました。");
            DateTime timestamp = DateTime.Now;
            filePath = Path.Combine(directoryPath, timestamp.ToString("yyyy-MM-dd-HH-mm-ss-ff") + "_" + fileName);
        }

        accessingFilePaths.Add(filePath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
    }


    private string CombineSubdirectoryHierarchy(string[] relativeSubdirectoryHierarchy)
    {
        string combinedDirectoryName = "";
        for (int i = 0; i < relativeSubdirectoryHierarchy.Length; i++)
        {
            combinedDirectoryName = Path.Combine(combinedDirectoryName, relativeSubdirectoryHierarchy[i]);
        }
        return combinedDirectoryName;
    }

    /// <summary>
    /// write bufferにテキストを追加する。
    /// </summary>
    /// <param name="text"></param>
    public void AppendLine(string text)
    {
        writeBuffer.Enqueue(text);
    }

    /// <summary>
    /// write bufferのテキストをファイルに書き込む。
    /// </summary>
    public void Flush()
    {
        if (writeBuffer.Count != 0)
        {
            File.AppendAllLines(filePath, writeBuffer);

            writeBuffer = new();
        }
    }

    void OnApplicationQuit()
    {
        Flush();
    }
}

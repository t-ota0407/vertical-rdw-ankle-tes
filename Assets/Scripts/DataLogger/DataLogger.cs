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
    /// DataLogger�𐶐�����B�����ɑΉ�����t�@�C�����쐬����B����filepath�ɑ΂��āA1�����C���X�^���X�𐶐��ł��Ȃ����Ƃ�ۏ؂���B
    /// </summary>
    /// <param name="fileName">���O��ۑ�����t�@�C���̃t�@�C����</param>
    /// <param name="subdirectoryHierarchy">���[�U�[���w�肷��T�u�f�B���N�g���̊K�w�̃��X�g�B�V�X�e�����w�肵���t�@�C���ۑ��f�B���N�g���̔z���ɃT�u�f�B���N�g���𐶐����āA���̃T�u�f�B���N�g���Ƀ��O�̃t�@�C����ۑ��������Ƃ��Ɏw�肷��B</param>
    public DataLogger(string fileName, params string[] subdirectoryHierarchy)
    {
        string combinedSubdirectoryPath = CombineSubdirectoryHierarchy(subdirectoryHierarchy);
        string directoryPath = Path.Combine(Application.dataPath, "Data", combinedSubdirectoryPath);
        
        filePath = Path.Combine(directoryPath, fileName);

        if (accessingFilePaths.Contains(filePath))
        {
            Debug.LogWarning("�����ŕ����̃t�@�C�����쐬���邱�Ƃ͂ł��܂���B�擪�Ƀ^�C���X�^���v��t�������ʖ��̃t�@�C�����쐬���܂����B");
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
    /// write buffer�Ƀe�L�X�g��ǉ�����B
    /// </summary>
    /// <param name="text"></param>
    public void AppendLine(string text)
    {
        writeBuffer.Enqueue(text);
    }

    /// <summary>
    /// write buffer�̃e�L�X�g���t�@�C���ɏ������ށB
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

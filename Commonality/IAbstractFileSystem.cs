using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Commonality
{
    public interface IAbstractFileSystem
    {
        string[] Directory_GetFiles(string path);
        Stream File_OpenRead(string path);
        Stream File_Create(string path);
        void Directory_CreateDirectory(string path);
        StreamWriter File_AppendText(string path);
        string Path_GetDirectoryName(string path);
        string Path_GetFileName(string path);
    }
}

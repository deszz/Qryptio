using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Qryptio.Core.Exchange;
using Qryptio.Properties;

namespace Qryptio.Pages
{
    public enum FileType
    {
        Executable,
        Picture,
        QrFile,
        Plain
    }

    public class FileTypeInfo
    {
        public readonly string Description;
        public readonly Bitmap Icon;

        public FileTypeInfo(string desc, Bitmap icon)
        {
            Description = desc;
            Icon = icon;
        }
    }

    public static class FileTypeExtenstion
    {
        private static Dictionary<FileType, FileTypeInfo> types;

        // TODO: Add document (text, pdf, etc.) and video types
        static FileTypeExtenstion()
        {
            types = new Dictionary<FileType, FileTypeInfo>();

            types.Add(FileType.QrFile, new FileTypeInfo(Resources.FileDescription_Encrypted, Resources.file_encrypted));
            types.Add(FileType.Plain, new FileTypeInfo(Resources.FileDescription_Plain, Resources.file_plain));
            types.Add(FileType.Executable, new FileTypeInfo(Resources.FileDescription_Executable, Resources.file_executable));
            types.Add(FileType.Picture, new FileTypeInfo(Resources.FileDescription_Picture, Resources.file_pictures));
        }

        public static FileType GetFileType(this FileInfo fileInfo)
        {
            return GetFileType(fileInfo.Extension);
        }

        public static FileType GetFileType(string extenstion)
        {
            switch (extenstion.ToLower())
            {
                case QrFile.Extention:
                    return FileType.QrFile;
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".ico":
                case ".icon":
                case ".bmp":
                case ".svg":
                    return FileType.Picture;
                case ".exe":
                case ".dll":
                    return FileType.Executable;
                default:
                    return FileType.Plain;
            }
        }

        public static FileTypeInfo GetFileTypeInfo(this FileType fileType)
        {
            if (Enum.IsDefined(typeof(FileType), fileType))
                return types[fileType];

            return null;
        }
    }
}

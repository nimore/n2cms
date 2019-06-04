using System;
using System.Collections.Generic;

namespace N2.Edit.FileSystem
{
    public static class FileSystemExtensions
    {
        public static DirectoryData GetDirectoryOrVirtual(this IFileSystem fs, string virtualDir)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("GetDirectoryOrVirtual: {0}", virtualDir));

            return fs.GetDirectory(virtualDir)
                ?? DirectoryData.Virtual(virtualDir);
        }

        public static IEnumerable<FileData> GetFilesRecursive(this IFileSystem fs, string ancestorDir)
        {
            foreach (var file in fs.GetFiles(ancestorDir))
                yield return file;
            foreach (var dir in fs.GetDirectories(ancestorDir))
                foreach (var file in GetFilesRecursive(fs, dir.VirtualPath))
                    yield return file;
        }
    }
}

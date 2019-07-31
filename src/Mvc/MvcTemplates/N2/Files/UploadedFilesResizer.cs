using System;
using System.IO;
using System.Linq;
using System.Web;
using N2.Configuration;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Plugin;
using N2.Web;
using N2.Web.Drawing;

namespace N2.Management.Files
{
    [Service]
    public class UploadedFilesResizer : IAutoStart
    {
        private IFileSystem files;
        private ImagesElement images;
        private ImageResizer resizer;
        private string[] sizeNames;

        public UploadedFilesResizer(IFileSystem files, ImageResizer resizer, EditSection config)
        {
            this.files = files;
            this.resizer = resizer;
            this.images = config.Images;
            this.Enabled = config.Images.ResizeUploadedImages;
            sizeNames = config.Images.Sizes.AllElements.Select(s => s.Name).ToArray();
        }

        public bool Enabled { get; set; }

        public virtual void CreateSize(Url virtualPath, byte[] image, ImageSizeElement size)
        {
            if (!size.ResizeOnUpload)
                return;

            string resizedPath = ImagesUtility.GetResizedPath(virtualPath, size.Name);

            using (var sourceStream = new MemoryStream(image))
            {
                if (size.Width <= 0 && size.Height <= 0)
                {
                    files.WriteFile(resizedPath, sourceStream);
                }
                else
                {
                    // Delete the image before writing.
                    // Fixes a weird bug where overwriting the original file while it still exists
                    //  leaves the resized image the with the exact same file size as the original even
                    //  though it should be smaller.
                    if (files.FileExists(resizedPath))
                    {
                        files.DeleteFile(resizedPath);
                    }

                    try
                    {
                        using (var destinationStream = files.OpenFile(resizedPath))
                        {
                            resizer.Resize(sourceStream, new ImageResizeParameters(size.Width, size.Height, size.Mode) { Quality = size.Quality }, destinationStream);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public virtual void CreateSize(Url virtualPath, Stream sourceStream, ImageSizeElement size)
        {
            if (!size.ResizeOnUpload)
                return;

            string resizedPath = ImagesUtility.GetResizedPath(virtualPath, size.Name);
            
            if (size.Width <= 0 && size.Height <= 0)
            {
                files.WriteFile(resizedPath, sourceStream);
            }
            else
            {
                // Delete the image before writing.
                // Fixes a weird bug where overwriting the original file while it still exists
                //  leaves the resized image the with the exact same file size as the original even
                //  though it should be smaller.
                if (files.FileExists(resizedPath))
                {
                    files.DeleteFile(resizedPath);
                }

                try
                {
                    using (var destinationStream = files.OpenFile(resizedPath))
                    {
                        resizer.Resize(sourceStream, new ImageResizeParameters(size.Width, size.Height, size.Mode) { Quality = size.Quality }, destinationStream);
                    }
                }
                catch
                {
                }
            }    
        }        

        private byte[] ReadToEnd(Stream stream)
        {
            byte[] image = new byte[stream.Length];
            int numBytesRead = 0;
            long numBytesToRead = stream.Length;
            do
            {
                int n = stream.Read(image, numBytesRead, (int)Math.Min(4 * 1024 * 1024, numBytesToRead)); // max 4MB read for Azure compatability
                numBytesRead += n;
                numBytesToRead -= n;
            }
            while (numBytesToRead > 0);

            return image;
        }

        public virtual byte[] GetImageBytes(string virtualPath)
        {
            byte[] image;
            using (var s = files.OpenFile(virtualPath, readOnly: true))
            {
                image = ReadToEnd(s);
            }
            return image;
        }

        public bool IsResizableImagePath(string imageUrl)
        {
            string fileExtension = VirtualPathUtility.GetExtension(Url.PathPart(imageUrl));
            return images.ResizedExtensions.Contains(fileExtension.ToLower());
        }

        private void files_FileCopied(object sender, FileEventArgs e)
        {
            if (!Enabled)
                return;

            if (IsResizedPath(e.VirtualPath))
                return;

            foreach (ImageSizeElement size in images.Sizes.AllElements)
            {
                Url sourceUrl = e.SourcePath;
                Url destinationUrl = e.VirtualPath;

                string sourcePath = ImagesUtility.GetResizedPath(sourceUrl, size.Name);

                if (!files.FileExists(sourcePath))
                    continue;

                string destinationPath = ImagesUtility.GetResizedPath(destinationUrl, size.Name);
                if (!files.FileExists(destinationPath))
                {
                    files.CopyFile(sourcePath, destinationPath);
                }
            }
        }

        private void files_FileDeleted(object sender, FileEventArgs e)
        {
            if (!Enabled)
                return;

            if (!IsResizableImagePath(e.VirtualPath))
                return;

            foreach (ImageSizeElement size in images.Sizes.AllElements)
            {
                string resizedPath = ImagesUtility.GetResizedPath(e.VirtualPath, size.Name);

                if (files.FileExists(resizedPath))
                {
                    files.DeleteFile(resizedPath);
                }
            }
        }

        private void files_FileMoved(object sender, FileEventArgs e)
        {
            if (!Enabled)
                return;

            if (!IsResizableImagePath(e.VirtualPath))
                return;

            foreach (ImageSizeElement size in images.Sizes.AllElements)
            {
                string source = ImagesUtility.GetResizedPath(e.SourcePath, size.Name);
                if (files.FileExists(source))
                {
                    string destination = ImagesUtility.GetResizedPath(e.VirtualPath, size.Name);
                    if (!files.FileExists(destination))
                    {
                        files.MoveFile(source, destination);
                    }
                }
            }
        }

        private void files_FileWritten(object sender, FileEventArgs e)
        {
            if (!Enabled)
                return;

            Url virtualPath = e.VirtualPath;

            if (!IsResizableImagePath(virtualPath))
                return;

            if (images.Sizes.Count == 0)
                return;

            ////byte[] image = GetImageBytes(virtualPath);

            ////foreach (ImageSizeElement size in images.Sizes.AllElements)
            ////{
            ////    CreateSize(virtualPath, image, size);
            ////}

            using (var s = files.OpenFile(virtualPath, readOnly: true))
            {
                if (s.CanSeek)
                {
                    foreach (ImageSizeElement size in images.Sizes.AllElements)
                    {
                        s.Position = 0;
                        CreateSize(virtualPath, s, size);
                    }
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        s.CopyTo(ms);
                        foreach (ImageSizeElement size in images.Sizes.AllElements)
                        {
                            ms.Position = 0;
                            CreateSize(virtualPath, ms, size);
                        }
                    }
                }
            }
        }

        private bool IsResizedPath(string path)
        {
            string extensionlessPath = Url.RemoveAnyExtension(path);
            foreach (var sizeName in sizeNames)
            {
                if (extensionlessPath.EndsWith("_" + sizeName))
                    return true;
            }
            return false;
        }

        #region IAutoStart Members

        public void Start()
        {
            if (!images.ResizeUploadedImages)
                return;

            files.FileWritten += files_FileWritten;
            files.FileMoved += files_FileMoved;
            files.FileDeleted += files_FileDeleted;
            files.FileCopied += files_FileCopied;
        }

        public void Stop()
        {
            if (!images.ResizeUploadedImages)
                return;

            files.FileWritten -= files_FileWritten;
            files.FileMoved -= files_FileMoved;
            files.FileDeleted -= files_FileDeleted;
            files.FileCopied -= files_FileCopied;
        }

        #endregion IAutoStart Members
    }
}

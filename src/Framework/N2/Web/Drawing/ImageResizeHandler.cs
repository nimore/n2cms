using System;
using System.IO;
using System.Web;
using N2.Edit.FileSystem;

namespace N2.Web.Drawing
{
    /// <summary>
    /// Resizes the requested image to the requested size.
    /// </summary>
    public class ImageResizeHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //UrlDecode is neccessary to compensate encoding in ImageAdapter
            string imageUrl = HttpUtility.UrlDecode(context.Request["img"]);
            string w = context.Request["w"];
            string h = context.Request["h"];
            string m = context.Request["m"];

            double width = 0;
            double.TryParse(w, out width);

            double height = 0;
            double.TryParse(h, out height);

            ImageResizeMode mode;
            switch (m)
            {
                case "FitCenterOnTransparent":
                    mode = ImageResizeMode.FitCenterOnTransparent;
                    break;
                case "Fill":
                    mode = ImageResizeMode.Fill;
                    break;
                case "Stretch":
                    mode = ImageResizeMode.Stretch;
                    break;
                default:
                    mode = ImageResizeMode.Fit;
                    break;
            }

            if (ImagesUtility.IsImagePath(imageUrl) == true)
            {
                IFileSystem fs = N2.Context.Current.Resolve<IFileSystem>();
                if (fs.FileExists(imageUrl))
                {					
                    //string path = context.Server.MapPath(imageUrl);
					if (CacheUtility.IsUnmodifiedSince(context.Request, fs.GetFile(imageUrl).Updated))
                    {
                        CacheUtility.NotModified(context.Response);
                    }

                    context.Response.ContentType = GetContentType(imageUrl);

                    string extension = VirtualPathUtility.GetExtension(imageUrl);
                    ImageResizer ir = N2.Context.Current.Resolve<ImageResizer>();

                    CacheUtility.SetValidUntilExpires(context.Response, TimeSpan.FromDays(7));
                    using (var s = fs.OpenFile(imageUrl, readOnly: true))
                    {
                        var resized = ir.GetResizedBytes(s, new ImageResizeParameters(width, height, mode));
                        context.Response.BinaryWrite(resized);
                    }
                }
                else
                {
                    throw new HttpException(404, "Not found");
                }
            }            
        }

        private static string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();
            switch (extension)
            {
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";				
                default:
                    return "application/x-" + extension.TrimStart('.');
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}

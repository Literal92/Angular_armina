using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
namespace shop.Services.Handler
{
    public static class ImgHandler
    {
        public static List<(string base64File, string filename, bool ValidSize)> GetFilename(List<string> Base64Images, List<string> ValidExtention, int? ChkSize)
        {
            try
            {
                var output = new List<(string base64File, string filename, bool validSize)>();
                foreach (var item in Base64Images)
                {
                    var index = item.IndexOf(",");
                    var base64 = item.Substring(index + 1);
                    byte[] bytes = Convert.FromBase64String(base64);
                    var size = bytes.Count();
                    if (ChkSize != null && size > ChkSize)
                    {
                        (string, string, bool) tuple = (null, null, false);
                        output.Add(tuple);
                        return output;
                    }
                    if (bytes.Any())
                    {
                        var ext = GetFileExtension(base64);
                        var valid = true;
                        if (ValidExtention.Any())
                        {
                            valid = ValidExtention.Any(c => c == ext);
                        }
                        if (valid)
                        {
                            var randome = Path.GetRandomFileName();
                            var filename = randome + ext;
                            var tuple = (item, filename, true);
                            output.Add(tuple);
                        }
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task SaveImg(List<(string base64File, string filename)> Base64VFName, string WebRootPath, string FolderParent, string FolderChild, int LargestSize)
        {
            foreach (var item in Base64VFName)
            {
                var index = item.base64File.IndexOf(",");
                var base64 = item.base64File.Substring(index + 1);
                byte[] bytes = Convert.FromBase64String(base64);
                if (bytes.Any())
                {
                    var ext = GetFileExtension(base64);
                    // var randome = Path.GetRandomFileName();
                    var filename = item.filename; //randome + ext;
                    string pathFolder = Path.Combine(WebRootPath, FolderParent, FolderChild);
                    if (!Directory.Exists(pathFolder))
                    {
                        Directory.CreateDirectory(pathFolder);
                    }

                    var pathFile = Path.Combine(pathFolder, filename);
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        byte[] resize = ResizeToByte(bytes: bytes, FileFormat: ext, LargestSize: LargestSize);
                        await stream.WriteAsync(resize, 0, resize.Length);
                        await stream.FlushAsync();
                    }
                }

            }
        }
        public static async Task RemoveFile(string WebRootPath, string FolderParent, string FolderChild, string FileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (WebRootPath != null && FolderParent != null && FolderChild != null && FileName != null)
                    {
                        string path = Path.Combine(WebRootPath, FolderParent, FolderChild, FileName);
                        if (!Directory.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                }
                catch { }
            });

        }


        public static async Task SaveImg(List<(string base64File, string filename)> Base64VFName, string WebRootPath, string FolderParent, string FolderChild, string FolderSubChild, int LargestSize)
        {
            foreach (var item in Base64VFName)
            {
                var index = item.base64File.IndexOf(",");
                var base64 = item.base64File.Substring(index + 1);
                byte[] bytes = Convert.FromBase64String(base64);
                if (bytes.Any())
                {
                    var ext = GetFileExtension(base64);
                    // var randome = Path.GetRandomFileName();
                    var filename = item.filename; //randome + ext;
                    string pathFolder = Path.Combine(WebRootPath, FolderParent, FolderChild);
                    if (!Directory.Exists(pathFolder))
                    {
                        Directory.CreateDirectory(pathFolder);
                    }

                    string pathSubFolder = Path.Combine(pathFolder, FolderSubChild);
                    if (!Directory.Exists(pathSubFolder))
                    {
                        Directory.CreateDirectory(pathSubFolder);
                    }

                    var pathFile = Path.Combine(pathSubFolder, filename);

                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        byte[] resize = ResizeToByte(bytes: bytes, FileFormat: ext, LargestSize: LargestSize);
                        await stream.WriteAsync(resize, 0, resize.Length);
                        await stream.FlushAsync();
                    }
                }
            }
        }

        public static async Task SaveFile(List<(string base64File, string filename)> Base64VFName, string path, int? LargestSize = null, int? MinSize = null, int? width = null, int? height = null)
        {
            foreach (var item in Base64VFName)
            {
                var index = item.base64File.IndexOf(",");
                var base64 = item.base64File.Substring(index + 1);
                byte[] bytes = Convert.FromBase64String(base64);
                if (bytes.Any())
                {
                    var ext = GetFileExtension(base64);
                    // var randome = Path.GetRandomFileName();
                    var filename = item.filename; //randome + ext;
                    string pathFolder = Path.Combine(path);
                    if (!Directory.Exists(pathFolder))
                    {
                        Directory.CreateDirectory(pathFolder);
                    }

                    var pathFile = Path.Combine(pathFolder, filename);

                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        byte[] resize = { };
                        if (LargestSize != null || MinSize != null || width != null || height != null)
                        {
                            resize = ResizeToByte(bytes: bytes, FileFormat: ext, LargestSize: LargestSize, MinSize: MinSize, width: width, height: height);
                        }
                        await stream.WriteAsync((resize.Count() > 0) ? resize : bytes, 0, (resize.Count() > 0) ? resize.Length : bytes.Length);
                        await stream.FlushAsync();
                    }
                }
            }
        }

        public static async Task SaveFile(List<(string base64File, string filename)> Base64VFName, string WebRootPath, string FolderParent, string FolderChild, string FolderSubChild)
        {
            foreach (var item in Base64VFName)
            {
                var index = item.base64File.IndexOf(",");
                var base64 = item.base64File.Substring(index + 1);
                byte[] bytes = Convert.FromBase64String(base64);
                if (bytes.Any())
                {
                    var ext = GetFileExtension(base64);
                    // var randome = Path.GetRandomFileName();
                    var filename = item.filename; //randome + ext;
                    string pathFolder = Path.Combine(WebRootPath, FolderParent, FolderChild);
                    if (!Directory.Exists(pathFolder))
                    {
                        Directory.CreateDirectory(pathFolder);
                    }

                    string pathSubFolder = Path.Combine(pathFolder, FolderSubChild);
                    if (!Directory.Exists(pathSubFolder))
                    {
                        Directory.CreateDirectory(pathSubFolder);
                    }

                    var pathFile = Path.Combine(pathSubFolder, filename);

                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        // byte[] resize = ResizeToByte(bytes, LargestSize, ext);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        await stream.FlushAsync();
                    }
                }
            }
        }


        public static async Task RemoveFile(string WebRootPath, string FolderParent, string FolderChild, string FolderSubChild, string FileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (WebRootPath != null && FolderParent != null && FolderChild != null && FileName != null)
                    {
                        string path = Path.Combine(WebRootPath, FolderParent, FolderChild, FolderSubChild, FileName);
                        if (!Directory.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                }
                catch { }
            });

        }
        public static async Task RemoveOldFile(string WebRootPath, string FolderParent, string FolderChild, string FileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (WebRootPath != null && FolderParent != null && FolderChild != null && FileName != null)
                    {
                        string path = Path.Combine(WebRootPath, FolderParent, FolderChild, FileName);
                        if (!Directory.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                }
                catch { }
            });

        }

        public static byte[] ResizeToByte(byte[] bytes, string FileFormat, int? LargestSize = null, int? MinSize = null, int? width = null, int? height = null)
        {
            byte[] Resized;

            using (MemoryStream StartMemoryStream = new MemoryStream(),
                                NewMemoryStream = new MemoryStream())
            {
                // write the string to the stream  
                StartMemoryStream.Write(bytes, 0, bytes.Length);

                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap startBitmap = new Bitmap(StartMemoryStream);

                // set thumbnail height and width proportional to the original image.  
                int newHeight = 0;
                int newWidth = 0;
                double HW_ratio = 0;
                if (LargestSize != null)
                {
                    if (startBitmap.Height > startBitmap.Width)
                    {
                        newHeight = LargestSize.Value;
                        HW_ratio = (double)((double)LargestSize / (double)startBitmap.Height);
                        newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                    }
                    else
                    {
                        newWidth = LargestSize.Value;
                        HW_ratio = (double)((double)LargestSize / (double)startBitmap.Width);
                        newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                    }
                }
                else if (MinSize != null)
                {
                    if (startBitmap.Height < startBitmap.Width)
                    {
                        newHeight = MinSize.Value;
                        HW_ratio = (double)((double)MinSize / (double)startBitmap.Height);
                        newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                    }
                    else
                    {
                        newWidth = MinSize.Value;
                        HW_ratio = (double)((double)MinSize / (double)startBitmap.Width);
                        newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                    }
                }
                else if (width != null && height != null)
                {
                    newWidth = width.Value;
                    newHeight = height.Value;
                }
                else if (width != null)
                {
                    newWidth = width.Value;

                    HW_ratio = (double)((double)width.Value / (double)startBitmap.Width);
                    newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                }
                else if (height != null)
                {

                    HW_ratio = (double)((double)width.Value / (double)startBitmap.Height);
                    newWidth = (int)(HW_ratio * (double)startBitmap.Width);

                    newHeight = height.Value;
                }
                // create a new Bitmap with dimensions for the thumbnail.  
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);

                // Copy the image from the START Bitmap into the NEW Bitmap.  
                // This will create a thumnail size of the same image.  
                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);

                // Save this image to the specified stream in the specified format.  
                switch (FileFormat)
                {
                    case ".png":
                        {

                            newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        }
                    default:
                        {
                            newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        }
                    case null:
                        {
                            newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        }

                }
                newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Fill the byte[] for the thumbnail from the new MemoryStream.  
                Resized = NewMemoryStream.ToArray();
            }

            // return the resized image as a string of bytes.  
            return Resized;
        }

        // Resize a Bitmap  
        public static byte[] ResizeToByte(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                {
                    int h = 100;
                    int w = 100;

                    using (Bitmap b = new Bitmap(img, new Size(w, h)))
                    {
                        using (MemoryStream ms2 = new MemoryStream())
                        {
                            b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                            return bytes = ms2.ToArray();
                        }
                    }
                }
            }
        }
        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;


                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }
        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);
            //var data2 = base64String.Substring(0, 6);

            //if (data2== "UEsDBA")
            //{
            //    return ".zip";
            //}
            //if (data2 == "UEsDBB")
            //{
            //      return ".docx";
            //}

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return ".png";
                case "/9J/4":
                    return ".jpg";

                case "AAAAG":
                    return ".mp4";
                case "AAAAF":
                    return ".mp4";
                case "AAAAI":
                    return ".mp4";
                case "SUQZA":
                    return ".mp3";
                case "UESDB":
                    return ".zip";
                case "QK3M6":
                    return ".tif";

                case "JVBER":
                    return ".pdf";
                case "AAABA":
                    return ".ico";
                case "UMFYI":
                    return ".rar";
                case "E1XYD":
                    return ".rtf";
                case "U1PKC":
                    return ".txt";
                case "MQOWM":
                case "77U/M":
                    return ".srt";

                case "77u/2":
                    return ".csv";
                default:
                    return ".csv";
            }
        }
    }
}

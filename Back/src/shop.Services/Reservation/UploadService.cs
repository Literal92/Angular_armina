using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using shop.Services.Contracts.Reservation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation.Enum;
using System.Linq;
using shop.Services.Handler;

namespace shop.Services.Reservation
{
    /// <summary>
    /// Summary description for Upload
    /// </summary>
    /// 
    /// 
    public class UploadService : IUploadService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UploadService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

        }
        private bool IsExtensionAllowed(IList<string> allowedExtensions, string fileExtension)
        {
            int length = allowedExtensions.Count;
            if (length == 0)
                return true;
            for (int i = 0; i < length; i++)
            {
                if (String.CompareOrdinal(allowedExtensions[i], fileExtension) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual (bool valid, string filename, string error) GetFileNameAndValidation(IFormFile file, List<string> allowFormats, int? sizeKb)
        {
            try
            {
                if (sizeKb != null)
                {
                    if (file.Length / 1024 > sizeKb)
                        return (false, null, $"حداکثر سایز:{sizeKb}Kb");
                }

                var ext = Path.GetExtension(file.FileName);

                var valid = true;
                if (allowFormats != null)
                {
                    valid = IsExtensionAllowed(allowFormats, file.ContentType);
                }
                if (!valid)
                    return (false, null, "فرمت مجاز نیست");


                var randome = Path.GetRandomFileName();
                var filename = randome + ext;

                return (true, filename, null);

            }
            catch (Exception ex)
            {
                return (false, null, "خطا در بررسی فایل !");
            }
        }

        public string UploadPicResize(IFormFile file, string path, int maxPicWidth, ref long serverFileSize, int[] size, Dimensions dimension, ref string[] resized)
        {
            var image = Image.FromStream(file.OpenReadStream(), true, true);
            var allowedMimeType = new[] { "image/png", "image/x-png", "image/pjpeg", "image/jpeg", "image/gif" };
            var serverfileName = "";
            var webroot = _hostingEnvironment.WebRootPath;
            path = webroot + path;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            int result = 0; //Failor Flag

            if (file != null && file.Length != 0)
            {
                var a = file.OpenReadStream();
                var filePath = "";
                var userFileName = file.FileName;
                if (IsExtensionAllowed(allowedMimeType, file.ContentType))
                {

                    var r = new Random();
                    if (File.Exists(Path.Combine(path, userFileName)))
                    {
                        string newstr;
                        do
                        {
                            int randomnumber = r.Next(1, 10);
                            newstr = string.Concat(DateTime.Now.ToShortPersianDateString().Replace("/", ""), randomnumber,
                              userFileName.Replace(" ", ""));
                        } while (File.Exists(path + newstr));

                        filePath = Path.Combine(path, newstr);

                        if (image.Width > maxPicWidth)
                        {


                            var resizedImg = ResizeImage(image, maxPicWidth, Dimensions.Width, filePath);
                            // resizedImg.Save(filePath);

                        }
                        else
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                        }

                        serverfileName = newstr;
                        serverFileSize = file.Length / 1024; //Save server file size in KB
                    }
                    else
                    {
                        filePath = Path.Combine(path, userFileName.Replace(" ", ""));

                        if (image.Width > maxPicWidth)
                        {
                            var resizedImg = ResizeImage(image, maxPicWidth, Dimensions.Width, filePath);
                            //    resizedImg.Save(filePath);
                        }
                        else
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                        }
                        serverfileName = userFileName.Replace(" ", "");
                        serverFileSize = file.Length / 1024; //Save server file size in KB
                    }

                    for (int i = 0; i < size.Length; i++)
                    {
                        resized[i] = ResizeImage(image, serverfileName, path, size[i], Dimensions.Width);
                    }
                    return serverfileName;
                }
                throw new Exception("unexpected extension in upload Service");
            }
            throw new Exception("No File were selected in upload Service");
        }

        public string UploadFile(IFormFile file, string path, ref long serverFileSize, string[] allowedExtensions)
        {
            var serverfileName = "";
            var webroot = _hostingEnvironment.WebRootPath;
            path = webroot + path;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            int result = 0; //Failor Flag

            if (file != null && file.Length != 0)
            {
                var userFileName = file.FileName;
                if (IsExtensionAllowed(allowedExtensions, file.ContentType))
                {
                    var r = new Random();
                    if (File.Exists(Path.Combine(path, userFileName)))
                    {
                        string newstr;
                        do
                        {
                            int randomnumber = r.Next(1, 10);
                            newstr = string.Concat(DateTime.Now.ToShortPersianDateString().Replace("/", ""), randomnumber,
                              userFileName.Replace(" ", ""));
                        } while (File.Exists(path + newstr));

                        var filePath = Path.Combine(path, newstr);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        serverfileName = newstr;
                        serverFileSize = file.Length / 1024; //Save server file size in KB
                    }
                    else
                    {
                        var filePath = Path.Combine(path, userFileName.Replace(" ", ""));

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        serverfileName = userFileName.Replace(" ", "");
                        serverFileSize = file.Length / 1024; //Save server file size in KB
                    }

                    return serverfileName;

                }
                throw new Exception("unexpected extension in upload Service");
            }
            throw new Exception("No File were selected in upload Service");
        }


        public virtual async Task UploadFile(IFormFile file, string fileName, string path)
        {
            try
            {

                var webroot = _hostingEnvironment.WebRootPath;
                path = webroot + path;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var filePath = Path.Combine(path, fileName.Trim());

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task UploadFiles(string userId, IEnumerable<IFormFile> files, string[] allowedExtensions, UploadType uploadType)
        {
            foreach (var item in files)
            {
               await Upload(userId, item, allowedExtensions, uploadType);
            }
        }

        public async Task<string> Upload(string id, IFormFile file, string[] allowedExtensions, UploadType uploadType, bool createTumb = true)
        {
            var webroot = _hostingEnvironment.WebRootPath;
            var path = webroot;
            path = Path.Combine(path, "Upload");
            //path = Path.Combine(path, "Images");

            if (file != null && file.Length != 0)
            {
                if (IsExtensionAllowed(allowedExtensions, file.ContentType))
                    return null;

                //if (!string.IsNullOrEmpty(id))
                //{
                //    path = Path.Combine(path, "Users");
                //    path = Path.Combine(path, id);
                //}

                if (uploadType == UploadType.User)
                {
                    path = Path.Combine(path, "Users");
                    path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.Product)
                {
                    path = Path.Combine(path, "Product");
                    path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.DocumentsProvider)
                {
                    path = Path.Combine(path, "Provider");
                    path = Path.Combine(path, id);
                    path = Path.Combine(path, "Documents");
                }
                else if (uploadType == UploadType.Category)
                {
                    path = Path.Combine(path, "Category");
                    path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.SubService)
                {
                    path = Path.Combine(path, "SubService");
                    path = Path.Combine(path, id);
                }

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                var filename = Path.GetRandomFileName();
                var extension = Path.GetExtension(file.FileName);
                var pathFinal = Path.Combine(path, filename + extension);

                if (File.Exists(pathFinal))
                {
                    //Delete it
                    File.Delete(pathFinal);
                }

                {
                    using (FileStream fs = System.IO.File.Create(pathFinal))
                    {
                        //file.CopyTo(fileStream);
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
                if (createTumb)
                {
                    ResizeImage(path, filename + file.ContentType, 150, Dimensions.Width);
                }
                return file.FileName;
            }
            throw new Exception("No File were selected in upload Service");
        }

        public async Task<List<(string base64File, string filename,bool ValidSize)>> GetFileName(string base64, List<string> ValidExtention, int? ChkSize= 10485760)
        {
            return ImgHandler.GetFilename(new List<string>{base64}, ValidExtention, ChkSize);
        }

        public virtual async Task<string> Upload(string id, (string base64File, string filename) file,  UploadType uploadType, bool createTumb = true, int? LargestSize=null,int? MinSize=null, int?width=null, int? height=null)
        {
            var webroot = _hostingEnvironment.WebRootPath;
            var path = webroot;
            path = Path.Combine(path, "Upload");
            //path = Path.Combine(path, "Images");

            if (file.base64File != null && file.base64File.Length != 0)
            {
                // filename
               // var FileExtension = ImgHandler.GetFileExtension(file.base64File);

                //if (!string.IsNullOrEmpty(id))
                //{
                //    path = Path.Combine(path, "Users");
                //    path = Path.Combine(path, id);
                //}

                if (uploadType == UploadType.User)
                {
                    path = Path.Combine(path, "Users");
                    path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.Product)
                {
                    path = Path.Combine(path, "Product");
                    path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.Category)
                {
                    path = Path.Combine(path, "Category");
                    path = Path.Combine(path, id);
                }
               
                else if (uploadType == UploadType.Field)
                {
                    path = Path.Combine(path, "Field");
                    path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.CSV)
                {
                    path = Path.Combine(path, "CSV");
                  //  path = Path.Combine(path, id);
                }
                else if (uploadType == UploadType.Order)
                {
                    path = Path.Combine(path, "Order");
                    //  path = Path.Combine(path, id);
                }
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);




               // var filename = Path.GetRandomFileName();
                //var extension = FileExtension;
                var pathFinal = Path.Combine(path, file.filename);

                if (File.Exists(pathFinal))
                {
                    //Delete it
                    File.Delete(pathFinal);
                }

                // upload file
                await ImgHandler.SaveFile(Base64VFName: new List<(string base64File, string filename)> { file },path: path,LargestSize: LargestSize, MinSize: MinSize, width:width, height:height);
                //if (createTumb)
                //{
                //    ResizeImage(path, filename + file.ContentType, 150, Dimensions.Width);
                //}
                return file.filename;
            }
            throw new Exception("No File were selected in upload Service");
        }
        public async Task<string> getUploadedFileName(string filename, string userId, UploadType uploadType, string id = null)
        {
            if (string.IsNullOrEmpty(filename)) return "";

            var path = "/";
            path = Path.Combine(path, "Assets");
            path = Path.Combine(path, "Images");

            if (!string.IsNullOrEmpty(userId))
            {
                path = Path.Combine(path, "Users");
                path = Path.Combine(path, userId);
            }
            if (uploadType == UploadType.Product)
            {
                path = Path.Combine(path, "Product");
                path = Path.Combine(path, id);
            }
            else if (uploadType == UploadType.DocumentsProvider)
            {
                path = Path.Combine(path, "Provider");
                path = Path.Combine(path, id);
                path = Path.Combine(path, "Documents");
            }

            path = Path.Combine(path, filename);

            return path;
        }

        public async Task RemoveFile(string WebRootPath, string FolderParent, string FolderChild, string FileName)
        {
            await  ImgHandler.RemoveFile(WebRootPath, FolderParent, FolderChild, FileName);
        }

        public virtual async Task<(bool success, string error)> UploadImgAndResponsive(IFormFile file, string fileName, string path, int maxPicWidth, Dimensions dimension)
        {
            try
            {
                var image = Image.FromStream(file.OpenReadStream(), true, true);
                var webroot = _hostingEnvironment.WebRootPath;
                path = webroot + path;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (file == null && file.Length == 0)
                    return (false, "فایلی انتخاب نشده است !");

                var filePath = Path.Combine(path, fileName.Trim());

                if (image.Width > maxPicWidth)
                {
                    ResizeImage(image, maxPicWidth, Dimensions.Width, filePath);
                }
                else
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                return (true, null);
            }
            catch
            {
                return (false, "خطا در ثبت فایل");
            }
        }
        #region Resize

        public Image ResizeImage(Image imgToResize, int size, Dimensions dimension, string dest)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = (size / (float)sourceWidth);
                    break;
                default:
                    nPercent = ((float)size / sourceHeight);
                    break;
            }

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(b);

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            b.Save(dest, imgToResize.RawFormat);
            g.Dispose();
            b.Dispose();
            //   imgToResize.Dispose();

            return b;
        }

        /// <summary>
        /// This method used to resize images and like resizefile method, but this has most quality
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size">determinde a int for width/height of image and set its height/width proportion to width/height</param>
        /// <param name="dimension">dimension determine size for width or height</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ResizeImage(string fileName, string path, string album, int size, Dimensions dimension)
        {
            var strAddress = _hostingEnvironment.WebRootPath + "/" + path + "/" + fileName;
            if (!File.Exists(strAddress))
                return "no Exist"; // not exist source file

            string dest = _hostingEnvironment.WebRootPath + "/" + path + "/" + album + "/" + size;
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            Stream st = new FileStream(strAddress, FileMode.Open);
            var imgToResize = Image.FromStream(st);
            st.Close();
            st.Dispose();
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;
            float nPercent = 0;
            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = ((float)size / sourceWidth);
                    break;
                case Dimensions.Height:
                    break;
                default:
                    nPercent = (size / (float)sourceHeight);
                    break;
            }
            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);
            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(b);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            b.Save(dest + "/" + album + ".jpg", imgToResize.RawFormat);
            g.Dispose();
            b.Dispose();
            imgToResize.Dispose();
            return album + ".jpg";

        }

        public bool ResizeImage(string path, string fileName, int size, Dimensions dimension)
        {
            var strAddress = _hostingEnvironment.WebRootPath + "/" + path + "/" + fileName;
            if (!File.Exists(strAddress))
                return false; // not exist source file

            Stream st = new FileStream(strAddress, FileMode.Open);
            var imgToResize = Image.FromStream(st);
            st.Close();
            st.Dispose();
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;
            float nPercent = 0;
            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = ((float)size / sourceWidth);
                    break;
                case Dimensions.Height:
                    break;
                default:
                    nPercent = (size / (float)sourceHeight);
                    break;
            }
            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);
            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(b);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            b.Save(path + "/Thumb", imgToResize.RawFormat);
            g.Dispose();
            b.Dispose();
            imgToResize.Dispose();
            return true;
        }


        public string ResizeImage(Image imgToResize, string fileName, string path, int size, Dimensions dimension)
        {

            string dest = path + "/" + size;
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;
            float nPercent = 0;
            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = ((float)size / sourceWidth);
                    break;
                case Dimensions.Height:
                    break;
                default:
                    nPercent = (size / (float)sourceHeight);
                    break;
            }
            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);
            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(b);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            b.Save(dest + "/" + fileName, imgToResize.RawFormat);
            g.Dispose();
            b.Dispose();
            imgToResize.Dispose();
            return fileName;

        }
        #endregion
    }
}
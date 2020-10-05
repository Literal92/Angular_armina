
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation.Enum;

namespace shop.Services.Contracts.Reservation
{
    public interface IUploadService
    {

        string UploadPicResize(IFormFile file, string path, int maxPicWidth, ref long serverFileSize, int[] size, Dimensions dimension, ref string[] resized);

        string UploadFile(IFormFile file, string path, ref long serverFileSize, string[] allowedExtensions);

        Task<string> Upload(string id, IFormFile file, string[] allowedExtensions, UploadType uploadType, bool createTumb = true);
        Task<string> Upload(string id, (string base64File, string filename) file, UploadType uploadType, bool createTumb = true, int? LargestSize = null, int? MinSize=null,int? width=null,int? height=null);
        Task<List<(string base64File, string filename, bool ValidSize)>> GetFileName(string base64, List<string> ValidExtention, int? ChkSize= 10485760);
        Task UploadFiles(string userId, IEnumerable<IFormFile> files, string[] allowedExtensions, UploadType uploadType);

        Task<string> getUploadedFileName(string filename, string userId, UploadType uploadType, string storeId = null);
        Task RemoveFile(string WebRootPath, string FolderParent, string FolderChild, string FileName);
        string ResizeImage(string source, string destination, string album, int size, Dimensions dimension);
        (bool valid, string filename, string error) GetFileNameAndValidation(IFormFile file, List<string> allowFormats, int? sizeKb);
        Task UploadFile(IFormFile file, string fileName, string path);

        Task<(bool success, string error)> UploadImgAndResponsive(IFormFile file, string fileName, string path, int maxPicWidth, Dimensions dimension);
    }
}
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Reservation;
using shop.Services.Identity;
using shop.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace shop.Area.Api.Controllers
{
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantRoles.Admin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class TrackingCodeController : Controller
    {
        #region Fields
        private readonly ITrackingCodeService _trackingCodeService;
        private readonly IUploadService _uploadService;
        private IWebHostEnvironment _hostingEnvironment;
        #endregion

        #region Ctor
        public TrackingCodeController(ITrackingCodeService trackingCodeService,
        IUploadService uploadService,
         IWebHostEnvironment hostingEnvironment)
        {
            _trackingCodeService = trackingCodeService;
            _uploadService = uploadService;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Methods    


        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create([FromBody] TrackingCodeFileViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");
            try
            {
                var filesIcon = new List<(string base64File, string filename, bool ValidSize)>();

                //// get filename
                if (model.FileName != default)
                {
                    filesIcon = await _uploadService.GetFileName(model.FileName, new List<string> { ".csv", ".jpeg", ".png", ".svg" }, 20048000);
                    if (filesIcon.FirstOrDefault().ValidSize != true)
                        return BadRequest($"حداکثر سایز مجاز برای ایکون KB 2000 است");

                    model.FileName = filesIcon.FirstOrDefault().filename;
                }

                if (model.FileName != null)
                {
                    var file = (filesIcon.FirstOrDefault().base64File, filesIcon.FirstOrDefault().filename);
                    await _uploadService.Upload(id: model.FileName.ToString(), file: file, uploadType: UploadType.CSV);

                }


                TextReader reader = new StreamReader(_hostingEnvironment.WebRootPath + "/upload/csv/" + model.FileName);
                var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.CurrentUICulture);
                var records = csvReader.GetRecords<TrackingCodeCSVModel>().ToList()
                    .Select(p => new TrackingCode
                    {
                        City = p.City,
                        Code = p.Code,

                        Name = p.Reciver,
                        SendDate = Convert.ToDateTime(model.SendDate)
                    });


                await _trackingCodeService.Create(records.ToList());

                return Ok(true);


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });

            }
        }


        #endregion
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rent.Models;
using test1.Bl;
using test1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        Isettings ClsSettings;
        public SettingsController(Isettings isettings)
        {
            ClsSettings = isettings;
        }

        [HttpGet("Show")]
        public ApiResponse Show()
        {
                ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsSettings.Show();
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }

        // POST api/<SettingsController>
        [HttpPost("Edit")]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResponse> Edit(
          [FromBody] TbSetting? setting,
          [FromForm] List<IFormFile>? favicon,
          [FromForm] List<IFormFile>? Logo,
          [FromForm] List<IFormFile>? HomeImg1,
          [FromForm] List<IFormFile>? HomeImg2,
          [FromForm] List<IFormFile>? HomeImg3)
        {
            BL bL=new BL();
            ApiResponse response = new ApiResponse();
            try
            {
                if (setting == null)
                {
                    response.errorMessage = "error in data";
                    response.status = 400;
                    return response;
                }
                        

             
                setting.Logo = await bL.TryUpload(Logo,"Settings", setting.Logo);
                setting.Favicon = await bL.TryUpload(favicon, "Settings", setting.Favicon);
                setting.HomeImg1 = await bL.TryUpload(HomeImg1, "Settings", setting.HomeImg1);
                setting.HomeImg2 = await bL.TryUpload(HomeImg2, "Settings", setting.HomeImg2);
                setting.HomeImg3 = await bL.TryUpload(HomeImg3, "Settings", setting.HomeImg3);

                response.data = ClsSettings.Edit(setting);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.status = 500;
                response.errorMessage = ex.Message;
                return response;
            }
        }

        private async Task<string> TryUpload(List<IFormFile>? files, string PathName, string currentValue)
        {
            if (files != null && files.Any(f => f.Length > 0))
            {
                return await UploadImage(files,PathName);
            }
            return currentValue;
        }

        private async Task<string> UploadImage(List<IFormFile> files,string PathName)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads",PathName , imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return imageName;
                }
            }
            return "";
        }
    }
}

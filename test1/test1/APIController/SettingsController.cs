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
        IReview ClsReview;
        public SettingsController(Isettings isettings,IReview review)
        {
            ClsSettings = isettings;
            ClsReview = review;
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
          [FromForm] TbSetting? setting,
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
                    response.errorMessage = "خطأ بالبيانات";
                    response.status = 400;
                    return response;
                }
                        

             
                setting.Logo = await bL.TryUpload(Logo, setting.Logo);
                setting.Favicon = await bL.TryUpload(favicon, setting.Favicon);
                setting.HomeImg1 = await bL.TryUpload(HomeImg1, setting.HomeImg1);
                setting.HomeImg2 = await bL.TryUpload(HomeImg2, setting.HomeImg2);
                setting.HomeImg3 = await bL.TryUpload(HomeImg3, setting.HomeImg3);

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

        [HttpGet("ShowFiveStars")]
        [AllowAnonymous]
        public ApiResponse ShowFiveStars()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.Get5Stars();
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;

            }

        }
    }
}

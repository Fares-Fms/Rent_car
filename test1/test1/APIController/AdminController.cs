using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rent.Models;
using test1.Bl;
using test1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class AdminController : ControllerBase
    {
        UserManager<ApplicationUser> userManager;
        IAd ClsAd;
        IReview ClsReview;
        RentCarContext Context;
        public AdminController(UserManager<ApplicationUser> UserManager, IAd ad, IReview review, RentCarContext rentCar)
        {
            userManager = UserManager;
            ClsAd = ad;
            ClsReview = review;
            Context = rentCar;
        }
        [HttpGet("GetUsers")]
        public async Task<ApiResponse> GetUsers()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var users = userManager.Users.ToList();
                var userRolesViewModel = new List<UserModel>();

                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    userRolesViewModel.Add(new UserModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        roles = roles,
                        Number = user.PhoneNumber,
                        Email = user.Email

                    });
                }
                response.data = userRolesViewModel;
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }
        [HttpPost("DeActive/{id}")]
        public async Task<ApiResponse> DeActive(string id)
        {
            ApiResponse response = new ApiResponse();
            try
            {

                var user = await userManager.FindByIdAsync(id);
                if (user.IsActive == false)
                {
                    response.errorMessage = "المستخدم غير نشط بالفعل";
                    return response;
                }
                if (user != null)
                {
                    user.IsActive = false;
                    await userManager.UpdateAsync(user);
                    response.data = "Done";
                    response.status = 200;
                }
                else
                {
                    response.errorMessage = "خطأ في معرف المستخدم";
                }

            }
            catch (Exception ex)
            {

                response.errorMessage = ex.Message;
            }
            return response;
        }
        [HttpPost("Active/{id}")]
        public async Task<ApiResponse> Active(string id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user.IsActive == true)
                {
                    response.errorMessage = "المستخدم نشط بالفعل";
                    return response;
                }
                if (user != null && user.IsActive == false)
                {
                    user.IsActive = true;
                    await userManager.UpdateAsync(user);
                    response.data = "تم التفعيل";
                    response.status = 200;
                }
                else
                {
                    response.errorMessage = " خطأ في معرف المستخدم";
                }

            }
            catch (Exception ex)
            {

                response.errorMessage = ex.Message;
            }
            return response;
        }

        [HttpPost("CreateAd")]
        public async Task<ApiResponse> CreateAd([FromForm] TbAd tb, [FromForm] List<IFormFile> File1)
        {
            ApiResponse response = new ApiResponse();
            BL bl = new BL();
            try
            {

                if (tb == null)
                {
                    response.errorMessage = "قم بتعبئة الحقول";
                    return response;
                }

                if (File1 == null || !File1.Any())
                {
                    response.errorMessage = "الرجاء ادخال الصورة ";
                    return response;
                }


                tb.Status = "Available";
                tb.CreateDate = DateTime.Now;
                tb.ImgName = await bl.UploadImage(File1);

                await Context.TbAds.AddAsync(tb);
                await Context.SaveChangesAsync();
                response.data = "Done";

            }

            catch (Exception ex)
            {
                response.errorMessage = ex.Message;

                // Cleanup uploaded image if creation failed
                if (!string.IsNullOrEmpty(tb.ImgName))
                {
                    await bl.DeleteImageAsync(tb.ImgName);
                }


            }


            return response;

        }

    
        [HttpPost("DeleteAd")]
        public ApiResponse DeleteAd(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.Delete(id);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }

        [HttpPost("ApproveComment/{id}")]
        public ApiResponse ApproveComment(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.ApproveReview(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        } 
        [HttpGet("ShowALLComments")]
        public ApiResponse ShowComments()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.GetReviewsAdmin();
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }
        [HttpGet("CountComments")]
        public ApiResponse CountComments()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.Count();
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }
        [HttpPost("DeclineComment/{id}")]
        public ApiResponse DeclineComment(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.DeclineReview(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }

    }
}

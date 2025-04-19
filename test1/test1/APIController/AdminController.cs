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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        UserManager<ApplicationUser> userManager;
        IAd ClsAd;
        IReview ClsReview;
        public AdminController(UserManager<ApplicationUser> UserManager, IAd ad,IReview review)
        {
            userManager = UserManager;
            ClsAd = ad;
            ClsReview = review;
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

        [HttpPost("CreateAd")]
        public ApiResponse CreateAd([FromForm]TbAd tb, [FromForm] List<IFormFile> File1)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.CreateAd(tb, File1);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
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

        [HttpPost("ApproveComment")]
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
           [HttpPost("DeclineComment")]
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

using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rent.Models;
using System.Security.Claims;
using test1.Bl;
using test1.Models;

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        RentCarContext context;
        UserManager<ApplicationUser> userManager;
        ICar ClsCar;
        IReview ClsReview;
        public CarController(RentCarContext rentCar,UserManager<ApplicationUser> user,ICar icar,IReview review)
        {
            context = rentCar;
            userManager = user;
            ClsCar = icar;
            ClsReview = review;
        }
        [HttpPost("EditCar/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ApiResponse> EditCar(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var existingCar = ClsCar.ShowCar(id);
                if (existingCar == null || existingCar.Status == "Deleted" || existingCar.Sold == 1)
                {
                    response.errorMessage = "السيارة غير موجودة";
                    response.status = 405;
                 
                }
                else
                {

                response.data = existingCar;
                response.status = 200;
                }
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }

        [HttpPost("SaveEditCar/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ApiResponse> SaveEditCar([FromRoute]int Id, [FromForm] TbCar? car, [FromForm] List<IFormFile>? File1, [FromForm] List<IFormFile>? File2, [FromForm] List<IFormFile>? File3)
        {
            BL bL = new BL();
            ApiResponse apiResponse = new ApiResponse();
            if (!ModelState.IsValid)
            {
                apiResponse.errorMessage = "ادخال خاطئ";
                return apiResponse;
            }
            try
            {
                var userid = userManager.GetUserId(User);
                var newCar = ClsCar.ShowCar(Id);

                if (newCar.UserId == userid)
                {
                  
                        car.Img1 = await bL.TryUpload(File1,car.Img1);

                   
                        car.Img2 = await bL.TryUpload(File2, car.Img2);

                
                        car.Img3 = await bL.TryUpload(File3, car.Img3);

                    
                    car.UpdateDate = DateTime.Now;
                    context.Entry(car).State = EntityState.Modified;

                    await context.SaveChangesAsync();
                }
                else
                {
                    apiResponse.errorMessage = "انت لست مالك السيارة";
                    apiResponse.status = 400;
                    return apiResponse;
                }
                apiResponse.data = "تم التعديل بنجاح";
                apiResponse.status = 200;
            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;

                // Cleanup uploaded image if creation failed
                if (!string.IsNullOrEmpty(car.Img1))
                {
                    await bL.DeleteImageAsync(car.Img1);
                }

                if (!string.IsNullOrEmpty(car.Img2))
                {
                    await bL.DeleteImageAsync(car.Img2);
                }
                if (!string.IsNullOrEmpty(car.Img3))
                {
                    await bL.DeleteImageAsync(car.Img3);
                }
            }
                return apiResponse;

        }



        [HttpPost("AddCar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ApiResponse> AddCar([FromForm] TbCar car, [FromForm] List<IFormFile> File1 , [FromForm] List<IFormFile> File2, [FromForm] List<IFormFile> File3)
        {
            BL bL = new BL(); 
            ApiResponse apiResponse = new ApiResponse();
            if (!ModelState.IsValid)
            {

                apiResponse.status = Unauthorized();
                apiResponse.errorMessage = "ادخال غير صحيح";
                return apiResponse;
            }
            try
            {
                car.Img1=await bL.UploadImage(File1);
                car.Img2=await bL.UploadImage(File2);
                car.Img3=await bL.UploadImage(File3);
                var userid = userManager.GetUserId(User);
                car.UserId = userid;
                car.CreateDate = DateTime.Now;
                car.IsDeleted = false;
                context.TbCars.Add(car);
                await context.SaveChangesAsync();
                apiResponse.data = "تم اضافة السيارة بنجاح";
                apiResponse.status = 200;
            
            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;

                // Cleanup uploaded image if creation failed
                if (!string.IsNullOrEmpty(car.Img1))
                {
                    await bL.DeleteImageAsync(car.Img1);
                }

                if (!string.IsNullOrEmpty(car.Img2))
                {
                    await bL.DeleteImageAsync(car.Img2);
                }
                if (!string.IsNullOrEmpty(car.Img3))
                {
                    await bL.DeleteImageAsync(car.Img3);
                }
            }
            return apiResponse;
        }



        [HttpGet("ShowCar/{Id}")]
        public async Task< ApiResponse> ShowCar(int Id)
        {
            ApiResponse apiResponse = new ApiResponse();
            ShowCarModel model = new ShowCarModel();
            try
            {
                var car= ClsCar.ShowCar(Id);
                if (car != null)
                {

                model.Car = car;
                var userid = await userManager.FindByIdAsync(model.Car.UserId);
                
                model.User = SellerProfile(userid);
                model.Reviews = ClsReview.GetReviews(car.Id);
                apiResponse.data = model;
                apiResponse.status=200;
                }
                else
                {
                    apiResponse.errorMessage = "السيارة غير موجودة";
                    apiResponse.status = 400;
                }

            }
            catch (Exception ex) 
            {
                apiResponse.errorMessage = ex.Message;
            }
            return apiResponse;
        }



        [HttpPost("ShowFilteredCars")]
        public ApiResponse ShowFilteredCars([FromBody]FilterCarModel Fcar)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var car = ClsCar.ShowFilteredCars(Fcar);
                apiResponse.data = car;
                apiResponse.status = 200;

            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;
            }
            return apiResponse;
        }

    



        [HttpPost("Delete/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ApiResponse DeleteCar(int Id)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                var userid = userManager.GetUserId(User);
                var car = ClsCar.ShowCar(Id);
                if (car.UserId == userid)
                {
                    car.Status = "Deleted";
                    context.Entry(car).State = EntityState.Modified;
                    context.SaveChanges();
                    apiResponse.data = "done";
                    apiResponse.status = 200;
                    return apiResponse;
                }
                else
                {
                    apiResponse.errorMessage = "انت لست مالك السيارة ";
                    apiResponse.status = "400";
                    return apiResponse;
                }
            }
            catch(Exception ex) { 
                apiResponse.errorMessage = ex.Message;
            return  apiResponse;
            }
        }



        [HttpGet("ShowAllCars")]
        public ApiResponse ShowAllCars()
        {
                ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.ShowALLCars(null);
                response.status= 200;
                return response;
            }
            catch (Exception ex) { 
                response.errorMessage = ex.Message;
            return response;
            }
        }
        [HttpGet("ShowUserCars/{UserId}")]
        public ApiResponse ShowUserCars(string UserId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.ShowALLCars(UserId);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }
        [HttpGet("CountUserCars/{UserId}")]
        public ApiResponse CountUserCars(string UserId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.Count(UserId);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }




        [HttpGet("ShowSixCars/{Id}/{UserId?}")]
        public ApiResponse ShowEightCars(int Id,string? UserId)
        {
                ApiResponse response = new ApiResponse();
            try
            {
 
                response.data = ClsCar.ShowEightCars(Id, UserId);
                response.status= 200;
                return response;
            }
            catch (Exception ex) { 
                response.errorMessage = ex.Message;
            return response;
            }
        }



        [HttpGet("Count")]
        public ApiResponse Count()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.Count(null);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }



        [HttpPost("Sold/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ApiResponse Sold(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var userid = userManager.GetUserId(User);

                response.data = ClsCar.Sold(id,userid);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }

        [HttpPost("AddComment/{carId}")]

        public ApiResponse AddComment([FromRoute]int carId,[FromBody] TbReview review)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.AddReview(review, carId);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;

        }


        public SellerModel SellerProfile(ApplicationUser applicationUser)
        {
            if (applicationUser != null)
            {
                SellerModel model = new SellerModel()
                {
                    Id = applicationUser.Id,
                    Name = applicationUser.UserName,
                    phone = applicationUser.PhoneNumber,
                    Picture = applicationUser.ProfileImage,
                    city =applicationUser.city
                    
                };
                return model;

            }
            return new SellerModel();
        }

    }
}

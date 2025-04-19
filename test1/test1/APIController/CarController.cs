using Azure;
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
        IReview Review;
        public CarController(RentCarContext rentCar,UserManager<ApplicationUser> user,ICar icar,IReview review)
        {
            context = rentCar;
            userManager = user;
            ClsCar = icar;
            Review = review;
        }
        [HttpPost("EditCar/{Id}")]
        [Authorize]
        public async Task<ApiResponse> EditCar(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var existingCar = ClsCar.GetById(id);
                if (existingCar == null || existingCar.Status == "Deleted" || existingCar.Sold == 1)
                {
                    response.errorMessage = "car not found";
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

        [HttpPost("SaveEditCar")]
        [Authorize]
        public async Task<ApiResponse> SaveEditCar([FromForm] TbCar? car, [FromForm] List<IFormFile>? File1, [FromForm] List<IFormFile>? File2, [FromForm] List<IFormFile>? File3)
        {
            BL bL = new BL();
            ApiResponse apiResponse = new ApiResponse();
            if (!ModelState.IsValid)
            {
                apiResponse.errorMessage = "Invalid input";
                return apiResponse;
            }
            try
            {
                var userid = userManager.GetUserId(User);
           

                if (car.UserId == userid)
                {
                  
                        car.Img1 = await bL.TryUpload(File1, "car",car.Img1);

                   
                        car.Img2 = await bL.TryUpload(File2, "car", car.Img2);

                
                        car.Img3 = await bL.TryUpload(File3, "car", car.Img3);

                    
                    car.UpdateDate = DateTime.Now;
                    context.Entry(car).State = EntityState.Modified;

                    await context.SaveChangesAsync();
                }
                else
                {
                    apiResponse.errorMessage = "You are not the owner";
                    apiResponse.status = 400;
                    return apiResponse;
                }
                apiResponse.data = "done";
                apiResponse.status = 200;
                return apiResponse;
            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;
                apiResponse.data = null;

                return apiResponse;
            }

        }



        [HttpPost("AddCar")]
        [Authorize]
        public async Task<ApiResponse> AddCar([FromForm] TbCar car, [FromForm] List<IFormFile> File1 , [FromForm] List<IFormFile> File2, [FromForm] List<IFormFile> File3)
        {
            BL bL = new BL(); 
            ApiResponse apiResponse = new ApiResponse();
            if (!ModelState.IsValid)
            {

                apiResponse.status = Unauthorized();
                apiResponse.errorMessage = "Invalid input";
                return apiResponse;
            }
            try
            {
                car.Img1=await bL.UploadImage(File1,"car");
                car.Img2=await bL.UploadImage(File2, "car");
                car.Img3=await bL.UploadImage(File3, "car");
                var userid = userManager.GetUserId(User);
                car.UserId = userid;
                car.CreateDate = DateTime.Now;
                car.Status = "Available";
                context.TbCars.Add(car);
                await context.SaveChangesAsync();
                apiResponse.data = "done";
                apiResponse.status = 200;
                return apiResponse;
            }
            catch(Exception ex) 
            {
                apiResponse.errorMessage=ex.Message;
                apiResponse.data=null;
             
                return apiResponse;
            }
           
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
                model.Reviews = Review.GetReviews(car.UserId);
                apiResponse.data = model;
                apiResponse.status=200;
                }
                else
                {
                    apiResponse.errorMessage = "This car is not available";
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
        [Authorize]
        public ApiResponse DeleteCar(int Id)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                var userid = userManager.GetUserId(User);
                var car = ClsCar.ShowCar(Id);
                if (car.UserId == userid)
                {
                    car.Status = "0";
                    context.Entry(car).State = EntityState.Modified;
                    context.SaveChanges();
                    apiResponse.data = "done";
                    apiResponse.status = 200;
                    return apiResponse;
                }
                else
                {
                    apiResponse.errorMessage = "you are not the owner of this car";
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
        public ApiResponse ShowSixCars(int Id,string? UserId)
        {
                ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.ShowSixCars(Id,UserId);
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
        [Authorize]
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

                };
                return model;

            }
            return new SellerModel();
        }
    }
}

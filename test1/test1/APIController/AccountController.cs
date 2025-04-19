using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using test1.Models;
using Microsoft.Win32;
using rent.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using test1.Bl;

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        IConfiguration configuration;
        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signIn,IConfiguration conf)
        {

            _userManager = manager;
            _signInManager = signIn;

            configuration = conf;
        }
    


        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ApiResponse> Register([FromBody] Register model, [FromForm] List<IFormFile>? File1)
        {
            BL bl =new BL();
            ApiResponse response = new ApiResponse();
            if (!ModelState.IsValid)
            {
                
                response.status = 405;
                response.errorMessage = "Invalid input";
                return response;
            }


            ApplicationUser user = new ApplicationUser()
            {

                UserName = model.Name,
                PhoneNumber = model.Number,
                Email = model.Email,
                
            };
            try

            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var loginresult = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
                    if(File1 != null) { 
                    user.ProfileImage = await bl.UploadImage(File1, "ProfileImg");
                    var results = await _userManager.UpdateAsync(user);
                    }

                    var C_user = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.AddToRoleAsync(C_user, "Admin");

                    response.data = GenerateToken(C_user);
                    response.status = "201";
                    response.errorMessage = null;
                }
                else
                {
                    response.status = "400";
                    response.errorMessage = "faild";
                }

            }
            catch (Exception ex)
            {
                response.status = "500";
                response.errorMessage = ex.Message;
            }

            return response;
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<ApiResponse> LogIn([FromBody] LogInModel model)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (!ModelState.IsValid)
                {

                    response.status = 405;
                    response.errorMessage = "Invalid input";
                    return response;
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    response.errorMessage = "Error in Email or Password";
                    response.status = 405;
                    return response;
                }

                var loginresult = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, true);
                if (loginresult.Succeeded)
                {
                    response.data = GenerateToken(user);
                    response.status = "200";
                }
                else
                {
                    response.errorMessage = "error in email or password";
                }
            }
            catch(Exception ex)
            {
                response.errorMessage=ex.Message;
            }
            return response;
        }
        [HttpPost("LogOut")]
        [Authorize]
        public async Task<ApiResponse> LogOut()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                await _signInManager.SignOutAsync();
                response.status = "200";
                response.data = "Log Out Success";
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }
        [HttpGet("GetProfile")]
        [Authorize]
        public async Task<ApiResponse> GetProfile()
        {
            ApiResponse response = new ApiResponse();


            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                response.status = 405;
                response.errorMessage = "Error in the Request";
                return response;
            }
            var userData = new
            {
                user.Id,
                user.city,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.ProfileImage,
              
            };
            response.data= userData;
            response.status = "200";
            return response;
        }
        [HttpGet("EditProfile")]
        [Authorize]
        public async Task<ApiResponse> EditProfile(ApplicationUser Auser, List<IFormFile>? File1)
        {
            BL bl = new BL();
            ApiResponse response= new ApiResponse();
            var user = await _userManager.GetUserAsync(User);


            if (user == null)
            {
                response.errorMessage = "user not found";
                return response;
            }

        
                Auser.ProfileImage = await bl.TryUpload(File1, "ProfileImg",Auser.ProfileImage);
            

            var result = await _userManager.UpdateAsync(Auser);

            if (result.Succeeded)
            {
                response.status = 200;
            }
            else
            {
                response.errorMessage = "UnExpected Error";
            }

        
            return response;
        }
            string GenerateToken(IdentityUser user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName)
    };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}

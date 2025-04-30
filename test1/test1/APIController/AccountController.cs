//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using test1.Models;
//using Microsoft.Win32;
//using rent.Models;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.Extensions.Options;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;
//using test1.Bl;

//namespace test1.APIController
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class AccountController : ControllerBase
//    {
//        UserManager<ApplicationUser> _userManager;
//        SignInManager<ApplicationUser> _signInManager;
//        IConfiguration configuration;
//        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signIn, IConfiguration conf)
//        {

//            _userManager = manager;
//            _signInManager = signIn;
//            configuration = conf;
//        }



//        [HttpPost("Register")]
//        [AllowAnonymous]
//        public async Task<ApiResponse> Register([FromForm] Register model, [FromForm] List<IFormFile>? File1)
//        {
//            BL bl = new BL();
//            ApiResponse response = new ApiResponse();
//            if (!ModelState.IsValid)
//            {

//                response.status = 405;
//                response.errorMessage = "Invalid input";
//                return response;
//            }


//            ApplicationUser user = new ApplicationUser()
//            {
//                FullName = model.Name,
//                PhoneNumber = model.Number,
//                Email = model.Email,
//                UserName = $@"{model.Name}_{Guid.NewGuid().ToString().Substring(0, 8)}",
//                city = model.City

//            };

//            try

//            {
//                var result = await _userManager.CreateAsync(user, model.Password);

//                var loginresults = await _userManager.FindByEmailAsync(user.Email);
//                if (loginresults == null)
//                {
//                    response.errorMessage = "the Email is used";
//                }

//                if (result.Succeeded)
//                {
//                    var loginresult = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
//                    if (File1 != null)
//                    {
//                        user.ProfileImage = await bl.UploadImage(File1, "ProfileImg");
//                        var results = await _userManager.UpdateAsync(user);
//                    }

//                    var R_user = await _userManager.FindByEmailAsync(user.Email);
//                    await _userManager.AddToRoleAsync(R_user, "Seller");

//                    response.data = GenerateToken(R_user);
//                    response.status = "201";
//                    response.errorMessage = null;
//                }
//                else
//                {
//                    response.status = "400";
//                    response.errorMessage = "the user is exisit";
//                }

//            }
//            catch (Exception ex)
//            {
//                response.status = "500";
//                response.errorMessage = ex.Message;
//            }



//            return response;
//        }

//        [HttpPost("LogIn")]
//        [AllowAnonymous]
//        public async Task<ApiResponse> LogIn([FromBody] LogInModel model)
//        {
//            ApiResponse response = new ApiResponse();
//            try
//            {
//                if (!ModelState.IsValid)
//                {

//                    response.status = 405;
//                    response.errorMessage = "Invalid input";
//                    return response;
//                }
//                var user = await _userManager.FindByEmailAsync(model.Email);
//                if (user == null)
//                {
//                    response.errorMessage = "Error in Email or Password";
//                    response.status = 405;
//                    return response;
//                }

//                var loginresult = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, true);
//                if (loginresult.Succeeded)
//                {
//                    response.data = GenerateToken(user);
//                    //Response.Cookies.Append("token", JwtToken, new CookieOptions
//                    //{
//                    //    HttpOnly = true,  // تمنع الوصول للتوكن من JavaScript
//                    //    Secure = false,   // استخدم false في بيئة التطوير (localhost)
//                    //    SameSite = SameSiteMode.None // مطلوب مع credentials: 'include'
//                    //});
//                    response.status = "200";
//                }
//                else
//                {
//                    response.errorMessage = "error in email or password";
//                }
//            }
//            catch (Exception ex)
//            {
//                response.errorMessage = ex.Message;
//            }
//            return response;
//        }
//        [HttpPost("LogOut")]
//        [Authorize]
//        public async Task<ApiResponse> LogOut()
//        {
//            ApiResponse response = new ApiResponse();
//            try
//            {
//                await _signInManager.SignOutAsync();
//                response.status = "200";
//                response.data = "Log Out Success";
//            }
//            catch (Exception ex)
//            {
//                response.errorMessage = ex.Message;
//            }
//            return response;
//        }
//        [HttpGet("GetProfile")]
//        [Authorize(Roles = "Seller")]
//        public async Task<IActionResult> GetProfile()
//        {
//            ApiResponse response = new ApiResponse();


//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                response.status = 405;
//                response.errorMessage = "Error in the Request";
//                return Ok(response);
//            }
//            var userData = new
//            {
//                user.Id,
//                user.city,
//                user.FullName,
//                user.Email,
//                user.PhoneNumber,
//                user.ProfileImage,


//            };
//            response.data = userData;
//            response.status = "200";
//            return Ok(response);
//        }
//        [HttpPost("EditProfile")]
//        [Authorize]
//        public async Task<ApiResponse> EditProfile([FromForm] ApplicationUser Auser, [FromForm] List<IFormFile>? File1)
//        {
//            BL bl = new BL();
//            ApiResponse response = new ApiResponse();
//            var user = await _userManager.GetUserAsync(User);


//            if (user == null)
//            {
//                response.errorMessage = "user not found";
//                return response;
//            }


//            Auser.ProfileImage = await bl.TryUpload(File1, "ProfileImg", Auser.ProfileImage);


//            var result = await _userManager.UpdateAsync(Auser);

//            if (result.Succeeded)
//            {
//                response.status = 200;
//            }
//            else
//            {
//                response.errorMessage = "UnExpected Error";
//            }


//            return response;
//        }
//        string GenerateToken(IdentityUser user)
//        {
//            var claims = new[]
//            {
//        new Claim(ClaimTypes.NameIdentifier, user.Id),
//        new Claim(ClaimTypes.Email, user.Email),
//        new Claim(ClaimTypes.Name, user.UserName)
//        };

//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: configuration["JwtSettings:Issuer"],
//                audience: configuration["JwtSettings:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddHours(2),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }

//}

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
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace test1.APIController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        /*        [HttpPost("Register")]
                [AllowAnonymous]
                public async Task<IActionResult> Register([FromForm] Register model, [FromForm] List<IFormFile>? File1)
                {
                    // التحقق من صحة النموذج
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "Invalid input data",
                            Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                        });
                    }

                    // التحقق من وجود المستخدم مسبقاً
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        return Conflict(new
                        {
                            Status = 409,
                            Message = "The email address is already registered"
                        });
                    }

                    // إنشاء مستخدم جديد
                    var user = new ApplicationUser
                    {
                        FullName = model.Name,
                        PhoneNumber = model.Number,
                        Email = model.Email,
                        UserName = $"{model.Name.Replace(" ", "_")}_{Guid.NewGuid().ToString()[..8]}",
                        city = model.City,
                        IsActive = true // يمكنك تعيين القيمة الافتراضية هنا
                    };

                    try
                    {
                        // إنشاء الحساب
                        var result = await _userManager.CreateAsync(user, model.Password);

                        if (!result.Succeeded)
                        {
                            return BadRequest(new
                            {
                                Status = 400,
                                Message = "User creation failed",
                                Errors = result.Errors.Select(e => e.Description)
                            });
                        }

                        // رفع صورة الملف الشخصي إذا وجدت
                        if (File1 != null && File1.Count > 0)
                        {
                            var uploadResult = await _imageService.UploadImageAsync(File1[0], "ProfileImg");
                            if (uploadResult.Success)
                            {
                                user.ProfileImage = uploadResult.FilePath;
                                await _userManager.UpdateAsync(user);
                            }
                        }

                        // تعيين دور "Seller" للمستخدم
                        await _userManager.AddToRoleAsync(user, "Seller");

                        // إنشاء وتوقيع Token
                        var token = await GenerateToken(user);

                        return CreatedAtAction(nameof(Register), new
                        {
                            Status = 201,
                            Data = new
                            {
                                Token = token,
                                UserId = user.Id,
                                IsAdmin = false // يمكنك التحقق من الأدوار إذا لزم الأمر
                            },
                            Message = "User registered successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        // تسجيل الخطأ
                        _logger.LogError(ex, "Error during user registration");

                        return StatusCode(500, new
                        {
                            Status = 500,
                            Message = "An error occurred while processing your request",
                            DetailedError = _env.IsDevelopment() ? ex.Message : null
                        });
                    }
                }*/
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ApiResponse> Register([FromForm] Register model, [FromForm] List<IFormFile>? File1)
        {
            BL bl = new BL();
            ApiResponse response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.status = 405;
                response.errorMessage = "مدخلات غير صالحة";
                return response;
            }

            ApplicationUser user = new ApplicationUser()
            {
                FullName = model.Name,
                PhoneNumber = model.Number,
                Email = model.Email,
                UserName = $"{model.Name}_{Guid.NewGuid().ToString().Substring(0, 8)}",
                city = model.City
            };
            var CheckUser = await _userManager.FindByEmailAsync(model.Email);
            if (CheckUser != null)
            {
                response.errorMessage = "البريد الإلكتروني مستخدم بالفعل";
                return response;
            }
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (File1 != null)
                    {
                        user.ProfileImage = await bl.UploadImage(File1);
                        await _userManager.UpdateAsync(user);
                    }

                    await _userManager.AddToRoleAsync(user, "Seller");

                    response.data = GenerateToken(user);
                    response.IsAdmin = false;
                    response.status = "201";
                }
                else
                {
                    response.status = "400";
                    response.errorMessage = "المستخدم موجود بالفعل";
                }
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;

                // تنظيف الصورة المرفوعة في حال فشل الإنشاء
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    await bl.DeleteImageAsync(user.ProfileImage);
                }
            }

            return response;
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<ApiResponse> LogIn([FromBody] LogInModel model)
        {
            ApiResponse response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.status = 405;
                response.errorMessage = "مدخلات غير صالحة";
                return response;
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                response.status = 405;
                response.errorMessage = "خطأ في البريد الإلكتروني أو كلمة المرور";
                return response;
            }
            if (user.IsActive == false)
            {
                response.errorMessage = "هذا المستخدم محظور من استخدام الموقع";
                return response;
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                response.data = await GenerateToken(user);
                response.IsAdmin = true;
            }
            else
            {
                response.data = await GenerateToken(user);
                response.IsAdmin = false;
            }
            response.status = "200";
            return response;
        }

        [HttpGet("GetProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProfile()
        {
            ApiResponse response = new ApiResponse();
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                response.status = 405;
                response.errorMessage = "خطأ في الطلب";
                return Ok(response);
            }

            var userData = new
            {
                user.Id,
                user.city,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.ProfileImage
            };

            response.data = userData;
            response.status = "200";
            return Ok(response);
        }

        [HttpPost("EditProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ApiResponse> EditProfile([FromForm] ApplicationUser Auser, [FromForm] List<IFormFile>? File1)
        {
            BL bl = new BL();
            ApiResponse response = new ApiResponse();
            var user = await _userManager.GetUserAsync(User);
            try
            {
                if (user == null)
                {
                    response.errorMessage = "المستخدم غير موجود";
                    return response;
                }

                Auser.ProfileImage = await bl.TryUpload(File1, Auser.ProfileImage);
                var result = await _userManager.UpdateAsync(Auser);

                if (result.Succeeded)
                {
                    response.status = 200;
                }
                else
                {
                    response.errorMessage = "خطأ غير متوقع";
                }
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }

            return response;
        }
        private async Task<string> GenerateToken(ApplicationUser user)
        {
            // الحصول على أدوار المستخدم (مثل "Admin")
            var userRoles = await _userManager.GetRolesAsync(user); // تحتاج لجعل الدالة async

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName)
    };

            // إضافة أدوار المستخدم كـ Claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // ClaimTypes.Role مهم للصلاحيات
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
  
    }
}
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
    public class ADController : ControllerBase
    {
        IAd ClsAd;
        public ADController(IAd ad)
        {
            ClsAd = ad;
        }
   
        [HttpGet("ShowAd")]
        [AllowAnonymous]
        public ApiResponse Show()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.Show();
                response.status = 200;
                return response;
            }
            catch (Exception ex) 
            {response.errorMessage = ex.Message;
                return response;

            }
           
        }
        [HttpPost("Hit/{id}")]
        [AllowAnonymous]
        public void Hit(int id)
        {
            ClsAd.Click(id);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using test1.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace test1.Bl
{
    public interface IAd
    {
        string Click(int Id);
        TbAd GetAdById(int Id);
        Task<string> CreateAd(TbAd ad, List<IFormFile> File1);
        Task<string> Delete(int id);
        List<TbAd> Show();
    }

    public class ClsAd : IAd
    {
        private readonly RentCarContext _context;
  

        public ClsAd(RentCarContext context, ILogger<ClsAd> logger)
        {
            _context = context;
          
        }

        public string Click(int Id)
        {
            try
            {
                var ad = GetAdById(Id);
                if (ad == null || ad.Status != "Available" || ad.EndDate < DateTime.Now)
                {
                    return "الاعلان غير متوفر ";
                  
                }

                ad.Hit++;
                _context.Entry(ad).State = EntityState.Modified;
                _context.SaveChanges();
                return "تم";
            }
            catch (Exception ex)
            {
                
                return ex.Message;
            }
        }

        public TbAd GetAdById(int Id)
        {
            try
            {
                return _context.TbAds
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == Id &&
                                        a.Status == "Available" &&
                                        a.EndDate >= DateTime.Now);
            }
            catch (Exception ex)
            {
             
                return new TbAd();
            }
        }

        public async Task<string> CreateAd(TbAd ad, List<IFormFile> File1)
        {
            BL bl = new BL();
            if (ad == null)
            {
                return ("كائن الإعلان فارغ");
              
            }

            if (File1 == null || !File1.Any())
            {
                return ("لم يتم توفير ملفات لإنشاء الإعلان");
             
            }

            try
            {
                ad.Status = "Available";
                ad.CreateDate = DateTime.Now;
                ad.ImgName = await bl.UploadImage(File1);

                await _context.TbAds.AddAsync(ad);
                await _context.SaveChangesAsync();
                return "Done";
            }
            catch (Exception ex)
            {
                

                if (!string.IsNullOrEmpty(ad.ImgName))
                {
                    await bl.DeleteImageAsync(ad.ImgName);
                }

                return ex.Message;
            }
        }

        public async Task<string> Delete(int id)
        {
            try
            {
                var ad = GetAdById(id);
                if (ad == null || ad.Status== "Deleted")
                {
                    return "الاعلان غير موجود";
     
                }

                ad.Status = "Deleted";
                _context.Entry(ad).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return "تم الحذف بنجاح";
            }
            catch (Exception ex)
            {
              return ex.Message;
            }
        }

        public List<TbAd> Show()
        {
            try
            {
                return _context.TbAds
                    .AsNoTracking()
                    .Where(a => a.Status == "Available" && a.EndDate >= DateTime.Now)
                    .ToList();
            }
            catch (Exception ex)
            {
           
                return new List<TbAd>();
            }
        }
    }
}
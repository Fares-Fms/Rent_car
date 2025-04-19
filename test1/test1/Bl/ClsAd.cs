using Microsoft.EntityFrameworkCore;
using test1.Models;

namespace test1.Bl
{
   public interface IAd
    {
        public bool Click(int Id);
        public TbAd GetAdById(int Id);
        public  Task<bool> CreateAd(TbAd ad, List<IFormFile> File1);
        public Task<bool> Delete(int id);
        public List<TbAd> Show();

    }
    public class ClsAd : IAd
    {
        RentCarContext context;
        public ClsAd(RentCarContext carContext)
        {
            context = carContext;
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var Ad=GetAdById(id);
                Ad.Status = "Deleted";
                context.Entry(Ad).State = EntityState.Modified;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async  Task<bool> CreateAd(TbAd ad, List<IFormFile> File1)
        {
            BL bl=new BL();
            try
            {
             
                    ad.Status = "Available";
                    ad.CreateDate = DateTime.Now;
                    ad.ImgName = await bl.UploadImage(File1, "Ad");
                    context.TbAds.Add(ad); 
                    await context.SaveChangesAsync();
                    return true;                    
            }
            catch (Exception ex)
            {
                return false;
            }
        }
            public TbAd GetAdById(int Id)
        {
            try
            {

                var ad = context.TbAds.FirstOrDefault(a => (a.Id == Id) && (a.EndDate != DateTime.Now)&&(a.Status=="Available"));
                return ad;
            }
            catch
            {
                return new TbAd();
            }
        }
            public bool Click(int Id)
        {
            try
            {

                var ad = GetAdById(Id);
                ad.Hit++;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<TbAd> Show()
        {
            try
            {
                var ad = context.TbAds.Where(a => a.Status == "Available" && a.EndDate>=DateTime.Now).ToList();
                return ad;
            }
            catch { return new List<TbAd>(); }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using test1.Models;

namespace test1.Bl
{
   public interface Isettings
    {
        public TbSetting Show();
        public Task<bool> Edit(TbSetting setting);

    }
    public class ClsSettings: Isettings
    {
        RentCarContext context;
        public ClsSettings(RentCarContext carContext)
        {
            context = carContext;
        }
        public TbSetting Show()
        {
            try
            {
                var settings = context.TbSettings.FirstOrDefault();
                return settings;

            }
            catch (Exception ex)
            {
                return new TbSetting();
            }
        }
        public async Task<bool> Edit(TbSetting setting)
        { 
            try
            {
                var settings = context.TbSettings.FirstOrDefault();
                context.Entry(setting).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}

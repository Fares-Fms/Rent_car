using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using test1.Models;

namespace test1.Bl
{
    public interface ICar
    {
        public TbCar ShowCar(int Id);
        public List<TbCar> ShowALLCars(string? Id);
        public List<PageCarModel> ShowEightCars(int Counter, string? userId);
        public List<TbCar> ShowLatestCars();
        public int Count(string? userId);
        public List<TbCar> ShowFilteredCars(FilterCarModel car);
        public string DeleteCar(int Id);
        public String Sold(int id,string userid);
     

    }
    public class ClsCars : ICar
    {
        RentCarContext context;
        IReview ClsReview;
        public ClsCars(RentCarContext _context,IReview review)
        {
            context = _context;
            ClsReview = review;
        }
        public TbCar ShowCar(int Id)
        {
            try
            {

                var cars = context.TbCars.FirstOrDefault(a =>( a.Id == Id)&&(a.Status!="Deleted"));
                return cars;
            }
            catch
            {
                return new TbCar();
            }
        }
        public List<TbCar> ShowALLCars(string? Id)
        {
            try
            {
                List<TbCar> LstCars = new List<TbCar>();
                if (Id.IsNullOrEmpty())
                {
               LstCars = context.TbCars.Where(a => a.Status != "Deleted").ToList();                    
                }
                else
                {
                   LstCars= context.TbCars.Where(a => a.Status != "Deleted" && a.UserId==Id).ToList();
                }
                return LstCars;
            }

            catch
            {
                return new List<TbCar>();
            }
        }
        public List<TbCar> ShowFilteredCars(FilterCarModel car)
        {
            try
            {
                var query = context.TbCars.AsQueryable();

                if (!string.IsNullOrEmpty(car.Transmission))
                    query = query.Where(a => a.Transmission.Contains( car.Transmission));

                if (!string.IsNullOrEmpty(car.Fuel))
                    query = query.Where(a => a.Fuel.Contains(car.Fuel));

                if (!string.IsNullOrEmpty(car.Model))
                    query = query.Where(a => a.Model.Contains(car.Model));

                if (car.Year != null)
                    query = query.Where(a => a.Year == car.Year);

                if (!string.IsNullOrEmpty(car.Engine))
                    query = query.Where(a => a.Engine.Contains(car.Engine));

                if (!string.IsNullOrEmpty(car.Brand))
                    query = query.Where(a => a.Brand.Contains(car.Brand));

                if (!string.IsNullOrEmpty(car.Color))
                    query = query.Where(a => a.Color.Contains(car.Color));

                if (car.KiloMetrage != null)
                {
                    var min = car.KiloMetrage.Value - 5000;
                    var max = car.KiloMetrage.Value + 5000;
                    query = query.Where(a => a.KiloMetrage >= min && a.KiloMetrage <= max);
                }

                return query.ToList();
            }
            catch
            {
                return new List<TbCar>();
            }
        }
        public string DeleteCar(int Id)
        {
            try
            {
                var car = ShowCar(Id);
                car.Status = "Deleted";
                context.Entry(car).State = EntityState.Modified;
                context.SaveChanges();
                return "تم حذف السيارة ";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<PageCarModel> ShowEightCars(int Counter, string? userId)
        {
            try
            {
                Counter = (Counter - 1) * 6;
                List<PageCarModel> carsWithReviews = new List<PageCarModel>();


                if (userId.IsNullOrEmpty())
                {
                    carsWithReviews = context.TbCars
    .Where(a => a.Status != "Deleted" ).
   Select(t => new PageCarModel
   {
       Id = t.Id,
       Brand = t.Brand,
       Model = t.Model,
       Transmission = t.Transmission,
       Status = t.Status,
       City = t.City,
       Color = t.Color,
       Engine = t.Engine,
       CreateDate = t.CreateDate,
       KiloMetrage = t.KiloMetrage,
       Img1 = t.Img1,
       Comments = context.TbReviews.Count(r => r.CarId == t.Id)
   }).Take(8).Skip(Counter).ToList();
       

                }
                else
                {
                   carsWithReviews = context.TbCars
      .Where(t => t.Status != "Deleted" && t.UserId == userId)
      .Select(t => new PageCarModel
      {
          Id = t.Id,
          Brand = t.Brand,
          Model = t.Model,
          Transmission = t.Transmission,
          Status = t.Status,
          City = t.City,
          Color = t.Color,
          Engine = t.Engine,
          CreateDate = t.CreateDate,
          KiloMetrage = t.KiloMetrage,
          Img1 = t.Img1,
          Comments = context.TbReviews.Count(r => r.CarId == t.Id),
          UserId=t.UserId
      }).Take(8).Skip(Counter).ToList();


                }

                return carsWithReviews;
            }
            catch
            {
                return new List<PageCarModel>();
            }
        }

        public int Count(string? userId)
        {
            try
            {
                int LstCars;
                if (userId.IsNullOrEmpty())
                {
                    LstCars = context.TbCars.Where(a => a.Status != "Deleted").Count();

                }
                else
                {

                 LstCars = context.TbCars.Where(a => a.Status != "Deleted" && a.UserId==userId).Count();
                }
                return LstCars;
            }
            catch
            {
                return 0;
            }
        }

        public List<TbCar> ShowLatestCars()
        {
            try
            {
                var LstCars = context.TbCars.Where(a => a.Status != "Deleted").OrderBy(a=>a.CreateDate).Take(10).ToList();
                return LstCars;
            }
            catch
            {
                return new List<TbCar>();
            }
        }

        public string Sold(int id,string userid)
        {
            try
            {
                var car = context.TbCars.FirstOrDefault(a => (a.Id == id) && (a.Status != "Deleted"));
                if (car.UserId==userid)
                {

                    car.Sold = 1;
                    context.SaveChanges();
                    return "تم تحديث الحالة";
                }
                else return "يجب ان تكون مالك السيارة";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    
    }
}

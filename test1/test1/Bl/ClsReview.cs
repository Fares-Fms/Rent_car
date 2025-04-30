using Microsoft.EntityFrameworkCore;
using test1.Models;

namespace test1.Bl
{
    public interface IReview
    {
        public List<TbReview> GetReviews(int? id);
       public List<TbReview> Get5Stars();
        public string AddReview(TbReview review, int carId);
        public string ApproveReview(int id);
        public string DeclineReview(int id);
        public int? Count();
        public List<TbReview> GetReviewsAdmin();

    }
    public class ClsReview : IReview
    {
        RentCarContext context;
        public ClsReview(RentCarContext carContext)
        {
            context = carContext;   
        }

        public string AddReview(TbReview review, int carId)
        {
            try
            {
                var car = context.TbCars.FirstOrDefault(c => c.Id == carId);
                if (car == null)
                {
                   
                    return "please comment on a car";
                }

                review.CarId = car.Id;
                review.CreatedDate = DateTime.Now;
                review.IsPublic = "OnHold";

                context.TbReviews.Add(review);
                context.SaveChanges();
                return "the message has been sent to the admin";
            }
            catch (Exception ex)
            {
 
                return ex.ToString();
            }
        }




        public string ApproveReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = "Approved";
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return "The message Has been approved";
            }
            catch(Exception ex) { return ex.ToString(); }
        }
        public string DeclineReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = "Declined";
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return "The message Has been Declined";
            }
            catch (Exception ex) { return ex.ToString(); }
        }

        public List<TbReview> Get5Stars()
        {
            try
            {
                var reviews=context.TbReviews.Where(a=>a.Stars>=4 && a.IsPublic=="Approved").Take(7).ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }

        public List<TbReview> GetReviews(int? id)
        {
            try
            {
                var reviews = context.TbReviews.Where(a => a.CarId== id && a.IsPublic == "Approved").ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }
        public List<TbReview> GetReviewsAdmin()
        {
            try
            {
                var reviews = context.TbReviews.Where(a => ( a.IsPublic == "Approved"||a.IsPublic=="OnHold")).Take(8).ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }
        public int? Count()
        {
            try
            {
                var Count = context.TbReviews.Where(a =>  a.IsPublic == "Approved" || a.IsPublic=="OnHold").Count();
                if (Count==0)
                {
                    return null;
                }
                return Count;
            }
            catch
            {
                return null ;
            }
        }
    }
}

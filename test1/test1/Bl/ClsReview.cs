using Microsoft.EntityFrameworkCore;
using test1.Models;

namespace test1.Bl
{
    public interface IReview
    {
        public List<TbReview> GetReviews(string? id);
       public List<TbReview> Get5Stars();
        public bool AddReview(TbReview review);
        public bool ApproveReview(int id);
        public bool DeclineReview(int id);
    }
    public class ClsReview : IReview
    {
        RentCarContext context;
        public ClsReview(RentCarContext carContext)
        {
            context = carContext;   
        }

        public bool AddReview(TbReview review)
        {
            try
            {
                var NewReview = context.TbReviews.Add(review);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

      

        public bool ApproveReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = 1;
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public bool DeclineReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = 2;
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public List<TbReview> Get5Stars()
        {
            try
            {
                var reviews=context.TbReviews.Where(a=>a.Stars>=4 && a.IsPublic==1).Take(7).ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }

        public List<TbReview> GetReviews(string? id)
        {
            try
            {
                var reviews = context.TbReviews.Where(a => a.UserId== id && a.IsPublic == 1).Take(5).ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }
    }
}

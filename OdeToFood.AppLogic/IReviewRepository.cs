using System.Collections.Generic;
using System.Threading.Tasks;
using OdeToFood.Domain;

namespace OdeToFood.AppLogic
{
    public interface IReviewRepository
    {
        Task<IReadOnlyList<Review>> GetAllAsync();
        Task<Review> GetByIdAsync(int id);
        Task<IReadOnlyList<Review>> GetReviewsByRestaurantAsync(int id);
        Task<Review> AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
    }
}
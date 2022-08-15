using OdeToFood.Domain;

namespace OdeToFood.Web.Models
{
    public interface IConverter
    {
        Review ConvertEditReviewViewModelToReview(EditReviewViewModel model);
    }
}

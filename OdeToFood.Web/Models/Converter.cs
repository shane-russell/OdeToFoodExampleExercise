using OdeToFood.Domain;

namespace OdeToFood.Web.Models
{
    public class Converter: IConverter
    {
        public Review ConvertEditReviewViewModelToReview(EditReviewViewModel model)
        {
            Review review = new Review
            {
                Body = model.Body,
                Rating = model.Rating,
                RestaurantId = model.RestaurantId,
                ReviewerName = model.ReviewerName
            };
            return review;
        }
    }
}

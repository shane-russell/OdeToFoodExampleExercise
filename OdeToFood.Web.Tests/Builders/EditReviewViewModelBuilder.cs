using System;
using OdeToFood.Web.Models;

namespace OdeToFood.Web.Tests.Builders
{
    public class EditReviewViewModelBuilder
    {
        private readonly EditReviewViewModel _editReviewViewModel;

        public EditReviewViewModelBuilder()
        {
            var random = new Random();

            _editReviewViewModel = new EditReviewViewModel
            {
                Rating = random.Next(),
                Body = Guid.NewGuid().ToString(),
                RestaurantId = random.Next(),
                ReviewerName = Guid.NewGuid().ToString()
            };
        }
        public EditReviewViewModel Build()
        {
            return _editReviewViewModel;
        }
    }
}

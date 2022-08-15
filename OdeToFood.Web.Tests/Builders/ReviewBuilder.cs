using System;
using OdeToFood.Domain;

namespace OdeToFood.Web.Tests.Builders
{
    internal class ReviewBuilder
    {
        private readonly Review _review;
        private readonly Random _random;

        public ReviewBuilder()
        {
            _random = new Random();

            _review = new Review
            {
                ReviewerName = Guid.NewGuid().ToString(),
                Rating = _random.Next(1, 6)
            };
        }

        public ReviewBuilder WithId()
        {
            _review.Id = _random.Next();
            return this;
        }

        public ReviewBuilder WithRestaurantId(int restaurantId)
        {
            _review.RestaurantId = restaurantId;
            return this;
        }

        public ReviewBuilder WithReviewerName(string name)
        {
            _review.ReviewerName = name;
            return this;
        }

        public Review Build()
        {
            return _review;
        }

       
    }
}

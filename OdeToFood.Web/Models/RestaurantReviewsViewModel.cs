using System.Collections.Generic;
using OdeToFood.Domain;

namespace OdeToFood.Web.Models
{
    public class RestaurantReviewsViewModel
    {
        public Restaurant Restaurant { get; set; }
        public IReadOnlyList<Review> Reviews { get; set; }

    }
}

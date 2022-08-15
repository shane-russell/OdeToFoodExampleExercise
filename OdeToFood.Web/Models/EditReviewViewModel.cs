using System.ComponentModel.DataAnnotations;

namespace OdeToFood.Web.Models
{
    public class EditReviewViewModel
    {
        [Range(1, 10)]
        public int Rating { get; set; }

        public string Body { get; set; }

        public int RestaurantId { get; set; }

        [Required]
        public string ReviewerName { get; set; }
    }
}

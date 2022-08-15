using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdeToFood.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OdeToFood.AppLogic;
using OdeToFood.Domain;

namespace OdeToFood.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IConverter _converter;

        public HomeController(IRestaurantRepository restaurantRepository, IReviewRepository reviewRepository, IConverter converter)
        {
            _restaurantRepository = restaurantRepository;
            _reviewRepository = reviewRepository;
            _converter = converter;
        }

        public IActionResult Index()
        {
            var restaurants = _restaurantRepository.GetAll();
            return View(restaurants);
        }

        [HttpGet("Home/Details/{id}")]
        public async Task<ActionResult> Details(int id)
        {
            RestaurantReviewsViewModel viewModel = new RestaurantReviewsViewModel
            {
                Restaurant = _restaurantRepository.GetById(id),
                Reviews = await _reviewRepository.GetReviewsByRestaurantAsync(id)
            };
            return View(viewModel);
        }

        // GET: 
        [HttpGet("Home/AddReview/{restaurantId}")]
        public IActionResult AddReview(int restaurantId)
        {
            return View(new EditReviewViewModel { RestaurantId = restaurantId });
        }

        [HttpPost("Home/AddReview/{restaurantId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int restaurantId, EditReviewViewModel model)
        {
            if (model.RestaurantId != restaurantId)
            {
                ModelState.AddModelError("InvalidRestaurantId", "Restaurant id's in model and url don't match.");
            }

            if (ModelState.IsValid)
            {
                Review review = _converter.ConvertEditReviewViewModelToReview(model);

                await _reviewRepository.AddAsync(review);

                return RedirectToAction(nameof(Details), new { id = restaurantId });
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

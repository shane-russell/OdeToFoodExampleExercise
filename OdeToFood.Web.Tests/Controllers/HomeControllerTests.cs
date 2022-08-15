using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OdeToFood.AppLogic;
using OdeToFood.Domain;
using OdeToFood.Web.Controllers;
using OdeToFood.Web.Models;
using OdeToFood.Web.Tests.Builders;

namespace OdeToFood.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<IRestaurantRepository> _restaurantsRepositoryMock;
        private Mock<IReviewRepository> _reviewsRepositoryMock;
        private Mock<IConverter> _converterMock;
        private HomeController _controller;
        [SetUp]
        public void Setup()
        {
            _restaurantsRepositoryMock = new Mock<IRestaurantRepository>();
            _reviewsRepositoryMock = new Mock<IReviewRepository>();
            _converterMock = new Mock<IConverter>();
            _controller = new HomeController(_restaurantsRepositoryMock.Object, _reviewsRepositoryMock.Object, _converterMock.Object);
        }

        [Test]
        public void Index_ShouldReturnAListOfRestaurants()
        {
            var restaurants = new List<Restaurant>
            {
                new RestaurantBuilder().Build()
            };

            _restaurantsRepositoryMock.Setup(r => r.GetAll()).Returns(restaurants);

            var viewResult = _controller.Index() as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.SameAs(restaurants));
            _restaurantsRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Test]
        public void Details_ShouldReturnDetailsOfRestaurantAndReviews()
        {
            var restaurant = new RestaurantBuilder().WithId().Build();
            var reviews = new List<Review>
            {
                new ReviewBuilder().WithId().Build()
            };

            _restaurantsRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(restaurant);
            _reviewsRepositoryMock.Setup(review => review.GetReviewsByRestaurantAsync(It.IsAny<int>())).ReturnsAsync(reviews);

            var viewResult = _controller.Details(restaurant.Id).Result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            RestaurantReviewsViewModel model = viewResult.Model as RestaurantReviewsViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Restaurant, Is.SameAs(restaurant));
            Assert.That(model.Reviews, Is.SameAs(reviews));

            _restaurantsRepositoryMock.Verify(r => r.GetById(restaurant.Id), Times.Once);
            _reviewsRepositoryMock.Verify(r => r.GetReviewsByRestaurantAsync(restaurant.Id), Times.Once);
        }

        [Test]
        public void AddReview_ShouldReturnAViewWithAnEditModelContainingTheRestaurantId()
        {
            int restaurantId = new Random().Next(1, int.MaxValue);

            var viewResult = _controller.AddReview(restaurantId) as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            var viewModel = viewResult.Model as EditReviewViewModel;
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.RestaurantId, Is.EqualTo(restaurantId));
        }

        [Test]
        public void AddReview_ValidReviewPosted_ShouldPersistReviewAndRedirectToRestaurantDetails()
        {
            EditReviewViewModel viewModel = new EditReviewViewModelBuilder().Build();
            Review review = new ReviewBuilder().WithRestaurantId(viewModel.RestaurantId).Build();

            _converterMock.Setup(c => c.ConvertEditReviewViewModelToReview(It.IsAny<EditReviewViewModel>()))
                .Returns(review);

            _reviewsRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Review>())).ReturnsAsync(review);

            var redirectResult = _controller.AddReview(review.RestaurantId, viewModel).Result as RedirectToActionResult;

            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.Permanent, Is.False);
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(HomeController.Details)));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(review.RestaurantId));

            _converterMock.Verify(c => c.ConvertEditReviewViewModelToReview(viewModel), Times.Once);
            _reviewsRepositoryMock.Verify(r => r.AddAsync(review), Times.Once);
        }

        [Test]
        public void AddReview_InvalidModel_ShouldReturnTheSameView()
        {
            EditReviewViewModel viewModel = new EditReviewViewModelBuilder().Build();

            _controller.ModelState.AddModelError("someError", "Message");

            var viewResult = _controller.AddReview(viewModel.RestaurantId, viewModel).Result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.SameAs(viewModel));
            _reviewsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        }

        [Test]
        public void AddReview_RestaurantIdsDoNotMatch_ShouldReturnTheSameView()
        {
            EditReviewViewModel viewModel = new EditReviewViewModelBuilder().Build();

            var viewResult = _controller.AddReview(viewModel.RestaurantId + 1, viewModel).Result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.SameAs(viewModel));
            _reviewsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        }
    }
}

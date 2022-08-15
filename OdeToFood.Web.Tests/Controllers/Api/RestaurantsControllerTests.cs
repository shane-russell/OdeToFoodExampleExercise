using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OdeToFood.AppLogic;
using OdeToFood.Domain;
using OdeToFood.Web.Controllers.Api;
using OdeToFood.Web.Tests.Builders;

namespace OdeToFood.Web.Tests.Controllers.Api
{
    class RestaurantsControllerTests
    {
        private RestaurantsController _controller;
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Random _random = new Random();

        [SetUp]
        public void Setup()
        {
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _controller = new RestaurantsController(_restaurantRepositoryMock.Object);
        }

        [Test]
        public void GetAll_ReturnsAllRestaurantsFromRepository()
        {
            //Arrange
            var restaurants = new List<Restaurant>();
            _restaurantRepositoryMock.Setup(r => r.GetAll()).Returns(restaurants);

            //Act
            var okResult = _controller.GetAll() as OkObjectResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(restaurants));
            _restaurantRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Test]
        public void GetRestaurant_ReturnsRestaurantIfItExists()
        {
            //Arrange
            var restaurant = new RestaurantBuilder().WithId().Build();
            _restaurantRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(restaurant);

            //Act
            var okResult = _controller.GetRestaurant(restaurant.Id) as OkObjectResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.GetById(restaurant.Id), Times.Once);
            Assert.That(okResult.Value, Is.SameAs(restaurant));
        }

        [Test]
        public void GetRestaurant_ReturnsNotFoundIfItDoesNotExists()
        {
            //Arrange
            _restaurantRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(() => null);

            //Act
            int someId = _random.Next();
            var notFoundResult = _controller.GetRestaurant(someId) as NotFoundResult;

            //Assert
            Assert.That(notFoundResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.GetById(someId), Times.Once);
        }

        [Test]
        public void Post_ValidRestaurantIsSavedInRepository()
        {
            //Arrange
            var newRestaurant = new RestaurantBuilder().Build();

            _restaurantRepositoryMock.Setup(repo => repo.Add(It.IsAny<Restaurant>())).Returns(() =>
            {
                newRestaurant.Id = _random.Next();
                return newRestaurant;
            });

            //Act
            var createdResult = _controller.Post(newRestaurant) as CreatedAtActionResult;

            //Assert
            Assert.That(createdResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.Add(newRestaurant), Times.Once);
            Assert.That(createdResult.Value, Is.SameAs(newRestaurant));
            Assert.That(createdResult.ActionName, Is.EqualTo(nameof(RestaurantsController.GetRestaurant)));
            Assert.That(createdResult.RouteValues["id"], Is.EqualTo(((Restaurant)createdResult.Value).Id));
        }

        [Test]
        public void Post_InValidRestaurantCausesBadRequest()
        {
            //Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");

            //Act
            var badRequestResult = _controller.Post(new RestaurantBuilder().WithEmptyName().Build()) as BadRequestResult;

            //Assert
            Assert.That(badRequestResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(repo => repo.Add(It.IsAny<Restaurant>()), Times.Never);
        }

        [Test]
        public void Put_ExistingRestaurantIsSavedInRepository()
        {
            //Arrange
            var aRestaurant = new RestaurantBuilder().WithId().Build();

            _restaurantRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(aRestaurant);

            //Act
            var okResult = _controller.Put(aRestaurant.Id, aRestaurant) as OkResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.GetById(aRestaurant.Id), Times.Once);
            _restaurantRepositoryMock.Verify(r => r.Update(aRestaurant), Times.Once);
        }

        [Test]
        public void Put_NonExistingRestaurantReturnsNotFound()
        {
            //Arrange
            _restaurantRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(() => null);

            var aRestaurant = new RestaurantBuilder().WithId().Build();

            //Act
            var notFoundResult = _controller.Put(aRestaurant.Id, aRestaurant) as NotFoundResult;

            //Assert
            Assert.That(notFoundResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.GetById(aRestaurant.Id), Times.Once);
            _restaurantRepositoryMock.Verify(r => r.Update(It.IsAny<Restaurant>()), Times.Never);
        }

        [Test]
        public void Put_InValidRestaurantModelStateCausesBadRequest()
        {
            //Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");

            var aRestaurant = new RestaurantBuilder().WithEmptyName().Build();

            //Act
            var badRequestResult = _controller.Put(aRestaurant.Id, aRestaurant) as BadRequestResult;

            //Assert
            Assert.That(badRequestResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.Update(It.IsAny<Restaurant>()), Times.Never);
        }

        [Test]
        public void Put_MismatchBetweenUrlIdAndRestaurantIdCausesBadRequest()
        {
            //Arrange
            var aRestaurant = new RestaurantBuilder().WithId().Build();

            //Act
            var badRequestResult = _controller.Put(aRestaurant.Id + 1, aRestaurant) as BadRequestResult;

            //Assert
            Assert.That(badRequestResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.Update(It.IsAny<Restaurant>()), Times.Never);
        }

        [Test]
        public void Delete_ExistingRestaurantIsDeletedFromRepository()
        {
            //Arrange
            var aRestaurant = new RestaurantBuilder().WithId().Build();

            _restaurantRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(aRestaurant);

            //Act
            var okResult = _controller.Delete(aRestaurant.Id) as OkResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.GetById(aRestaurant.Id), Times.Once);
            _restaurantRepositoryMock.Verify(r => r.Delete(aRestaurant.Id), Times.Once);
        }

        [Test]
        public void Delete_NonExistingRestaurantReturnsNotFound()
        {
            //Arrange
            _restaurantRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(() => null);
            var someId = _random.Next();

            //Act
            var notFoundResult = _controller.Delete(someId) as NotFoundResult;

            //Assert
            Assert.That(notFoundResult, Is.Not.Null);
            _restaurantRepositoryMock.Verify(r => r.GetById(someId), Times.Once);
            _restaurantRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }
    }
}

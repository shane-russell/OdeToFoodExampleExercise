using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OdeToFood.AppLogic;
using OdeToFood.Domain;
using OdeToFood.Web.Controllers.Api;
using OdeToFood.Web.Tests.Builders;

namespace OdeToFood.Web.Tests.Controllers.Api
{
    [TestFixture]
    public class ReviewControllerTests
    {
        private ReviewsController _controller;
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Random _random = new Random();

        [SetUp]
        public void Setup()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _controller = new ReviewsController(_reviewRepositoryMock.Object);
        }

        [Test]
        public void Get_ReturnsAllReviewsFromRepository()
        {
            //Arrange
            var reviews = new List<Review>();
            _reviewRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(reviews);

            //Act
            var okResult = _controller.Get().Result as OkObjectResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
            Assert.That(okResult.Value, Is.SameAs(reviews));
        }

        [Test]
        public void Get_ReturnsReviewIfItExists()
        {
            //Arrange
            var review = new ReviewBuilder().WithId().Build();
            //_reviewRepositoryMock.Setup(r => r.GetByIdAsync(review.Id)).Returns(() => Task.FromResult(review));
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(review);

            //Act
            var okResult = _controller.Get(review.Id).Result as OkObjectResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetByIdAsync(review.Id), Times.Once);
            Assert.That(okResult.Value, Is.SameAs(review));
        }

        [Test]
        public void Get_ReturnsNotFoundIfReviewDoesNotExists()
        {
            //Arrange
            //_reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns(() => Task.FromResult<Review>(null));
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);
            var someReviewId = _random.Next();

            //Act
            var notFoundResult = _controller.Get(someReviewId).Result as NotFoundResult;

            //Assert
            Assert.That(notFoundResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetByIdAsync(someReviewId), Times.Once);
        }

        [Test]
        public void Post_ValidReviewIsSavedInRepository()
        {
            //Arrange
            var newReview = new ReviewBuilder().Build();
            _reviewRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Review>())).Returns(() =>
            {
                newReview.Id = _random.Next(1, int.MaxValue);
                return Task.FromResult(newReview);
            });

            //Act
            var createdResult = _controller.Post(newReview).Result as CreatedAtActionResult;

            //Assert
            Assert.That(createdResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.AddAsync(newReview), Times.Once);

            Assert.That(createdResult.Value, Is.SameAs(newReview));
            Assert.That(createdResult.ActionName, Is.EqualTo(nameof(ReviewsController.Get)));
            Assert.That(createdResult.RouteValues["id"], Is.EqualTo(newReview.Id));
        }

        [Test]
        public void Post_InValidReviewModelStateCausesBadRequest()
        {
            //Arrange
            _controller.ModelState.AddModelError("ReviewerName", "ReviewerName is required");

            var newReview = new ReviewBuilder().WithReviewerName(null).Build();

            //Act
            var badRequestResult = _controller.Post(newReview).Result as BadRequestResult;

            //Assert
            Assert.That(badRequestResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Review>()), Times.Never);
        }

        [Test]
        public void Put_ExistingReviewIsSavedInRepository()
        {
            //Arrange
            var aReview = new ReviewBuilder().WithId().Build();
            //_reviewRepositoryMock.Setup(r => r.GetByIdAsync(aReview.Id)).Returns(() => Task.FromResult<Review>(aReview));
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(aReview);

            //Act
            var okResult = _controller.Put(aReview.Id, aReview).Result as OkResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetByIdAsync(aReview.Id), Times.Once);
            _reviewRepositoryMock.Verify(r => r.UpdateAsync(aReview), Times.Once);
        }

        [Test]
        public void Put_NonExistingReviewReturnsNotFound()
        {
            //Arrange
            //_reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns(() => Task.FromResult<Review>(null));
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            var aReview = new ReviewBuilder().WithId().Build();

            //Act
            var notFoundResult = _controller.Put(aReview.Id, aReview).Result as NotFoundResult;

            //Assert
            Assert.That(notFoundResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetByIdAsync(aReview.Id), Times.Once);
            _reviewRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Review>()), Times.Never);
        }

        [Test]
        public void Put_InValidReviewModelStateCausesBadRequest()
        {
            //Arrange
            _controller.ModelState.AddModelError("ReviewerName", "ReviewerName is required");

            var aReview = new ReviewBuilder().WithReviewerName(null).Build();

            //Act
            var badRequestResult = _controller.Put(aReview.Id, aReview).Result as BadRequestResult;

            //Assert
            Assert.That(badRequestResult, Is.Not.Null);
        }

        [Test]
        public void Put_MismatchBetweenUrlIdAndReviewIdCausesBadRequest()
        {
            //Arrange
            var aReview = new ReviewBuilder().WithId().Build();

            //Act
            var badRequestResult = _controller.Put(aReview.Id + 1, aReview).Result as BadRequestResult;

            //Assert
            Assert.That(badRequestResult, Is.Not.Null);
        }

        [Test]
        public void Delete_ExistingReviewIsDeletedFromRepository()
        {
            //Arrange
            var aReview = new ReviewBuilder().WithId().Build();
            //_reviewRepositoryMock.Setup(r => r.GetByIdAsync(aReview.Id)).Returns(() => Task.FromResult(aReview));
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(aReview);

            //Act
            var okResult = _controller.Delete(aReview.Id).Result as OkResult;

            //Assert
            Assert.That(okResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetByIdAsync(aReview.Id), Times.Once);
            _reviewRepositoryMock.Verify(r => r.DeleteAsync(aReview.Id), Times.Once);
        }

        [Test]
        public void Delete_NonExistingReviewReturnsNotFound()
        {
            //Arrange
            //_reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).Returns(() => Task.FromResult<Review>(null));
            _reviewRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            var aReview = new ReviewBuilder().WithId().Build();

            //Act
            var notFoundResult = _controller.Delete(aReview.Id).Result as NotFoundResult;

            //Assert
            Assert.That(notFoundResult, Is.Not.Null);
            _reviewRepositoryMock.Verify(r => r.GetByIdAsync(aReview.Id), Times.Once);
            _reviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
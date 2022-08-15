using NUnit.Framework;
using OdeToFood.Web.Models;
using OdeToFood.Web.Tests.Builders;

namespace OdeToFood.Web.Tests
{
    [TestFixture]
    public class ConverterTests
    {
        private Converter _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new Converter();
        }

        [Test]
        public void ConvertEditReviewViewModelToReview_ValidModel_CorrectlyMapped()
        {
            //Arrange
            EditReviewViewModel model = new EditReviewViewModelBuilder().Build();

            //Act
            var review = _converter.ConvertEditReviewViewModelToReview(model);

            //Assert
            Assert.That(review, Is.Not.Null);
            Assert.That(review.ReviewerName, Is.EqualTo(model.ReviewerName));
            Assert.That(review.Rating, Is.EqualTo(model.Rating));
            Assert.That(review.RestaurantId, Is.EqualTo(model.RestaurantId));
            Assert.That(review.Body, Is.EqualTo(model.Body));
        }
    }
}

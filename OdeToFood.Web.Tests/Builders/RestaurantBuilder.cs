using System;
using OdeToFood.Domain;

namespace OdeToFood.Web.Tests.Builders
{
    internal class RestaurantBuilder
    {
        private readonly Restaurant _restaurant;
        private static readonly Random Random = new Random();

        public RestaurantBuilder()
        {
            _restaurant = new Restaurant
            {
                Name = Guid.NewGuid().ToString(),
            };
        }

        public RestaurantBuilder WithId()
        {
            _restaurant.Id = Random.Next();
            return this;
        }

        public RestaurantBuilder WithEmptyName()
        {
            _restaurant.Name = null;
            return this;
        }

        public Restaurant Build()
        {
            return _restaurant;
        }
    }
}
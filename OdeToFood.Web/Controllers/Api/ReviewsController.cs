using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdeToFood.AppLogic;
using OdeToFood.Domain;

namespace OdeToFood.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _reviewRepository.GetAllAsync());
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null) return NotFound();

            return Ok(review);
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Review newReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdReview = await _reviewRepository.AddAsync(newReview);

            return CreatedAtAction(nameof(Get), routeValues: new {id = createdReview.Id}, value: createdReview);
        }

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id != review.Id)
            {
                return BadRequest();
            }

            if (await _reviewRepository.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            await _reviewRepository.UpdateAsync(review);

            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _reviewRepository.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            await _reviewRepository.DeleteAsync(id);

            return Ok();
        }
    }
}
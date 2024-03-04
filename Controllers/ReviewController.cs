using events.Data;
using events.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly ApiContext _context;
		public ReviewController(ApiContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Добавление отзыва на мероприятие
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="eventid"></param>
		/// <param name="evaluation"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		[HttpPost("AddEventReview/{userid}/{eventid}/{evaluation}/{description}")]
		public IActionResult AddEventReview(EventReview eventReview)
		{
			EventReview eventReview = new EventReview()
			{
				Evaluation = eventReview.Evaluation,
				Description = eventReview.Description,
				EventId = eventReview.EventId,
				UserId = eventReview.UserId
            };
			_context.EventReviews.Add(eventReview);
			_context.SaveChanges();

			return new JsonResult(Ok(eventReview));
		}

		[HttpDelete("Delete/{reviewid}")]
		public JsonResult Delete(int reviewid)
		{
			try
			{
				var review = _context.EventReviews.First(q => q.Id == reviewid);

				_context.EventReviews.Remove(review);
				_context.SaveChanges();

				return new JsonResult(Ok());
			}
			catch
			{
                return new JsonResult(BadRequest());
            }
		}
	}
}

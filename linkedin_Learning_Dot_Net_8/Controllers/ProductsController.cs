using linkedin_Learning_Dot_Net_8.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_Learning_Dot_Net_8.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly ShopContext _shopContext;
		private readonly ILogger<ProductsController> _logger;
        public ProductsController(ShopContext shopContext,ILogger<ProductsController> logger)
        {
			_shopContext = shopContext;
			_logger= logger;
		/*
		 * Ensures that the database for the ShopContext is created.
		 * This method checks if the database exists, and if not, it creates it.
		 * It's often used in development or testing environments to set up the database
		 * without needing to run migrations.
		 */
			_shopContext.Database.EnsureCreated();

		}

		/* Uses asynchronous programming for better performance.
		   Includes basic error handling.
		   Returns an ActionResult, providing more flexibility in the HTTP response.*/
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
		{
			try
			{
		    /* This approach is more efficient, especially in I/O-bound operations like database calls,
		      as it frees up the thread to handle other requests while waiting for the data to be fetched. */
				var products = await _shopContext.Products.ToListAsync();
				return Ok(products);
			}
			catch (Exception ex)
			{
				// Log the exception
				_logger.LogError(ex, "An error occurred while retrieving products.");
				return StatusCode(500, "Internal server error");
			}
		}

	


	}
}

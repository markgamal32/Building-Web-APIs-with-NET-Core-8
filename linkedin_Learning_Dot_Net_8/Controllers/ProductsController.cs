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


		[HttpGet("{id:int}")]
		public async Task<ActionResult<Product>> GetProductById(int id)
		{
			try
			{
				var product = await _shopContext.Products.FindAsync(id);

				if (id <= 0 || product == null)
				{
					return id <= 0
						? BadRequest("Invalid ID. ID must be a positive integer.")
						: NotFound($"Product with ID {id} not found.");
				}

				return Ok(product);
			}
			catch (DbUpdateException dbEx)
			{
				_logger.LogError(dbEx, $"A database error occurred while retrieving product with ID {id}.");
				return StatusCode(500, "A database error occurred.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while retrieving product with ID {id}.");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		public async Task<ActionResult<Product>> AddProduct([FromBody] Product newProduct)
		{
			//  validation checks 
			if (newProduct == null ||
				string.IsNullOrWhiteSpace(newProduct.Name) ||
				newProduct.Price <= 0 || newProduct.Id <= 0)
			{
				// Determine which specific validation failed and return an appropriate response
				if (newProduct == null)
				{
					return BadRequest("Product data is required.");
				}

				if (string.IsNullOrWhiteSpace(newProduct.Name))
				{
					return BadRequest("Product name is required.");
				}

				if (newProduct.Price <= 0)
				{
					return BadRequest("Product price must be greater than zero.");
				}
				if (newProduct.Id <= 0)
				{
					return BadRequest("Product ID must be a positive integer.");
				}
			}
			try
			{
				// Add the new product to the database
				_shopContext.Products.Add(newProduct);
				await _shopContext.SaveChangesAsync();

				// Return the newly created product with a 201 Created status
				return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
			}
			catch (DbUpdateException dbEx)
			{
				// Log database-specific errors
				_logger.LogError(dbEx, "A database error occurred while adding a new product.");
				return StatusCode(500, "A database error occurred.");
			}
			catch (Exception ex)
			{
				// Log general errors
				_logger.LogError(ex, "An error occurred while adding a new product.");
				return StatusCode(500, "Internal server error");
			}
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
		{
			//  validation checks 
			if (updatedProduct == null ||
				string.IsNullOrWhiteSpace(updatedProduct.Name) ||
				updatedProduct.Price <= 0 || updatedProduct.Id != id)
			{
				// Determine which specific validation failed and return an appropriate response
				if (updatedProduct == null)
				{
					return BadRequest("Product data is required.");
				}

				if (string.IsNullOrWhiteSpace(updatedProduct.Name))
				{
					return BadRequest("Product name is required.");
				}

				if (updatedProduct.Price <= 0)
				{
					return BadRequest("Product price must be greater than zero.");
				}

				if (updatedProduct.Id != id)
				{
					return BadRequest("Product ID in the body does not match the URL.");
				}
			}

			try
			{
				var existingProduct = await _shopContext.Products.FindAsync(id);

				if (existingProduct == null)
				{
					return NotFound("Product not found.");
				}

				// Update the existing product's properties
				existingProduct.Name = updatedProduct.Name;
				existingProduct.Price = updatedProduct.Price;
				existingProduct.Sku = updatedProduct.Sku;
				existingProduct.Description = updatedProduct.Description;
				existingProduct.IsAvailable = updatedProduct.IsAvailable;
				existingProduct.CategoryId = updatedProduct.CategoryId;

				await _shopContext.SaveChangesAsync();

				return NoContent(); // Return 204 No Content for a successful update with no response body
			}
			catch (DbUpdateException dbEx)
			{
				// Log database-specific errors
				_logger.LogError(dbEx, "A database error occurred while updating the product.");
				return StatusCode(500, "A database error occurred.");
			}
			catch (Exception ex)
			{
				// Log general errors
				_logger.LogError(ex, "An error occurred while updating the product.");
				return StatusCode(500, "Internal server error");
			}
		}



		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			try
			{
				var product = await _shopContext.Products.FindAsync(id);

				if (product == null)
				{
					return NotFound("Product not found.");
				}

				// Remove the product from the database
				_shopContext.Products.Remove(product);
				await _shopContext.SaveChangesAsync();

				return Ok(product); // Return 200 OK for a Successful deletion with the Deleted Product
			}
			catch (DbUpdateException dbEx)
			{
				// Log database-specific errors
				_logger.LogError(dbEx, "A database error occurred while deleting the product.");
				return StatusCode(500, "A database error occurred.");
			}
			catch (Exception ex)
			{
				// Log general errors
				_logger.LogError(ex, "An error occurred while deleting the product.");
				return StatusCode(500, "Internal server error");
			}
		}


		//Deleting several Products
		[HttpDelete("batch")]
		public async Task<IActionResult> DeleteProducts([FromBody] List<int> productIds)
		{
			if (productIds == null || !productIds.Any())
			{
				return BadRequest("A list of product IDs is required.");
			}

			try
			{
				// Fetch the products that match the provided IDs
				var products = await _shopContext.Products
					.Where(p => productIds.Contains(p.Id))
					.ToListAsync();

				if (products.Count == 0)
				{
					return NotFound("No matching products found.");
				}

				// Remove the products from the database
				_shopContext.Products.RemoveRange(products);
				await _shopContext.SaveChangesAsync();

				return NoContent(); // Return 204 No Content for a successful deletion with no response body
			}
			catch (DbUpdateException dbEx)
			{
				// Log database-specific errors
				_logger.LogError(dbEx, "A database error occurred while deleting the products.");
				return StatusCode(500, "A database error occurred.");
			}
			catch (Exception ex)
			{
				// Log general errors
				_logger.LogError(ex, "An error occurred while deleting the products.");
				return StatusCode(500, "Internal server error");
			}
		}


	}
}

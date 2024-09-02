using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace linkedin_Learning_Dot_Net_8.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Sku { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public bool IsAvailable { get; set; } = false;


		[Required]
		public int CategoryId { get; set; }
		[JsonIgnore]
		public virtual Category? Category { get; set; }
	}
}

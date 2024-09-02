﻿namespace linkedin_Learning_Dot_Net_8.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;



		public virtual List<Product> Products { get; set; }= new List<Product>();
	}
}

﻿namespace linkedin_Learning_Dot_Net_8.Models
{
	public class ProductQueryParameters : QueryParameters
	{
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
	}
}
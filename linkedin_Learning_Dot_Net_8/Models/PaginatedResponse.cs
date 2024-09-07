namespace linkedin_Learning_Dot_Net_8.Models
{
	public class PaginatedResponse<T>
	{
		public List<T> Data { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalRecords { get; set; }
		public int TotalPages { get; set; }
	}
}

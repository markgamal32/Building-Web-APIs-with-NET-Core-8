namespace linkedin_Learning_Dot_Net_8.Models
{
	public class QueryParameters
	{
		//This means a user cannot request more than 100 items per page,
		//which helps prevent performance issues caused by excessively large requests.
		const int _maxSize = 100;
		private int _size = 50;

		public int Page { get; set; } = 1;

		public int Size
		{
			get { return _size; }
			set
			{
				_size = Math.Min(_maxSize, value);
			}
		}

	}
}

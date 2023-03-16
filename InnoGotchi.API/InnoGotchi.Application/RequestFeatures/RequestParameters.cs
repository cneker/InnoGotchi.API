namespace InnoGotchi.Application.RequestFeatures
{
    public abstract class RequestParameters
    {
        private readonly int _maxPageSize = 15;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
        }

        public string OrderBy { get; set; }
    }
}

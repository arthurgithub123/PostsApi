using System.Linq;

namespace PostsApi.Models.Pagination
{
    public class PaginationResponse<T> where T : class
    {
        public int TotalRecords { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
        public IQueryable<T> Data { get; set; }
    }
}

namespace PostsApi.Models.Pagination
{
    public class PaginationQueryParams
    {
        public PaginationQueryParams()
        {
            if(Page <= 0) Page = 1;

            if(Per_Page <= 0) Per_Page = 10;
            
            if(Per_Page > 100) Per_Page = 100;
        }

        public int Page { get; set; }

        public int Per_Page { get; set; }
    }
}

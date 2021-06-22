using System;
using System.Linq;

namespace PostsApi.Models.Pagination
{
    public class PaginationHelper<T> where T : class
    {
        public static PaginationResponse<T> CreateResponseWithPagination(IQueryable<T> pagedCategories, int totalRecords, PaginationQueryParams paginationQueryParams, string paginationUrl)
        {
            PaginationResponse<T> paginationResponse = new PaginationResponse<T>();

            double totalPages = ((double) totalRecords / (double) paginationQueryParams.Per_Page);

            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            paginationResponse.TotalRecords = totalRecords;
            paginationResponse.Data = pagedCategories;
            paginationResponse.PreviousPage = paginationQueryParams.Page - 1 >= 1 && paginationQueryParams.Page <= roundedTotalPages
                ?
                    string.Concat
                    (
                        paginationUrl,
                        "?page=",
                        (
                            (paginationQueryParams.Page - 1) < 1
                                ? 1
                                : paginationQueryParams.Page - 1
                        ),
                        "&per_page=", paginationQueryParams.Per_Page > 100 ? 100 : paginationQueryParams.Per_Page
                   )
                : null;
            paginationResponse.NextPage = paginationQueryParams.Page >= 1 && paginationQueryParams.Page < roundedTotalPages
                ?
                    string.Concat
                    (
                        paginationUrl,
                        "?page=",
                        (
                            (paginationQueryParams.Page + 1) < 1
                                ? 1
                                : paginationQueryParams.Page + 1
                        ),
                        "&per_page=", paginationQueryParams.Per_Page > 100 ? 100 : paginationQueryParams.Per_Page
                   )
                : null;

            return paginationResponse;
        }
    }
}

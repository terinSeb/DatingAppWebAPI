using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingAppCore.Helpers
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response,PaginationHeader header)
        {
            var jsOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            //var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header,jsOptions));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagintaion");
        }
    }
}

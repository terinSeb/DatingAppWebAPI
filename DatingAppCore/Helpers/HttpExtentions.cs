﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingAppCore.Helpers
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response,int currentPage, int itemsPerPage,
            int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            response.Headers.Add("Pagintaion", JsonSerializer.Serialize(paginationHeader));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagintaion");
        }
    }
}

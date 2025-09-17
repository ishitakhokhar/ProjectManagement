﻿using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Models
{
    [BindProperties(SupportsGet = true)]
    public class PaginationParams
    {
        public string Search { get; set; } 
        public string Status { get; set; } 
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
    }
}

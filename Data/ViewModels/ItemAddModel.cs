﻿using Microsoft.AspNetCore.Http;

namespace Data.ViewModels
{
    public class ItemAddModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Category { get; set; }

        public int Quantity { get; set; }

        public int QuantityForSale { get; set; }

        public int AvailableQuantity { get; set; }

        public string Description { get; set; }
        
        public string Location { get; set; }

        public IFormFile Image { get; set; }

        public string? ImageURL { get; set; }

        public string ImagePublicId { get; set; }
    }

    public class ItemAddModelWithFormFile
    {
        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Price { get; set; }

        public string? Category { get; set; }

        public string? Quantity { get; set; }

        public string? QuantityForSale { get; set; }

        public string? AvailableQuantity { get; set; }

        public string? Description { get; set; }
        
        public string? Location { get; set; }

        public IFormFile? Image { get; set; }

        public bool? ImageChanges { get; set; } // bool?
    }
}

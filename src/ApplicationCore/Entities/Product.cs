﻿namespace ApplicationCore.Entities
{
    public class Product
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Ingredients { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }
    }
}

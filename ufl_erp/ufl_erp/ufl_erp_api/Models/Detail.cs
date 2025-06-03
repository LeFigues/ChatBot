﻿using System.ComponentModel.DataAnnotations;

namespace ufl_erp_api.Models
{
    public class Detail
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int SaleId { get; set; }
        public Sale? Sale { get; set; }
    }
}

﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EntrevistaSoloTalentoBackend.Models
{
    public partial class Products
    {
        public Products()
        {
            Sales = new HashSet<Sales>();
            StoreProducts = new HashSet<StoreProducts>();
        }

        public int Code { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Image { get; set; }
        public int? Stock { get; set; }

        public virtual ICollection<Sales> Sales { get; set; }
        public virtual ICollection<StoreProducts> StoreProducts { get; set; }
    }
}
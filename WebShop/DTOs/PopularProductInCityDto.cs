using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models;
using WebShop.Models.Interfaces;

namespace WebShop.DTOs
{
    [NotMapped]
    public class PopularProductInCityDto : IProduct


    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
        [NotMapped]
        public ICollection<Category> Categories { get; set; }
        public int SoldCount { get; set; }
        public string Country { get; set; }


    }

}

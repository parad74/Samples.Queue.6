using System.ComponentModel.DataAnnotations;

namespace Rabbit.WebApi.Requests
{
      public class OrderRequest
    {
        [Required]
        public string? ProductName { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }
    }
}
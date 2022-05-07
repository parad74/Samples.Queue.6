using System.ComponentModel.DataAnnotations;

namespace Rabbit.WebApi.Requests
{
    public class CustomerRequest
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }
    }
}
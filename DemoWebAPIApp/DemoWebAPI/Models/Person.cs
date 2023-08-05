using System.ComponentModel.DataAnnotations;

namespace DemoWebAPI.Models
{
    public class Person
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string?   FirstName { get; set;}
        [Required]
        [MaxLength (20)]
        public string? LastName { get; set; }
        public string? Age { get; set; }
    }
}

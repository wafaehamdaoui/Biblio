using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string  description { get; set; }
        [Required]
        public string url { get; set; }
        [Required]
        public string author { get; set;}
        [Required]
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
    }
}

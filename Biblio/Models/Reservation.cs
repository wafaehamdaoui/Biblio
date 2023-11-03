using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblio.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        [Required]
        public DateTime TakeDate { get; set; }
        [Required]
        public DateTime BackDate { get; set; }

    }
}

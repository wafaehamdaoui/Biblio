﻿using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class Subscriber
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}

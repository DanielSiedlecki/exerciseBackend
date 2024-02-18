using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exerciseBackend.Models
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string taskName { get; set; }
        [Required]
        public string intervalDay { get; set; }
        [Required]
        public int intervalCount { get; set; }
        [Required]
        public int countOccurrences { get; set; }
        [Required]
        public DateTime nowDate { get; set; }
        [Required]
        public DateTime startDate { get;  set; }
        [Required]
        [NotMapped]
        public int timestampStartDate { get; set; }
        [Required]
        public DateTime firstExecutionDate { get;  set; }
        [Required]
        public DateTime lastExecutionDate { get;  set; }
        [Required]
        public DateTime nextExecutionDate { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;

    }
}
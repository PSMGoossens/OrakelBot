using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Models
{
    public class Reminder
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public User ForUser { get; set; }

        [Required]
        public string CronosFormat { get; set; }

        [Required]
        public string Text { get; set; }



    }
}

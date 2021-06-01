using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Models
{
    public class CommandCount
    {

        [Key]
        public long CommandCountId { get; set; }

        public string Command { get; set; }

        public int Count { get; set; } 

        public User User { get; set; }
    }
}

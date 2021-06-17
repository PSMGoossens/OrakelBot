using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Models
{
    public class User
    {
        [Key]
        public long DiscordId { get; set; }


        public string CurrentUsername { get; set; }


        public bool IsAdmin { get; set; }

        public string Discriminator { get; set; }


    }
}

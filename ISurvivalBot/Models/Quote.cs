using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Models
{
    public class Quote
    {


        [Key]
        public int Id;
        public string AddedByUser{ get; set; }
        public DateTime AddedAt { get; set; }



        public string QuoteAboutUser { get; set; }


        [Required]
        public string QuoteText { get; set; }


        //[Required]
        //public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.domain.Models.ViewModels
{
    public class CountryVM
    {
        public string CountryFullname { get; set; }
        public string CountryShortname { get; set; }
        public string CountryDescription { get; set; }
        public string CountryCode2 { get; set; }
        public string CountryCode3 { get; set; }
    }
}

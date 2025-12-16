using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.domain.Models.Entities
{
    public class Country : BaseEntity
    {
        [Required]
        public string CountryFullname { get; set; }
        [Required]
        public string CountryShortname { get; set; }
        public string? CountryDescription { get; set; } = null;
        [Required]
        public string CountryCode2 { get; set; }
        [Required]
        public string CountryCode3 { get; set; }
    }
}

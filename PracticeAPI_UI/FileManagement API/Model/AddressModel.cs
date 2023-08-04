using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class AddressModel
    {
       
        [Key]
        public Int64 AddressId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public int ZipCode { get; set; }


        [NotMapped]
        public string? CountryName { get; set; }

        [NotMapped]
        public string? StateName { get; set; }

        [NotMapped]
        public string? CityName { get; set; }
    }
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
    }
    public class State
    {
        [Key]
        public int StateId { get; set; }
        public string? StateName { get; set; }
        public int CountryId { get; set; }
    }
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public int StateId { get; set; }
    }
}

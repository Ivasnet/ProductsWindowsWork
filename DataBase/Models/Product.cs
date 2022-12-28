using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("Products")]
    public class Product
    {
        private string _id;

        [Required]
        public string Name { get; set; }

        [Key]
        public string Id
        {
            get 
            {
                return _id;
            }
            set
            {
                value = value.ToUpper();

                if (value.Length == 24 && IsHex(value))
                {
                    _id = value;
                }
            }
        }

        private bool IsHex(string chars)
        {
            bool isHex;
            foreach (var c in chars)
            {
                isHex = (c >= '0' && c <= '9') ||
                         (c >= 'A' && c <= 'F');

                if (!isHex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

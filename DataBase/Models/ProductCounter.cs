using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("ProductsCounters")]
    public class ProductCounter
    {
        private int _reception;

        private int _shipment;

        public int Reception
        {
            get
            {
                return _reception;
            }
            set
            {
                if (value >= 0)
                {
                    _reception = value;
                }
            }
        }

        public int Shipment
        {
            get
            {
                return _shipment;
            }
            set
            {
                if (value >= 0)
                {
                    _shipment = value;
                }
            }
        }

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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEShop.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        public bool Finaly { get; set; }

        public DateTime? PaymentDate { get; set; }
  
        public string? Authority { get; set; }
        public long? RefId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public ICollection<OrderDetails>? OrderDetails { get; set; }
    }
}

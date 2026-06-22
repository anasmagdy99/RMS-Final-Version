using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS_BACKEND.Models
{
    [Table("TransactionTypes")]
    public class TransactionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public double Unit { get; set; }

        [Required]
        public int Sign { get; set; }

        // Navigation Properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

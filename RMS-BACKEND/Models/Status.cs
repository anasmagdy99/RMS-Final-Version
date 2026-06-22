using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS_BACKEND.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? StatusName { get; set; }

        public string? Entity { get; set; }

        public int? OrderNumber { get; set; }

        // Navigation Properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

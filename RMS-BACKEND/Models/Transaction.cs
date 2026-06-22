using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS_BACKEND.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public int TransactionTypesID { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public int? SubstituteEmployeeId { get; set; }

        public string? LeaveRationale { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ResponseDate { get; set; }

        [Required]
        public string ResponseMessage { get; set; } = string.Empty;

        [Required]
        public int StatusID { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime CreationDate { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        // Navigation Properties
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [ForeignKey("SubstituteEmployeeId")]
        public virtual Employee? SubstituteEmployee { get; set; }

        [ForeignKey("TransactionTypesID")]
        public virtual TransactionType? TransactionType { get; set; }

        [ForeignKey("StatusID")]
        public virtual Status? Status { get; set; }
    }
}

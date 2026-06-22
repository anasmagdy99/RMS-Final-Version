using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS_BACKEND.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfEmployment { get; set; }

        [Required]
        public EmployeeRole EmployeeRole { get; set; }

        [Required]
        public int EmployeeLevelId { get; set; }

        public int? ManagerId { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        // Navigation Properties
        [ForeignKey("EmployeeLevelId")]
        public virtual EmployeeLevel? EmployeeLevel { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Employee? Manager { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual Status? Department { get; set; }

        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Transaction> SubstituteTransactions { get; set; } = new List<Transaction>();
    }

    public enum EmployeeRole : short
    {
        Employee = 0,
        Manager = 1
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS_BACKEND.Models
{
    [Table("EmployeeLevel")]
    public class EmployeeLevel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string LevelName { get; set; } = string.Empty;

        [Required]
        public string LevelDescription { get; set; } = string.Empty;

        [Required]
        public int RegularLeaveperYear { get; set; }

        [Required]
        public int CasualLeavePerYear { get; set; }

        [Required]
        public int OrderId { get; set; }

        // Navigation Properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

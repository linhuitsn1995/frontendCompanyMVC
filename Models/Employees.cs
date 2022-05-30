using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCompany.Models
{
    [Table("Employees")]
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeId { get; set; }
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string EmployeeFirstName { get; set; }
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string EmployeeLastName { get; set; }
        [Range(1, 100000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }
        [Display(Name = "Role")]
        public string Designation { get; set; }
        [Display(Name = "Age")]
        public int Age { get; set; }
    }
}

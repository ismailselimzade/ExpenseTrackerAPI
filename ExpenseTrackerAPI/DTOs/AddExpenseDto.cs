using ExpenseTrackerAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs
{
    public class AddExpenseDto
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public string Description { get; set; }
    }
}

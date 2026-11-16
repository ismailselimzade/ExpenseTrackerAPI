using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.DTOs
{
    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}

namespace ExpenseTrackerAPI.Models
{
    public class User
    {
        public User()
        {
            Expenses = new HashSet<Expense>();
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}

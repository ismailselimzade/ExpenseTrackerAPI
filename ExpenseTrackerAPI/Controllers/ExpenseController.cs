using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ExpenseController(AppDbContext db)
        {
            this._db = db;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense(AddExpenseDto addExpenseDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expense = new Expense() { UserId = userId, Amount = addExpenseDto.Amount, Category = addExpenseDto.Category, Description = addExpenseDto.Description, Date = DateTime.UtcNow };

            await _db.Expenses.AddAsync(expense);
            await _db.SaveChangesAsync();

            return Ok(expense);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExpense(UpdateExpenseDto updateExpenseDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var expense = await _db.Expenses.FindAsync(updateExpenseDto.Id);

            if (expense == null || expense.UserId != userId)
                return NotFound();

            expense.Category = updateExpenseDto.Category;
            expense.Description = updateExpenseDto.Description;
            expense.Amount = updateExpenseDto.Amount;
            expense.Date = DateTime.UtcNow;

            _db.Update(expense);
            await _db.SaveChangesAsync();

            return Ok();

        }


        [HttpDelete("{expenseId}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var expense = await _db.Expenses.FindAsync(expenseId);

            if (expense == null || expense.UserId != userId)
                return NotFound();

            _db.Remove(expense);
            await _db.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetExpenses([FromQuery] string? filter = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            List<Expense> expenses = null;
            if (string.Equals(filter, "lastMonth", StringComparison.OrdinalIgnoreCase))
                expenses = await _db.Expenses.Where(e => e.UserId == userId && e.Date > DateTime.UtcNow.AddMonths(-1)).ToListAsync();
            else if (string.Equals(filter, "last3Month", StringComparison.OrdinalIgnoreCase))
                expenses = await _db.Expenses.Where(e => e.UserId == userId && e.Date > DateTime.UtcNow.AddMonths(-3)).ToListAsync();
            else if (filter == null && startDate != null && endDate != null)
                expenses = await _db.Expenses.Where(e => e.UserId == userId && e.Date <= endDate && e.Date >= startDate).ToListAsync();

            return Ok(expenses ?? await _db.Expenses.Where(e => e.UserId == userId).ToListAsync());
        }
    }
}

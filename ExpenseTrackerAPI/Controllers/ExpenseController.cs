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
        public async Task<IActionResult> GetExpenses(
            [FromQuery] string? filter = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            IQueryable<Expense> query = _db.Expenses
                .Where(e => e.UserId == userId);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                if (string.Equals(filter, "lastMonth", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(e => e.Date >= DateTime.UtcNow.AddMonths(-1));
                }
                else if (string.Equals(filter, "last3Month", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(e => e.Date >= DateTime.UtcNow.AddMonths(-3));
                }
                else
                {
                    return BadRequest("Invalid filter. Allowed values: lastMonth, last3Month.");
                }
            }
            else
            {
                if (startDate.HasValue)
                {
                    var utcStart = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
                    query = query.Where(e => e.Date >= utcStart);
                }

                if (endDate.HasValue)
                {
                    var utcEndExclusive = DateTime.SpecifyKind(endDate.Value.Date.AddDays(1), DateTimeKind.Utc);
                    query = query.Where(e => e.Date < utcEndExclusive);
                }
            }

            var expenses = await query
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    Amount = e.Amount,
                    Category = e.Category,
                    Description = e.Description,
                    Date = e.Date
                })
                .ToListAsync();

            return Ok(expenses);
        }
    }
}

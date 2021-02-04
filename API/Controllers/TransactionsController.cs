using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Entities;
using System;
using System.Linq;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly DataContext context;

        public TransactionsController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Transaction>> GetTransactions([FromQuery] DateTime? startDate,
                                                                    [FromQuery] DateTime? endDate)
        {
            await context.Transactions.LoadAsync();
            await context.Accounts.LoadAsync();

            if (startDate == null && endDate == null)
            {
                return await context.Transactions.ToListAsync();
            }
            else
            {
                return await context.Transactions
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .OrderBy(t => t.Date)
                    .ToListAsync();
            }
        }

        [HttpGet("{id}")]
        public async Task<Transaction> GetTransaction(int id)
        {
            return await context.Transactions.FindAsync(id);
        }

        [HttpPost]
        public async Task AddTransaction([FromForm] Transaction transaction)
        {
            await AdjustBalanceRecursive(transaction.CreditAccount, transaction.Amount, true);
            await AdjustBalanceRecursive(transaction.DebitAccount, transaction.Amount, false);
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();
        }

        private async Task AdjustBalanceRecursive(Account child, decimal amount, bool credit)
        {
            if (credit)
            {
                child.Credit(amount);
            }
            else
            {
                child.Debit(amount);
            }

            if (child.ParentId != null)
            {
                Account parent = await context.Accounts.FindAsync(child.ParentId);
                await AdjustBalanceRecursive(parent, amount, credit);
            }
        }
    }
}

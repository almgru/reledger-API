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
        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            return await context.Transactions.ToListAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRange([FromQuery] DateTime startDate,
                                                                               [FromQuery] DateTime endDate)
        {
            return await context.Transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Transaction> GetTransaction(int id)
        {
            return await context.Transactions.FindAsync(id);
        }

        [HttpPost]
        public async Task AddTransaction([FromForm] Transaction transaction)
        {
            if (transaction.ToAccount.IncreaseBalanceOn == Account.IncreaseBalanceBehaviour.OnDebit)
            {
                transaction.ToAccount.Balance += transaction.Amount;
            }
            else
            {
                transaction.ToAccount.Balance -= transaction.Amount;
            }

            if (transaction.FromAccount.IncreaseBalanceOn == Account.IncreaseBalanceBehaviour.OnCredit)
            {
                transaction.FromAccount.Balance += transaction.Amount;
            }
            else
            {
                transaction.FromAccount.Balance -= transaction.Amount;
            }

            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();
        }
    }
}
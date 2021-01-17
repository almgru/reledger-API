using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Entities;

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

        [HttpGet("{id}")]
        public async Task<Transaction> GetTransaction(int id)
        {
            return await context.Transactions.FindAsync(id);
        }

        [HttpPost]
        public async Task AddTransaction([FromForm] Transaction transaction)
        {
            transaction.DebitAccount.Balance -= transaction.Amount;
            transaction.CreditAccount.Balance += transaction.Amount;

            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();
        }
    }
}
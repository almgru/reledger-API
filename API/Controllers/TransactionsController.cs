using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using API.Data;
using API.Model;

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
                return await context.Transactions
                    .Select(trans => new Transaction
                    {
                        Amount = trans.Amount,
                        Currency = trans.Currency,
                        DebitAccount = trans.DebitAccount.Name,
                        CreditAccount = trans.CreditAccount.Name,
                        DateTime = trans.DateTime,
                        Description = trans.Description,
                        Tags = trans.Tags.Select(tag => tag.Name)
                    })
                    .ToListAsync();
            }
            else
            {
                return await context.Transactions
                    .Where(t => t.DateTime >= startDate && t.DateTime <= endDate)
                    .Select(trans => new Transaction
                    {
                        Amount = trans.Amount,
                        Currency = trans.Currency,
                        DebitAccount = trans.DebitAccount.Name,
                        CreditAccount = trans.CreditAccount.Name,
                        DateTime = trans.DateTime,
                        Description = trans.Description,
                        Tags = trans.Tags.Select(tag => tag.Name)
                    })
                    .OrderBy(t => t.DateTime)
                    .ToListAsync();
            }
        }

        [HttpGet("{id}")]
        public async Task<Transaction> GetTransaction(int id)
        {
            return await context.Transactions
                .Where(trans => trans.Id == id)
                .Select(trans => new TransactionWithAttachments
                {
                    Amount = trans.Amount,
                    Currency = trans.Currency,
                    DebitAccount = trans.DebitAccount.Name,
                    CreditAccount = trans.CreditAccount.Name,
                    DateTime = trans.DateTime,
                    Description = trans.Description,
                    Tags = trans.Tags.Select(tag => tag.Name).ToList(),
                    Attachments = trans.Attachments
                        .Select(atch => new Attachment
                        {
                            Name = atch.Name,
                            Data = atch.Data
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync();
        }

        [HttpPost]
        public async Task AddTransaction([FromBody] AddTransaction request)
        {
            var debit = await context.Accounts
                .SingleAsync(acc => acc.Name == request.DebitAccount);
            var credit = await context.Accounts
                .SingleAsync(acc => acc.Name == request.CreditAccount);

            await AdjustBalanceRecursive(credit, request.Amount, true);
            await AdjustBalanceRecursive(debit, request.Amount, false);

            context.Transactions.Add(new API.Data.Entities.Transaction
            {
                Amount = request.Amount,
                Currency = request.Currency,
                DebitAccount = debit,
                CreditAccount = credit,
                DateTime = request.DateTime,
                Description = request.Description
            });
            await context.SaveChangesAsync();

            // TODO: Add tags and attachments
        }

        private async Task AdjustBalanceRecursive(
                API.Data.Entities.Account child,
                decimal amount,
                bool credit)
        {
            if (child == null) { return; }

            child.Balance = credit
                ? child.Balance - amount
                : child.Balance + amount;

            var parent = await context.Accounts.FindAsync(child.ParentId);
            await AdjustBalanceRecursive(parent, amount, credit);
        }
    }
}

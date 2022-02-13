using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ReledgerApi.Data;
using ReledgerApi.Model;

namespace ReledgerApi.Controllers
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
        public async Task AddTransaction([FromBody] AddTransactionRequest request)
        {
            if (!decimal.TryParse(request.Amount, out var amount))
            {
                BadRequest("Amount could not be parsed as decimal.");
            }

            var debit = await context.Accounts
                .SingleAsync(acc => acc.Name == request.DebitAccount);
            var credit = await context.Accounts
                .SingleAsync(acc => acc.Name == request.CreditAccount);

            context.Transactions.Add(new ReledgerApi.Data.Entities.Transaction
            {
                Amount = amount,
                Currency = request.Currency,
                DebitAccount = debit,
                CreditAccount = credit,
                DateTime = request.DateTime,
                Description = request.Description
            });
            await context.SaveChangesAsync();

            // TODO: Add tags and attachments
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ReledgerApi.Data;
using ReledgerApi.Model;
using Microsoft.AspNetCore.Http;

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

        /// <summary>Get all transactions. Optionally filter by date.</summary>
        /// <remarks>
        ///     <paramref name="startDate"/> and <paramref name="endDate"/> can be used together or individidually.
        /// </remarks>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Transaction>> GetTransactions([FromQuery] DateTime? startDate,
                                                                    [FromQuery] DateTime? endDate)
        {
            var query = context.Transactions.AsQueryable();

            if (startDate != null && endDate != null)
            {
                query = query.Where(t => t.DateTime >= startDate && t.DateTime <= endDate);
            }
            else if (startDate != null)
            {
                query = query.Where(t => t.DateTime >= startDate);
            }

            return await query
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
                    .ToArrayAsync();
        }

        /// <summary></summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionWithAttachments), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionWithAttachments>> GetTransaction(int id)
        {
            var result = await context.Transactions
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

            if (result == null) { return NotFound(); }

            return result;
        }

        /// <summary></summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTransaction([FromBody] AddTransactionRequest request)
        {
            var debit = await context.Accounts
                .SingleOrDefaultAsync(acc => acc.Name == request.DebitAccount);
            var credit = await context.Accounts
                .SingleOrDefaultAsync(acc => acc.Name == request.CreditAccount);

            if (debit == null || credit == null) { return NotFound(); }

            context.Transactions.Add(new ReledgerApi.Data.Entities.Transaction
            {
                Amount = request.Amount,
                Currency = request.Currency,
                DebitAccount = debit,
                CreditAccount = credit,
                DateTime = request.DateTime,
                Description = request.Description
            });

            // TODO: Add tags and attachments

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}

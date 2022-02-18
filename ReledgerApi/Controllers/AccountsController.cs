using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReledgerApi.Data;
using ReledgerApi.Data.Extensions;
using ReledgerApi.Model;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace ReledgerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly DataContext context;

        public AccountsController(DataContext context)
        {
            this.context = context;
        }

        /// <summary>Get names of all accounts.</summary>
        /// <returns>A list containing all account names. Returns empty list if no accounts.</returns>
        /// <response code="200"></response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Account>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await context.Accounts
                .Select(acc => new Account { Name = acc.Name })
                .ToListAsync();
        }

        /// <summary>Get detailed information about an account</summary>
        /// <returns>Detailed information about an account, including balance.</returns>
        /// <response code="200"></response>
        /// <response code="404">No account with the specified name exists.</response>
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(AccountWithBalance), StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountWithBalance>> GetAccount(
            string name,
            [FromQuery] DateTime? start,
            CancellationToken token)
        {
            if (await context.Accounts.NoneAsync(acc => acc.Name == name, token))
            {
                return NotFound();
            }

            // TODO: Support multiple currencies
            var query = context.Transactions.AsQueryable();

            if (start != null)
            {
                query = query.Where(trans => trans.DateTime >= start);
            }

            // decimal is stored as string in SQLite, so we need to sum client-side
            var debitAmounts = await query
                .Where(trans =>
                    trans.DebitAccount.Name == name ||
                    trans.DebitAccount.Name.StartsWith($"{name}."))
                .Select(trans => trans.Amount)
                .ToListAsync();
            var creditAmounts = await query
                .Where(trans =>
                    trans.CreditAccount.Name == name ||
                    trans.CreditAccount.Name.StartsWith($"{name}."))
                .Select(trans => trans.Amount)
                .ToListAsync();
            var totalDebit = debitAmounts.Sum();
            var totalCredit = creditAmounts.Sum();

            return await context.Accounts
                .Where(acc => acc.Name == name)
                .Select(acc => new AccountWithBalance
                {
                    Name = acc.Name,
                    Balance = totalDebit - totalCredit
                })
                .SingleOrDefaultAsync();
        }

        /// <summary>Add a new account</summary>
        /// <response code="200">OK is returned even if account already exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddAccount(AddAccountRequest request)
        {
            await AddAccountAndChildren(new ReledgerApi.Data.Entities.Account { Name = request.Name } );
            await context.SaveChangesAsync();

            return Ok();
        }

        private async Task AddAccountAndChildren(ReledgerApi.Data.Entities.Account account)
        {
            // If account has no children or parents, just add it if it doesn't exist and return
            if (account.Name.IndexOf(".") == -1)
            {
                context.Accounts.AddIfNotExists(account, acc => acc.Name == account.Name);
                return;
            }

            // Store a copy of the original account string so we can use that as a starting point for children
            var accountString = account.Name;

            var original = accountString;
            int rightMostDotIndex;
            /*
             * We start with getting the rightmost parent (for example "A.B" in account string "A.B.C" ["A.B.C" is not
             * a parent since it has no children]) and work our way up to more ancestral accounts (First "A.B", then
             * the parent of "A.B": "A").
             */
            while ((rightMostDotIndex = accountString.LastIndexOf(".")) != -1) // until no more parents
            {
                var parentName = accountString.Substring(0, rightMostDotIndex).Trim();
                var childString = original;
                var parent = await context.Accounts
                    .SingleOrDefaultAsync(acc => acc.Name == parentName);


                if (parent == null)
                {
                    parent = new ReledgerApi.Data.Entities.Account
                    {
                        Name = parentName
                    };

                    context.Accounts.Add(parent);
                    await context.SaveChangesAsync();
                }

                /*
                 * For each parent we determine its children by using the original account string as a starting point
                 * and working our way upwards until the child has the same account name as the current parent (So with 
                 * the previous example of "A.B.C", the child of "A.B" becomes "A.B.C" and the child of "A" is first
                 * "A.B.C" and then "A.B").
                 */
                while (childString != parent.Name)
                {
                    var child = await context.Accounts
                        .SingleOrDefaultAsync(acc => acc.Name == childString);

                    if (child == null)
                    {
                        child = new ReledgerApi.Data.Entities.Account
                        {
                            Name = childString
                        };
                        context.Accounts.Add(child);
                        await context.SaveChangesAsync();
                    }

                    if (child.ParentId == null)
                    {
                        child.ParentId = parent.Id;
                    }

                    childString = childString.Substring(0, childString.LastIndexOf(".")).Trim();
                }

                accountString = accountString.Substring(0, rightMostDotIndex).Trim();
            }
        }
    }
}

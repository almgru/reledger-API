using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReledgerApi.Data;
using ReledgerApi.Data.Extensions;
using ReledgerApi.Model;

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

        [HttpGet]
        public async Task<IEnumerable<Account>> GetAccounts()
        {
            return await context.Accounts
                .Select(acc => new Account { Name = acc.Name })
                .ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<AccountWithBalance> GetAccount(string name, [FromQuery] DateTime? start)
        {
            var query = context.Transactions.AsQueryable();

            if (start != null)
            {
                query = query.Where(trans => trans.DateTime >= start);
            }

            var totalDebit = await query
                .Where(trans => trans.DebitAccount.Name == name)
                .Select(trans => trans.Amount)
                .ToListAsync();
            var totalCredit = await query
                .Where(trans => trans.CreditAccount.Name == name)
                .Select(trans => trans.Amount)
                .ToListAsync();

            return await context.Accounts
                .Where(acc => acc.Name == name)
                .Select(acc => new AccountWithBalance
                {
                    Name = acc.Name,
                    Balance = totalDebit.Sum() - totalCredit.Sum()
                })
                .SingleOrDefaultAsync();
        }

        [HttpPost]
        public async Task AddAccount(AddAccountRequest request)
        {
            await AddAccountAndChildren(new ReledgerApi.Data.Entities.Account
            { Name = request.Name });
            await context.SaveChangesAsync();
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

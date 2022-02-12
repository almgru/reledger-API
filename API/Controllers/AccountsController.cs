using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Data.Extensions;
using API.Model;

namespace API.Controllers
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
                .Select(acc => new Account
                {
                    Name = acc.Name,
                    IncreaseBalanceOn = acc.IncreaseBalanceOn,
                    Balance = acc.Balance
                })
                .ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<Account> GetAccount(string name)
        {
            return await context.Accounts
                .Where(acc => acc.Name == name)
                .Select(acc => new Account
                {
                    Name = acc.Name,
                    IncreaseBalanceOn = acc.IncreaseBalanceOn,
                    Balance = acc.Balance
                })
                .SingleOrDefaultAsync();
        }

        [HttpPost]
        public async Task AddAccount(AddAccountRequest request)
        {
            await AddAccountAndChildren(new API.Data.Entities.Account
            {
                Name = request.Name,
                IncreaseBalanceOn = request.IncreaseBalanceOn,
                Balance = request.Balance
            });
            await context.SaveChangesAsync();
        }

        private async Task AddAccountAndChildren(API.Data.Entities.Account account)
        {
            // If account has no children or parents, just add it if it doesn't exist and return
            if (account.Name.IndexOf(".") == -1)
            {
                context.Accounts.AddIfNotExists(account, acc => acc.Name == account.Name);
                return;
            }

            // Store a copy of the original account string so we can use that as a starting point for children
            var accountString = account.Name;

            // Make sure increase balance behaviour is inherited from parent
            var mostAncestralAccountName = accountString.Substring(0, accountString.IndexOf("."));
            var mostAncestralAccount = await context.Accounts
                .SingleOrDefaultAsync(acc => acc.Name == mostAncestralAccountName);

            var increaseBehaviour = mostAncestralAccount != null
                ? mostAncestralAccount.IncreaseBalanceOn
                : account.IncreaseBalanceOn;

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
                    parent = new API.Data.Entities.Account
                    {
                        Name = parentName,
                        IncreaseBalanceOn = increaseBehaviour
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
                        child = new API.Data.Entities.Account
                        {
                            Name = childString,
                            IncreaseBalanceOn = increaseBehaviour
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

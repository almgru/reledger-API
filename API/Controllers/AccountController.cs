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
    public class AccountController : ControllerBase
    {
        private readonly DataContext context;

        public AccountController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Account>> GetAccounts()
        {
            return await context.Accounts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Account> GetAccount(int id)
        {
            return await context.Accounts.FindAsync(id);
        }

        [HttpPost]
        public async Task AddAccount([FromBody] Account account)
        {
            // Store a copy of the original account string so we can use that as a starting point for descendants
            string accountString = account.Name;
            string original = accountString;
            int rightMostDotIndex;

            /*
             * We start with by getting the rightmost ancestor (for example "A.B" in account string "A.B.C" ["A.B.C"
             * is not an ancestor since it has no descendants]) and work our way up to more ancestral accounts
             * (First "A.B", then the ancestor of "A.B": "A").
             *
             * For each ancestor we determine its descendants by using the original account string as a starting
             * point and working our way upwards until the descendant has the same account name as the current
             * ancestor (So the descendant of "A.B" becomes "A.B.C" and the descendant of "A" is first "A.B.C" and
             * then "A.B").
             */
            while ((rightMostDotIndex = accountString.LastIndexOf(".")) != -1) // until no more ancestors
            {
                var descendants = new List<Account>();
                var ancestor = new Account() { Name = accountString.Substring(0, rightMostDotIndex).Trim() };
                var descendantString = original;

                while (descendantString != ancestor.Name)
                {
                    /*
                     * Commit changes so the next query will be able to find the current descendant if it exists in the
                     * database.
                     */
                    await context.SaveChangesAsync();

                    var descendant = await context.Accounts.SingleOrDefaultAsync(acc => acc.Name == descendantString);

                    if (descendant == null)
                    {
                        descendant = new Account() { Name = descendantString };
                        await context.Accounts.AddAsync(descendant);
                    }

                    descendants.Add(descendant);
                    descendantString = descendantString.Substring(0, descendantString.LastIndexOf(".")).Trim();
                }

                ancestor.Descendants = descendants;
                await context.AddAsync(ancestor);
                accountString = accountString.Substring(0, rightMostDotIndex).Trim();
            }

            await context.SaveChangesAsync();
        }
    }
}
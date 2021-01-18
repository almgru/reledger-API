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
            return await context.Accounts.ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<Account> GetAccount(string name)
        {
            return await context.Accounts.SingleOrDefaultAsync(acc =>
                acc.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost]
        public async Task AddAccount([FromForm] Account account)
        {
            await context.AddAccountAndChildrenAsync(account);
            await context.SaveChangesAsync();
        }
    }
}
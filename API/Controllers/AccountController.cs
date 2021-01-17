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
        public async void AddAccount(Account account)
        {
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();
        }
    }
}
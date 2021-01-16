using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> GetTransactions()
        {
            return new List<string>() { "Hello,", "world!" };
        }

        [HttpGet("{id}")]
        public string GetTransactions(int id)
        {
            return "Hello, world!";
        }
    }
}
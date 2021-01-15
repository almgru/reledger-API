using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using API.DAO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly DatabaseAccess databaseAccess;

        public TransactionsController(DatabaseAccess databaseAccess)
        {
            this.databaseAccess = databaseAccess;
        }
    }
}
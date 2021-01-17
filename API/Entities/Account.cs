using System.Collections.Generic;

namespace API.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Account> Descendants { get; set; }
    }
}
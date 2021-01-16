using System.Collections.Generic;

namespace API.Entities
{
    public class Account
    {
        public Account(string name, IEnumerable<Account> descendants)
        {
            this.Name = name;
            this.Descendants = descendants;
        }

        public string Name { get; private set; }
        public IEnumerable<Account> Descendants { get; private set; }
    }
}
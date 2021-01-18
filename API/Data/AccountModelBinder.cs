using System;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace API.Data
{
    /// <summary>
    /// Custom binding for converting JSON POST data to Account entity.
    ///
    /// Currently, it looks at the name of the form data, looks for an entity that matches that in the database, and, if
    /// there is a match, returns that entity.
    /// </summary>
    public class AccountModelBinder : IModelBinder
    {
        private readonly DataContext context;

        public AccountModelBinder(DataContext context)
        {
            this.context = context;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            string firstModelNameCharacter = bindingContext.ModelName.Substring(0, 1);
            string camelCasedModelName = bindingContext.ModelName.Replace(firstModelNameCharacter,
                                                                          firstModelNameCharacter.ToLower());
            string value = bindingContext.HttpContext.Request.Form[camelCasedModelName];

            await this.context.Accounts.LoadAsync(); // TODO: Potential performance issue
            Account account = await this.context.Accounts.SingleOrDefaultAsync(acc => acc.Name == value);

            if (account == null)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName,
                                                        String.Format("No account with name '{0}'.", value));
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(account);
            }
        }
    }
}
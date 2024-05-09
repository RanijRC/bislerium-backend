using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Repository.Contracts
{
    public interface IEmail
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}

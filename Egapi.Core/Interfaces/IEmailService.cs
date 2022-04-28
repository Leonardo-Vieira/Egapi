using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egapi.Core.Interfaces
{
    public interface IEmailService
    {
        void SendVerifyEmailAddressNotification(string memberEmail, string verificationToken);
        void SendEmailToAdminNotification(string name, string email, string message);
    }
}

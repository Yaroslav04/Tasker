using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Core
{
    public static class MailServise
    {
        public static async Task SendEmailAsync( string _text)
        {
            string _mail = File.ReadAllText(FileManager.GeneralPath("mail.txt"));
            MailAddress from = new MailAddress("ischenkoyaroslav@gmail.com", "Tasker");
            MailAddress to = new MailAddress(_mail);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Tasker " + DateTime.Now.ToShortDateString();
            m.Body = _text;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("ischenkoyaroslav@gmail.com", "icksxcqinjfcbvpm");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
            return;
        }
    }
}

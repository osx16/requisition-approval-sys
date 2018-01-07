using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;

namespace OrderCreationForm.Controllers
{
    public class EmailController : Controller
    {
        [NonAction]
        public void SendEmailNotification(String emailbody, string emailAddress)
        {
            MailMessage mailMessage = new MailMessage("requsitionapproval@gmail.com", emailAddress);
            mailMessage.Subject = "[NO REPLY] New Requisition Submitted";
            mailMessage.Body = emailbody;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "requsitionapproval@gmail.com",
                Password = "carpentersfiji"
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }

        [NonAction]
        public void SendApprovalConfirmationEmail(String emailbody, string emailAddress)
        {
            MailMessage mailMessage = new MailMessage("requsitionapproval@gmail.com", emailAddress);
            mailMessage.Subject = "[NO REPLY] Requisition Notification";
            mailMessage.Body = emailbody;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "requsitionapproval@gmail.com",
                Password = "carpentersfiji"
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }

        // GET: Email
        public ActionResult Index()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Timers;
using GMAPI.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMAPI.Other
{
    public static class EmailService
    {
        
        //Er is nog geen globale ip adress dus je kan hem hier instellen
        private static String hostIp = "192.168.2.000";

        static SmtpClient client = new SmtpClient();

        public static void SetupEmail()
        {
            client.Credentials = new System.Net.NetworkCredential("Yorandevos12@gmail.com", "exffpcnglfeuhmcn");
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
        }

        public static void SendVerificationEmail(String email, String verificationId)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress("Yorandevos12@gmail.com", "Lugus", System.Text.Encoding.UTF8);
            mail.Subject = "Email Verification";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "To verify that you registered please click this link:" + hostIp + "/api/auth/email/verify=" + verificationId +
                "\nIf you did not register on our app then please ignore this email and the account will be removed from our server.";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                //TODO need exception handling
            }
        }
    }
}
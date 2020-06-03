using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Timers;
using GMAPI.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GMAPI.Other
{
    public static class EmailService
    {
        
        //Er is nog geen globale ip adress dus je kan hem hier instellen
        private static String hostIp = "80.112.188.42:5000";
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
                "\n\nIf you did not register on our app then please ignore this email and the account will be removed from our server.";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                
            }
        }
        
        public static Dictionary<Guid, Guid> verifications = new Dictionary<Guid, Guid>();
        

        public static void CreateEmailVerificationInstance(Account user)
        {
            Guid verificationId = Guid.NewGuid();
            verifications.Add(verificationId, user.Id);
            Timer verificationTimer = new Timer(6000000);
            verificationTimer.Elapsed += (sender, e) =>
            {
                RemoveVerificationInstance(verificationId);
                verificationTimer.Dispose();
            };
            SendVerificationEmail(user.Email, verificationId.ToString());
            verificationTimer.Enabled = true;
        }

        public static void RemoveVerificationInstance(Guid id)
        {
            verifications.Remove(id);
        }
    }
}
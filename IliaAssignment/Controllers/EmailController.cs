using AutoMapper;
using IliaAssignment.Data;
using IliaAssignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace IliaAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private SMTP _smtp;

        public EmailController(IOptions<SMTP> smtp)
        {
            _smtp = smtp.Value;
        }

        public bool EnviarEmail(string ToName, string ToEmail, string Subject, string Body)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_smtp.Username, _smtp.Username));
            mimeMessage.To.Add(new MailboxAddress(ToName, ToEmail));
            mimeMessage.Subject = Subject;
            mimeMessage.Body = new TextPart("plain")
            {
                Text = Body
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(_smtp.Host, _smtp.Port, true);
                smtpClient.Authenticate(_smtp.Username, _smtp.Password);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            };

            return true;
        }
    }
}

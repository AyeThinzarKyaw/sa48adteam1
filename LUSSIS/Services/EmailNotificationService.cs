﻿using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Helpers;

namespace LUSSIS.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private IEmployeeRepo employeeRepo;
        private static EmailNotificationService instance = new EmailNotificationService();

        public EmailNotificationService()
        {
            employeeRepo = EmployeeRepo.Instance;
        }

        //returns single instance
        public static IEmailNotificationService Instance
        {
            get { return instance; }
        }

        public void NotifyClerkShortFallInStationery(Stationery s, Employee clerk)
        {
            string body = "Dear " + clerk.Name + " there is insufficient incoming stock for Stationery: " + s.Description + " of Item Code: " + s.Code + ". Please raise a necessary Purchase Order for the recent demand. Thank you.";
            string subject = "Shortfall in Stationery. Please raise a Purchase Order.";

            SendNotificationEmail(clerk.Email, subject, body);
        }

        public void NotifyDeptHeadToApprovePendingRequisition(Requisition newRequisition)
        {
            Employee e = employeeRepo.FindById(newRequisition.EmployeeId);
            string employeeEmail = employeeRepo.FindById(newRequisition.EmployeeId).Email;
            string deptHeadEmail = null;

            Employee cs = employeeRepo.GetCoverStaffByDepartmentIdAndDate(e.DepartmentId, DateTime.Now);
            if(cs != null)
            {
                deptHeadEmail = cs.Email;
            }
            else
            {
                deptHeadEmail = employeeRepo.FindOneBy(x=> x.DepartmentId == e.DepartmentId && x.RoleId == 1).Email;
            }

            //MailMessage mail = new MailMessage(employeeEmail, deptHeadEmail);
            string subject = "New Requisition Form Submission for Approval";
            string body = "Please review a new Requisition form (Id:"+ newRequisition.Id
                +") submmission by " +e.Name + "for approval. Thank you.";
            SendNotificationEmail(deptHeadEmail, subject, body);
        }

        /**
        private void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient("smtp.nus.edu.sg", 587);
            client.Credentials = new NetworkCredential();
            client.Send(mail);
        } **/

        public void SendNotificationEmail(string receipient,string subject, string body, IEnumerable<string> attachments = null,string cc= null)
        {
            //SmtpClient client = new SmtpClient();
            //MailMessage mm = new MailMessage();
            //mm.To.Add(receipient);
            //mm.Subject = subject;
            //mm.Body = body;
            //if (attachments!=null)
            //{
            //    foreach (var item in attachments)
            //    {
            //        System.Net.Mail.Attachment attachment;
            //        attachment = new System.Net.Mail.Attachment(item);
            //        mm.Attachments.Add(attachment);
            //    }
            //}

            //client.Send(mm);

            WebMail.SmtpServer = "smtp.gmail.com";
            //gmail port to send emails 
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            //sending emails with secure protocol 
            WebMail.EnableSsl = true;
            //EmailId used to send emails from application 
            WebMail.UserName = "sa48team1@gmail.com";
            WebMail.Password = "Team1@SA48";
            //Sender email address. 
            WebMail.From = "sa48team1@gmail.com";

            //Send email 
            WebMail.Send(to: "sa48team1@gmail.com", subject: subject, body: body,cc: cc,filesToAttach: attachments, isBodyHtml: false);
        }

        public void NotifyEmployeeApprovedOrRejectedRequisition(Requisition r, Employee e)
        {
            string subject = "Result of pending requisition approval";
            string body = "Dear " + e.Name + ", your Requisition of Id: " + r.Id + ", has been updated as " + r.Status;
            SendNotificationEmail(e.Email, subject, body);
        }
        
        public void NotifyEmployeeCompletedRequisition(Requisition r, Employee e)
        {
            string subject = "Completed Requisition";
            string body = "Dear " + e.Name + ", your Requisition of Id: " + r.Id + ", has now been completed with all items requested fulfilled.";
            SendNotificationEmail(e.Email, subject, body);
        }
    }
}
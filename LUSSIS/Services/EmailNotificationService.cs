using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace LUSSIS.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private IEmployeeRepo employeeRepo;
        private static EmailNotificationService instance = new EmailNotificationService();

        private EmailNotificationService()
        {
            employeeRepo = EmployeeRepo.Instance;
        }

        //returns single instance
        public static IEmailNotificationService Instance
        {
            get { return instance; }
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

            MailMessage mail = new MailMessage(employeeEmail, deptHeadEmail);
            mail.Subject = "New Requisition Form Submission for Approval";
            mail.Body = "Please review a new Requisition form (Id:"+ newRequisition.Id
                +") submmission by " +e.Name + "for approval. Thank you.";
            //SendEmail(mail);
        }

        private void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient("smtp.nus.edu.sg", 587);
            client.Credentials = new NetworkCredential();
            client.Send(mail);
        }
    }
}
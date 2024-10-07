using AspNetCoreHero.ToastNotification.Abstractions;
using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Vereyon.Web;

namespace garage87.Controllers
{
    public class EmailController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly UserManager<User> _userManager;
        private readonly INotyfService _notyf;
        public EmailController
            (ICustomerRepository customerRepository,
            IImageHelper imageHelper,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage, IUserHelper userHelper, IMailHelper mailHelper, UserManager<User> userManager, IMessageRepository messageRepository, INotyfService notyf)
        {
            _customerRepository = customerRepository;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _userManager = userManager;
            _messageRepository = messageRepository;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendMail()
        {
            var customerEmails = _customerRepository.GetCustomerEmails().ToList();
            if (customerEmails.Any())
            {
                foreach (var email in customerEmails)
                {
                    // Compose the email message body
                    string messageBody = $"<h1>Autoshop Closure Notice</h1>" +
                                         $"<p>Dear user,</p>" +
                                         $"<p>This is to inform you that the autoshop remains closed for today.</p>" +
                                         $"<p>Thank you for your understanding!</p>";

                    try
                    {
                        // Send the email
                        Response response = _mailHelper.SendEmail(email, "Autoshop Closure Notice", messageBody);
                        if (!response.IsSuccess)
                        {
                            _notyf.Error($"Email sending failed to customer: {email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _notyf.Error($"Email sending failed to customer: {email}");
                    }
                }
            }
            _notyf.Success("Email sent successfully to all customers.");
            return RedirectToAction("Index", "Email");
        }

        public IActionResult SendReminderEmails()
        {


            var Emails = _customerRepository.GetReminderEmails().ToList();

            // Ensure userIds is not null or empty
            if (Emails != null && Emails.Any())
            {

                foreach (var email in Emails)
                {
                    // Compose the email message body
                    string messageBody = $"<h1>Appointment Reminder</h1>" +
                                         $"<p>Dear user,</p>" +
                                         $"<p>This is a friendly reminder that you have an appointment scheduled for tomorrow.</p>" +
                                         $"<p>Thank you!</p>";

                    try
                    {
                        // Send the email
                        Response response = _mailHelper.SendEmail(email, "Appointment Reminder", messageBody);
                        if (!response.IsSuccess)
                        {
                            _notyf.Error($"Email sending failed to customer: {email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _notyf.Error($"Email sending failed to customer: {email}");
                    }
                }
            }
            else
            {

            }
            _notyf.Success("Email sent successfully to all customers.");
            return RedirectToAction("Index", "Email");
        }

    }
}

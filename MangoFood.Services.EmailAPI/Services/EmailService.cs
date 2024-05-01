using MangoFood.Services.EmailAPI.Models;
using MangoFood.Services.EmailAPI.Models.DTO;
using MangoFood.Services.ShoppingCartAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MangoFood.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
           _dbOptions = dbOptions;
        }

        public async Task EmailAndCatalog(ShoppingCartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDTO.CartHeader.TotalAmount);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach(var item in cartDTO.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + "x" + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardDTO rewardDTO)
        {
            string message = "New Order Placed. <br/> Order ID: " + rewardDTO.OrderId;
            await LogAndEmail(message, rewardDTO.Email);
        }

        public async Task UserRegisteredCatalog(string email)
        {
            var message = "New user just registered successfully. <br/> Email: " + email;
            await LogAndEmail(message, email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                var emailLog = new EmailLogger()
                {
                    Email = email,
                    Message = message,
                    EmailSentAt = DateTime.Now
                };

                await using var _db = new AppDbContext(_dbOptions);
                _db.EmailLoggers.Add(emailLog);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;   
            }
        }
    }
}

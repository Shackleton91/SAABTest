using System;
using System.IO;
using System.Text.Json;
using EmailService;

namespace TicketManagementSystem
{
    public class TicketService
    {
        public int CreateTicket(string title, Priority priority, string assignedTo, string description, DateTime created, bool isPayingCustomer)
        {
            ValidateTicketInputs(title, description);

            var user = GetUserByUsername(assignedTo);

            priority = UpdatePriorityIfNeeded(priority, created, title);

            HandleHighPriorityTicket(priority, assignedTo);

            var (price, accountManager) = CalculatePriceAndAccountManager(isPayingCustomer, priority);

            var ticket = new Ticket
            {
                Title = title,
                AssignedUser = user,
                Priority = priority,
                Description = description,
                Created = created,
                PriceDollars = price,
                AccountManager = accountManager
            };

            var id = TicketRepository.CreateTicket(ticket);
            WriteTicketToFile(ticket);

            return id;
        }

        public void AssignTicket(int id, string username)
        {
            var user = GetUserByUsername(username);

            var ticket = GetTicketById(id);

            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }

        private void ValidateTicketInputs(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                throw new InvalidTicketException("Title or description were null or empty");
            }
        }

        private User GetUserByUsername(string username)
        {
            using (var userRepository = new UserRepository())
            {
                var user = userRepository.GetUser(username);
                if (user == null)
                {
                    throw new UnknownUserException($"User {username} not found");
                }
                return user;
            }
        }

        private Priority UpdatePriorityIfNeeded(Priority priority, DateTime created, string title)
        {
            var priorityRaised = false;

            if (created < DateTime.UtcNow - TimeSpan.FromHours(1))
            {
                if (priority == Priority.Low)
                {
                    priority = Priority.Medium;
                    priorityRaised = true;
                }
                else if (priority == Priority.Medium)
                {
                    priority = Priority.High;
                    priorityRaised = true;
                }
            }

            if ((title.Contains("Crash") || title.Contains("Important") || title.Contains("Failure")) && !priorityRaised)
            {
                if (priority == Priority.Low)
                {
                    priority = Priority.Medium;
                }
                else if (priority == Priority.Medium)
                {
                    priority = Priority.High;
                }
            }

            return priority;
        }

        private void HandleHighPriorityTicket(Priority priority, string assignedTo)
        {
            if (priority == Priority.High)
            {
                var emailService = new EmailServiceProxy();
                emailService.SendEmailToAdministrator(assignedTo);
            }
        }

        private (double price, User accountManager) CalculatePriceAndAccountManager(bool isPayingCustomer, Priority priority)
        {
            double price = 0;
            User accountManager = null;

            if (isPayingCustomer)
            {
                accountManager = new UserRepository().GetAccountManager();
                price = (priority == Priority.High) ? 100 : 50;
            }

            return (price, accountManager);
        }

        private Ticket GetTicketById(int id)
        {
            var ticket = TicketRepository.GetTicket(id);

            if (ticket == null)
            {
                throw new ApplicationException($"No ticket found for id {id}");
            }

            return ticket;
        }

        private void WriteTicketToFile(Ticket ticket)
        {
            var ticketJson = JsonSerializer.Serialize(ticket);
            var filePath = Path.Combine(Path.GetTempPath(), $"ticket_{ticket.Id}.json");
            File.WriteAllText(filePath, ticketJson);
        }
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
} 

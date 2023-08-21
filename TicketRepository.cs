using System.Collections.Generic;
using System.Linq;

namespace TicketManagementSystem
{
    public static class TicketRepository
    {
        private static readonly List<Ticket> Tickets = new List<Ticket>();

        public static int CreateTicket(Ticket ticket)
        {
            ticket.Id = GetNextTicketId();
            Tickets.Add(ticket);

            return ticket.Id;
        }

        public static void UpdateTicket(Ticket ticket)
        {
            var outdatedTicket = Tickets.FirstOrDefault(t => t.Id == ticket.Id);

            if (outdatedTicket != null)
            {
                Tickets.Remove(outdatedTicket);
                Tickets.Add(ticket);
            }
        }

        public static Ticket GetTicket(int id)
        {
            return Tickets.FirstOrDefault(ticket => ticket.Id == id);
        }

        private static int GetNextTicketId()
        {
            var currentHighestTicket = Tickets.Any() ? Tickets.Max(ticket => ticket.Id) : 0;
            return currentHighestTicket + 1;
        }
    }
} 

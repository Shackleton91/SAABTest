using System;

namespace TicketManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ticket Service Test Harness");

            TicketServiceTestHarness.Run();

            Console.WriteLine("Done");
        }
    }

    class TicketServiceTestHarness
    {
        public static void Run()
        {
            var service = new TicketService();

            var ticketId = CreateTicket(service);
            AssignTicket(service, ticketId);

            // Add more test cases if needed
        }

        private static int CreateTicket(TicketService service)
        {
            var ticketId = service.CreateTicket(
                "System Crash",
                Priority.Medium,
                "Johan",
                "The system crashed when the user performed a search",
                DateTime.UtcNow,
                true);

            return ticketId;
        }

        private static void AssignTicket(TicketService service, int ticketId)
        {
            service.AssignTicket(ticketId, "Michael");
        }
    }
} 

using System.Globalization;
using DatabaseContext;
using DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Core;

public class Facade
{
    private TicketCenterRepository _repository;
    private Dictionary<string, Func<Task>> _operations;
    
    private const string VIEW_MENU_AGAIN = "0";
    private const string ADD_EXHIBITION = "1";
    private const string ADD_TICKET = "2";
    private const string ADD_VISITOR = "3";
    private const string DELETE_EXHIBITION = "4";
    private const string DELETE_TICKET = "5";
    private const string DELETE_VISITOR = "6";
    private const string EDIT_EXHIBITION = "7";
    private const string EDIT_TICKET = "8";
    private const string EDIT_VISITOR = "9";
    private const string GET_EXHIBITION_BY_ID = "10";
    private const string GET_TICKET_BY_ID = "11";
    private const string GET_VISITOR_BY_ID = "12";
    private const string GET_COUNT_SOLD_TICKETS_BY_EXHIBITION_ID = "13";
    private const string GET_UNIQUE_EXHIBITIONS_BY_VISITOR_ID = "14";
    private const string GET_AVERAGE_DISCOUNT_FOR_VISITORS_BY_EXHIBITION_ID = "15";
    private const string OUTPUT_ALL_EXHIBITIONS = "16";
    private const string OUTPUT_ALL_TICKETS = "17";
    private const string OUTPUT_ALL_VISITORS = "18";
    private const string FINISH_PROCESS = "19";
    
    private const string TRY_ENTERING_AGAIN = "1";
    private const string RETURN_MENU = "2";
    
    public Facade(TicketCenterRepository repository)
    {
        _repository = repository;
        
        _operations = new Dictionary<string, Func<Task>>
        {
            { VIEW_MENU_AGAIN, ViewMenuAgain },
            { ADD_EXHIBITION, AddExhibition },
            { ADD_TICKET, AddTicket },
            { ADD_VISITOR, AddVisitor },
            { DELETE_EXHIBITION, DeleteExhibition },
            { DELETE_TICKET, DeleteTicket },
            { DELETE_VISITOR, DeleteVisitor },
            { EDIT_EXHIBITION, EditExhibition },
            { EDIT_TICKET, EditTicket },
            { EDIT_VISITOR, EditVisitor },
            { GET_EXHIBITION_BY_ID, GetExhibitionById },
            { GET_TICKET_BY_ID, GetTicketById },
            { GET_VISITOR_BY_ID, GetVisitorById },
            { GET_COUNT_SOLD_TICKETS_BY_EXHIBITION_ID, GetCountSoldTicketsByExhibitionId },
            { GET_UNIQUE_EXHIBITIONS_BY_VISITOR_ID, GetUniqueExhibitionsByVisitorId },
            { GET_AVERAGE_DISCOUNT_FOR_VISITORS_BY_EXHIBITION_ID, GetAverageDiscountForVisitorsByExhibitionId },
            { OUTPUT_ALL_EXHIBITIONS, OutputAllExhibitions },
            { OUTPUT_ALL_TICKETS, OutputAllTickets },
            { OUTPUT_ALL_VISITORS, OutputAllVisitors },
            { FINISH_PROCESS, Finish }
        };
    }
    
    public async Task Run()
    {
        Menu();
        while (true)
        {
            string? inputOperationNumber = Console.ReadLine();

            if (_operations.TryGetValue(inputOperationNumber, out Func<Task> operation))
            {
                try
                {
                    await operation();
                    if (inputOperationNumber == FINISH_PROCESS)
                    {
                        break;
                    }
                }
                catch (Exceptions ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("An unknown operation number has been entered!" +
                                  " Number of existing operations in the range from 1 to 16");
            }
            Console.WriteLine("\nSelect next operation (0 - view the menu again):");
        }
    }

    public void Menu()
    {
        Console.WriteLine("..........................................");
        Console.WriteLine("Select operation (enter operation number):");
        Console.WriteLine("|ADD OPERATIONS|");
        Console.WriteLine("1. Add exhibition");
        Console.WriteLine("2. Add ticket");
        Console.WriteLine("3. Add visitor");
        Console.WriteLine("|DELETE OPERATIONS|");
        Console.WriteLine("4. Delete exhibition");
        Console.WriteLine("5. Delete ticket");
        Console.WriteLine("6. Delete visitor");
        Console.WriteLine("|EDIT OPERATIONS|");
        Console.WriteLine("7. Edit exhibition");
        Console.WriteLine("8. Edit ticket");
        Console.WriteLine("9. Edit visitor");
        Console.WriteLine("|GET BY ID OPERATIONS|");
        Console.WriteLine("10. Get exhibition by ID");
        Console.WriteLine("11. Get ticket by ID");
        Console.WriteLine("12. Get visitor by ID");
        Console.WriteLine("|GET STATISTICS OPERATIONS|");
        Console.WriteLine("13. Get the count of tickets sold to the exhibition by ID");
        Console.WriteLine("14. Get the count of unique exhibitions visited by a visitor by ID");
        Console.WriteLine("15. Get the average discount percentage visitors of exhibition by ID");
        Console.WriteLine("|OUTPUT OPERATIONS|");
        Console.WriteLine("16. Output all exhibitions");
        Console.WriteLine("17. Output all tickets");
        Console.WriteLine("18. Output all visitors");
        Console.WriteLine("\n\n19. Finish program");
        Console.WriteLine("..........................................");
    }

    private async Task AddExhibition()
    {
        Console.WriteLine("Enter exhibition's name:");
        InputName(out var name, out var exitRequestName);
        if (!exitRequestName && name is not null)
        {
            Console.WriteLine("Enter the date of the exhibition (in format YYYY.MM.DD):");
            InputDate(out var date, out var exitRequestDate);
            if (!exitRequestDate && date is not null)
            {
                Exhibition exhibition = new Exhibition { Name = name, 
                    Date = DateTime.SpecifyKind((DateTime)date, DateTimeKind.Utc) };
                try
                {
                    await _repository.AddExhibitionAsync(exhibition);
                    Console.WriteLine("Exhibition added.");
                }
                catch (DbUpdateException)
                {
                    throw new FailedUpdateDbException();
                }
            }
        }
    }
    private async Task AddTicket()
    {
        Console.WriteLine("Enter exhibition's ID in which you want to add ticket:");
        if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }
        Exhibition exhibition = await _repository.GetExhibitionByIdAsync(exhibitionId);
        if (exhibition == null)
        {
            Console.WriteLine("First, create a exhibition with that ID!");
            throw new NonExistentEntityException("Exhibition");
        }
        
        Console.WriteLine("Enter visitor's ID to whom you want to add ticket:");
        if (!int.TryParse(Console.ReadLine(), out int visitorId))
        {
            throw new IncorrectIdException("visitor");
        }
        Visitor visitor = await _repository.GetVisitorByIdAsync(visitorId);
        if (visitor == null)
        {
            Console.WriteLine("First, create a visitor with that ID!");
            throw new NonExistentEntityException("Visitor");
        }
        Console.WriteLine("Enter the price of the ticket:");
        InputPrice(out var price, out var exitRequestPrice);
        if (!exitRequestPrice && price is not null)
        {
            double priceWithDiscount = Math.Round((double)price * ((double)(100 - visitor.Discount) / 100), 2);

            Ticket ticket = new Ticket
            {
                Price = Math.Round((double)price, 2), PriceWithDiscount = priceWithDiscount, 
                VisitorId = visitorId, ExhibitionId = exhibitionId
            };

            try
            {
                await _repository.AddTicketAsync(ticket);
                Console.WriteLine("Ticket added.");
            }
            catch (DbUpdateException)
            {
                throw new FailedUpdateDbException();
            }
        }
    }
    private async Task AddVisitor()
    {
        Console.WriteLine("Enter visitor's name:");
        InputName(out var name, out var exitRequestName);
        if (!exitRequestName && name is not null)
        {
            Console.WriteLine("Enter the visitor discount (number from 0 to 100):");
            InputDiscount(out var discount, out var exitRequestDiscount);
            if (!exitRequestDiscount && discount is not null)
            {
                Visitor visitor = new Visitor { Name = name, Discount = (int)discount};
        
                try
                {
                    await _repository.AddVisitorAsync(visitor);
                    Console.WriteLine("Visitor added.");
                }
                catch (DbUpdateException)
                {
                    throw new FailedUpdateDbException();
                }
            }
        }
    }
    private async Task DeleteExhibition()
    {
        Console.WriteLine("Enter exhibition's ID to delete:");
        if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }
        try
        {
            var isDeleted = await _repository.DeleteExhibitionAsync(exhibitionId);
            Console.WriteLine(isDeleted ? "Exhibition deleted." : "No exhibition with this ID.");
        }
        catch (DbUpdateException)
        {
            throw new FailedUpdateDbException();
        }
    }
    private async Task DeleteTicket()
    {
        Console.WriteLine("Enter ticket's ID to delete:");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        {
            throw new IncorrectIdException("ticket");
        }
        try
        {
            var isDeleted = await _repository.DeleteTicketAsync(ticketId);
            Console.WriteLine(isDeleted ? "Ticket deleted." : "No ticket with this ID.");
        }
        catch (DbUpdateException)
        {
            throw new FailedUpdateDbException();
        }
    }
    private async Task DeleteVisitor()
    {
        Console.WriteLine("Enter visitor's ID to delete:");
        if (!int.TryParse(Console.ReadLine(), out int visitorId))
        {
            throw new IncorrectIdException("visitor");
        }
        try
        {
            var isDeleted = await _repository.DeleteVisitorAsync(visitorId);
            Console.WriteLine(isDeleted ? "Visitor deleted." : "No visitor with this ID.");
        }
        catch (DbUpdateException)
        {
            throw new FailedUpdateDbException();
        }
    }
    private async Task EditExhibition()
    {
        Console.WriteLine("Enter exhibition's ID to update:");
        if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }
        
        Exhibition exhibition = await _repository.GetExhibitionByIdAsync(exhibitionId);
        
        if (exhibition == null)
        {
            throw new NonExistentEntityException("Exhibition");
        }
        
        Console.WriteLine($"Current exhibition's name {exhibition.Name}");
        Console.WriteLine("Enter new exhibition's name:");
        InputName(out var newName, out var exitRequestName);
        if (!exitRequestName && newName is not null)
        {
            Console.WriteLine($"Current exhibition's date {exhibition.Date}");
            Console.WriteLine("Enter new date of the exhibition (in format YYYY.MM.DD)::");
            InputDate(out var newDate, out var exitRequestDate);
            if (!exitRequestDate && newDate is not null)
            {
                exhibition.Name = newName;
                exhibition.Date = DateTime.SpecifyKind((DateTime)newDate, DateTimeKind.Utc);
                try
                {
                    await _repository.EditExhibitionAsync(exhibition);
                    Console.WriteLine("Exhibition updated.");
                }
                catch (DbUpdateException)
                {
                    throw new FailedUpdateDbException();
                }
            }
        }
    }
    private async Task EditTicket()
    {
        Console.WriteLine("Enter ticket's ID to update:");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        {
            throw new IncorrectIdException("ticket");
        }
        
        Ticket ticket = await _repository.GetTicketByIdAsync(ticketId);
        
        if (ticket == null)
        {
            throw new NonExistentEntityException("Ticket");
        }
        
        Console.WriteLine($"Current ticket's visitor ID {ticket.VisitorId}");
        Console.WriteLine("Enter new ticket's visitor ID:");
        if (!int.TryParse(Console.ReadLine(), out int newVisitorId))
        {
            throw new IncorrectIdException("visitor");
        }
        
        Visitor visitor = await _repository.GetVisitorByIdAsync(newVisitorId);
        
        if (visitor == null)
        {
            Console.WriteLine("First, create a visitor with that ID!");
            throw new NonExistentEntityException("Visitor");
        }
        
        Console.WriteLine($"Current ticket's exhibition ID {ticket.ExhibitionId}");
        Console.WriteLine("Enter new ticket's exhibition ID:");
        if (!int.TryParse(Console.ReadLine(), out int newExhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }
        
        Exhibition exhibition = await _repository.GetExhibitionByIdAsync(newExhibitionId);
        
        if (exhibition == null)
        {
            Console.WriteLine("First, create a exhibition with that ID!");
            throw new NonExistentEntityException("Exhibition");
        }
        
        Console.WriteLine($"Current ticket's price {ticket.Price}");
        Console.WriteLine("Enter new price of the ticket:");
        InputPrice(out var newPrice, out var exitRequestPrice);
        if (!exitRequestPrice && newPrice is not null)
        {
            double priceWithDiscount = Math.Round((double)newPrice * ((double)(100 - visitor.Discount) / 100), 2);

            ticket.Price = Math.Round((double)newPrice, 2);
            ticket.PriceWithDiscount = priceWithDiscount;
            ticket.VisitorId = newVisitorId;
            ticket.ExhibitionId = newExhibitionId;
        
            try
            {
                await _repository.EditTicketAsync(ticket);
                Console.WriteLine("Ticket updated.");
            }
            catch (DbUpdateException)
            {
                throw new FailedUpdateDbException();
            }
        }
    }
    private async Task EditVisitor()
    {
        Console.WriteLine("Enter visitor's ID to update:");
        if (!int.TryParse(Console.ReadLine(), out int visitorId))
        {
            throw new IncorrectIdException("visitor");
        }
        
        Visitor visitor = await _repository.GetVisitorByIdAsync(visitorId);
        
        if (visitor == null)
        {
            throw new NonExistentEntityException("Visitor");
        }
        
        Console.WriteLine($"Current visitor's name {visitor.Name}");
        Console.WriteLine("Enter new visitor's name:");
        InputName(out var newName, out var exitRequestName);
        if (!exitRequestName && newName is not null)
        {
            Console.WriteLine($"Current visitor`s discount {visitor.Discount}");
            Console.WriteLine("Enter new visitor discount (number from 0 to 100):");
            InputDiscount(out var newDiscount, out var exitRequestDiscount);
            if (!exitRequestDiscount && newDiscount is not null)
            {
                visitor.Name = newName;
                visitor.Discount = (int)newDiscount;
                var tickets = visitor.Tickets;
                foreach (var ticket in tickets)
                {
                    double priceWithDiscount = Math.Round(ticket.Price * ((double)(100 - visitor.Discount) / 100), 2);
                    ticket.PriceWithDiscount = priceWithDiscount;
                }
        
                try
                {
                    await _repository.EditVisitorAsync(visitor);
                    Console.WriteLine("Visitor updated.");
                }
                catch (DbUpdateException)
                {
                    throw new FailedUpdateDbException();
                }
            }
        }
    }
    private async Task GetExhibitionById()
    {
        Console.WriteLine("Enter exhibition's ID which you want to view:");
        if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }

        Exhibition exhibition = await _repository.GetExhibitionByIdAsync(exhibitionId);
        
        if (exhibition == null)
        {
            throw new NonExistentEntityException("Exhibition");
        }
        Console.WriteLine($"Information about exhibition ID - {exhibition.Id}:");
        Console.WriteLine($"Exhibition`s name - {exhibition.Name}");
        Console.WriteLine($"Exhibition`s date (YYYY.MM.DD) - {exhibition.Date:yyyy.MM.dd}");
    }
    private async Task GetTicketById()
    {
        Console.WriteLine("Enter ticket's ID which you want to view:");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        {
            throw new IncorrectIdException("ticket");
        }

        Ticket ticket = await _repository.GetTicketByIdAsync(ticketId);
        
        if (ticket == null)
        {
            throw new NonExistentEntityException("Ticket");
        }
        Console.WriteLine($"Information about ticket ID - {ticket.Id}:");
        Console.WriteLine($"Ticket`s price - {ticket.Price}");
        Console.WriteLine($"Ticket`s price with discount - {ticket.PriceWithDiscount}");
        Console.WriteLine($"Ticket`s visitor ID - {ticket.VisitorId}");
        Console.WriteLine($"Ticket`s exhibition ID - {ticket.ExhibitionId}");
    }
    private async Task GetVisitorById()
    {
        Console.WriteLine("Enter visitor's ID which you want to view:");
        if (!int.TryParse(Console.ReadLine(), out int visitorId))
        {
            throw new IncorrectIdException("visitor");
        }

        Visitor visitor = await _repository.GetVisitorByIdAsync(visitorId);
        
        if (visitor == null)
        {
            throw new NonExistentEntityException("Visitor");
        }
        Console.WriteLine($"Information about visitor ID - {visitor.Id}:");
        Console.WriteLine($"Visitor`s name - {visitor.Name}");
        Console.WriteLine($"Visitor`s discount - {visitor.Discount}");
    }
    
    private async Task GetCountSoldTicketsByExhibitionId()
    {
        Console.WriteLine("Enter exhibition's ID which you want to view count sold tickets:");
        if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }

        int countSoldTickets = await _repository.GetCountSoldTicketsByExhibitionIdAsync(exhibitionId);
        
        Console.WriteLine($"Count sold tickets: {countSoldTickets}");
    }

    private async Task GetUniqueExhibitionsByVisitorId()
    {
        Console.WriteLine("Enter visitor's ID whom you want to view count unique exhibitions:");
        if (!int.TryParse(Console.ReadLine(), out int visitorId))
        {
            throw new IncorrectIdException("visitor");
        }

        int countUniqueExhibitions = await _repository.GetUniqueExhibitionsByVisitorIdAsync(visitorId);
        
        Console.WriteLine($"Count unique exhibitions: {countUniqueExhibitions}");
    }
    
    private async Task GetAverageDiscountForVisitorsByExhibitionId()
    {
        Console.WriteLine("Enter exhibition's ID which you want to view average discount visitors:");
        if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
        {
            throw new IncorrectIdException("exhibition");
        }

        double averageDiscount = await _repository.GetAverageDiscountForVisitorsByExhibitionIdAsync(exhibitionId);
        
        Console.WriteLine($"Average discount visitors: {Math.Round(averageDiscount, 2)}");
    }

    private async Task OutputAllExhibitions()
    {
        Console.WriteLine("All exhibitions:");
        var exhibitions = await _repository.GetAllExhibitionsAsync();
        Console.WriteLine("| ID  | Name                                     | Date       |");
        Console.WriteLine("|-----|------------------------------------------|------------|");
        foreach (Exhibition exhibition in exhibitions)
        {
            Console.WriteLine($"| {exhibition.Id,-3} | {exhibition.Name,-40} | {exhibition.Date:yyyy.MM.dd} |");
        }
    }

    private async Task OutputAllTickets()
    {
        Console.WriteLine("All tickets:");
        var tickets = await _repository.GetAllTicketsAsync();
        Console.WriteLine("| ID  | Price      | Price with discount | Visitor ID | Exhibition ID |");
        Console.WriteLine("|-----|------------|---------------------|------------|---------------|");
        foreach (Ticket ticket in tickets)
        {
            Console.WriteLine($"| {ticket.Id,-3} | {ticket.Price,-10} | {ticket.PriceWithDiscount,-19} " +
                              $"| {ticket.VisitorId,-10} | {ticket.ExhibitionId,-13} |");
        }
    }

    private async Task OutputAllVisitors()
    {
        Console.WriteLine("All visitors:");
        var visitors = await _repository.GetAllVisitorsAsync();
        Console.WriteLine("| ID  | Name            | Discount |");
        Console.WriteLine("|-----|-----------------|----------|");
        foreach (Visitor visitor in visitors)
        {
            Console.WriteLine($"| {visitor.Id,-3} | {visitor.Name,-15} | {visitor.Discount + "%", -8} |");
        }
    }
    private Task Finish()
    {
        Console.WriteLine("Program finished.");
        return Task.CompletedTask;
    }

    private Task ViewMenuAgain()
    {
        Menu();
        return Task.CompletedTask;
    }

    private string TryAgainMenu(string incorrectData)
    {
        Console.WriteLine($"ERROR: Incorrect {incorrectData} input!");
    
        Console.WriteLine($"1. Try entering the {incorrectData} again");
        Console.WriteLine("2. Return to the menu");
        string? choice;
        while (true)
        {
            choice = Console.ReadLine();
            if (choice != TRY_ENTERING_AGAIN && choice != RETURN_MENU)
            {
                Console.WriteLine("An unknown operation number has been entered!" +
                                  " Number of existing operations in the range from 1 to 2");
            }
            else
            {
                break;
            }
            Console.WriteLine("Select operation (enter operation number):");
        }
        return choice;
    }

    private void InputName(out string? name, out bool exitRequest)
    {
        while (true)
        {
            name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("ERROR: Name can't be empty");
                string choice = TryAgainMenu("name");
                if (choice != TRY_ENTERING_AGAIN)
                {
                    name = null;
                    exitRequest = true;
                    break;
                }
            }
            else
            {
                exitRequest = false;
                break;
            }
            Console.WriteLine("Enter name:");
        }
    }

    private void InputDate(out DateTime? outDate, out bool exitRequest)
    {
        while (true)
        {
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy.MM.dd", 
                    null, DateTimeStyles.None, out DateTime date))
            {
                string choice = TryAgainMenu("date");
                if (choice != TRY_ENTERING_AGAIN)
                {
                    outDate = null;
                    exitRequest = true;
                    break;
                }
            }
            else
            {
                outDate = date;
                exitRequest = false;
                break;
            }
            Console.WriteLine("Enter the date (in format YYYY.MM.DD):");
        }
    }
    
    private void InputPrice(out double? outPrice, out bool exitRequest)
    {
        while (true)
        {
            if (!double.TryParse(Console.ReadLine(), out double price))
            {
                string choice = TryAgainMenu("price");
                if (choice != TRY_ENTERING_AGAIN)
                {
                    outPrice = null;
                    exitRequest = true;
                    break;
                }
            }
            else
            {
                outPrice = price;
                exitRequest = false;
                break;
            }
            Console.WriteLine("Enter the price:");
        }
    }
    
    private void InputDiscount(out int? outDiscount, out bool exitRequest)
    {
        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out int discount) || discount < 0 || discount > 100)
            {
                string choice = TryAgainMenu("discount");
                if (choice != TRY_ENTERING_AGAIN)
                {
                    outDiscount = null;
                    exitRequest = true;
                    break;
                }
            }
            else
            {
                outDiscount = discount;
                exitRequest = false;
                break;
            }
            Console.WriteLine("Enter discount:");
        }
    }
}

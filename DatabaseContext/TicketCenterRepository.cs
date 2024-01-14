using DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace DatabaseContext;
public class TicketCenterRepository : ITicketCenterRepository
{
    private TicketCenterContext _context;

    public TicketCenterRepository(TicketCenterContext context)
    {
        _context = context;
    }
    public async Task<int> AddExhibitionAsync(Exhibition exhibition)
    {
        try
        {
            await _context.Exhibitions.AddAsync(exhibition);
            await _context.SaveChangesAsync();
            return exhibition.Id;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<int> AddTicketAsync(Ticket ticket)
    {
        try
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<int> AddVisitorAsync(Visitor visitor)
    {
        try
        {
            await _context.Visitors.AddAsync(visitor);
            await _context.SaveChangesAsync();
            return visitor.Id;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> DeleteExhibitionAsync(int exhibitionId)
    {
        try
        {
            var exhibition = await _context.Exhibitions.FindAsync(exhibitionId);
            bool foundFlag = false;
            if (exhibition != null)
            {
                _context.Exhibitions.Remove(exhibition);
                await _context.SaveChangesAsync();
                foundFlag = true;
            }

            return foundFlag;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
        
    }
    public async Task<bool> DeleteTicketAsync(int ticketId)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            bool foundFlag = false;
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                foundFlag = true;
            }

            return foundFlag;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> DeleteVisitorAsync(int visitorId)
    {
        try
        {
            var visitor = await _context.Visitors.FindAsync(visitorId);
            bool foundFlag = false;
            if (visitor != null)
            {
                _context.Visitors.Remove(visitor);
                await _context.SaveChangesAsync();
                foundFlag = true;
            }

            return foundFlag;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<Exhibition> EditExhibitionAsync(Exhibition editableExhibition)
    {
        try
        {
            var exhibition = await _context.Exhibitions.FirstOrDefaultAsync(e => e.Id == editableExhibition.Id);

            if (exhibition != null)
            {
                exhibition.Name = editableExhibition.Name;
                exhibition.Date = editableExhibition.Date;

                await _context.SaveChangesAsync();
            }

            return exhibition;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<Ticket> EditTicketAsync(Ticket editableTicket)
    {
        try
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == editableTicket.Id);

            if (ticket != null)
            {
                ticket.Price = editableTicket.Price;
                ticket.PriceWithDiscount = editableTicket.PriceWithDiscount;
                ticket.VisitorId = editableTicket.VisitorId;
                ticket.ExhibitionId = editableTicket.ExhibitionId;

                await _context.SaveChangesAsync();
            }

            return ticket;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<Visitor> EditVisitorAsync(Visitor editableVisitor)
    {
        try
        {
            var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.Id == editableVisitor.Id);

            if (visitor != null)
            {
                visitor.Name = editableVisitor.Name;
                visitor.Discount = editableVisitor.Discount;
                visitor.Tickets = editableVisitor.Tickets;
                await _context.SaveChangesAsync();
            }

            return visitor;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            throw;
        }
    }
    public async Task<Exhibition> GetExhibitionByIdAsync(int exhibitionId)
    {
        return await _context.Exhibitions.FirstOrDefaultAsync(e => e.Id == exhibitionId);
    }
    public async Task<Ticket> GetTicketByIdAsync(int ticketId)
    {
        return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
    }
    public async Task<Visitor> GetVisitorByIdAsync(int visitorId)
    {
        return await _context.Visitors.FirstOrDefaultAsync(v => v.Id == visitorId);
    }
    public async Task<int> GetCountSoldTicketsByExhibitionIdAsync(int exhibitionId)
    {
        return await _context.Tickets.CountAsync(t => t.ExhibitionId == exhibitionId);
    }
    public async Task<int> GetUniqueExhibitionsByVisitorIdAsync(int visitorId)
    {
        return await _context.Tickets
            .Where(t => t.VisitorId == visitorId)
            .GroupBy(t => t.ExhibitionId)
            .Select(group => group.Key)
            .CountAsync();
    }
    public async Task<double> GetAverageDiscountForVisitorsByExhibitionIdAsync(int exhibitionId)
    {
        return await _context.Tickets
            .Where(t => t.ExhibitionId == exhibitionId)
            .AverageAsync(t => t.Visitor.Discount);
    }
    
    public async Task<List<Exhibition>> GetAllExhibitionsAsync()
    {
        return await _context.Exhibitions.ToListAsync();
    }
    
    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets.ToListAsync();
    }
    
    public async Task<List<Visitor>> GetAllVisitorsAsync()
    {
        return await _context.Visitors.ToListAsync();
    }
}
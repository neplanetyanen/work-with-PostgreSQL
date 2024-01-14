using DatabaseModels;

namespace DatabaseContext;

public interface ITicketCenterRepository
{
    Task<int> AddExhibitionAsync(Exhibition exhibition);
    Task<int> AddTicketAsync(Ticket ticket);
    Task<int> AddVisitorAsync(Visitor visitor);
    Task<bool> DeleteExhibitionAsync(int exhibitionId);
    Task<bool> DeleteTicketAsync(int ticketId);
    Task<bool> DeleteVisitorAsync(int visitorId);
    Task<Exhibition> EditExhibitionAsync(Exhibition editableExhibition);
    Task<Ticket> EditTicketAsync(Ticket editableTicket);
    Task<Visitor> EditVisitorAsync(Visitor editableVisitor);
    Task<Exhibition> GetExhibitionByIdAsync(int exhibitionId);
    Task<Ticket> GetTicketByIdAsync(int ticketId);
    Task<Visitor> GetVisitorByIdAsync(int visitorId);
    Task<int> GetCountSoldTicketsByExhibitionIdAsync(int exhibitionId);
    Task<int> GetUniqueExhibitionsByVisitorIdAsync(int visitorId);
    Task<double> GetAverageDiscountForVisitorsByExhibitionIdAsync(int exhibitionId);
    Task<List<Exhibition>> GetAllExhibitionsAsync();
    Task<List<Ticket>> GetAllTicketsAsync();
    Task<List<Visitor>> GetAllVisitorsAsync();
}
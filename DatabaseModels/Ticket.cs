namespace DatabaseModels;

public class Ticket
{
    public int Id { get; set; }
    public double Price { get; set; }
    public double PriceWithDiscount { get; set; }
    public int VisitorId { get; set; }
    public Visitor Visitor { get; set; }
    
    public int ExhibitionId { get; set; }
    public Exhibition Exhibition { get; set; }
}
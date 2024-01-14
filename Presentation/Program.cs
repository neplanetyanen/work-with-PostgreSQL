using Core;
using DatabaseContext;

public class Program
{
    public static async Task Main()
    {
        using (var dbContext = new TicketCenterContext())
        {
            var repository = new TicketCenterRepository(dbContext);
            var facade = new Facade(repository);
            await facade.Run();
        }
    }
}
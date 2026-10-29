namespace GestionOrdinateurs.API;

public class StockService
{
    private readonly object _verrou = new();

    public int StockSSD { get; private set; } = 10;

    public void ConsommerSSD()
    {
        Task.Delay(10).Wait();

        lock (_verrou)
        {
            StockSSD--;
        }
    }
}
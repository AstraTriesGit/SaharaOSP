namespace Marketplace.Models;

public class Seller
{
    public string Address { get; }
    public string Uuid { get; }

    public Seller(string address, string uuid)
    {
        Address = address;
        Uuid = uuid;
    }
}
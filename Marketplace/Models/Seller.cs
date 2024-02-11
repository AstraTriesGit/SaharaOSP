namespace Marketplace.Models;

public class Seller
{
    public static List<Seller> RegisteredSellers = [];
    public string Address { get; }
    public string Uuid { get; }

    public Seller(string address, string uuid)
    {
        Address = address;
        Uuid = uuid;
    }
}
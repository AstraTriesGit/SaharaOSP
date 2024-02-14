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

    public override string ToString()
    {
        return Address + " " + Uuid;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var p = (Seller) obj;
        return Address == p.Address && Uuid == p.Uuid;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Address, Uuid);
    }
}
namespace Marketplace.Models;

public class Buyer
{
    public readonly string Address;
    public readonly List<Product> Wishlist = [];
    public readonly List<Product> RatedProducts = [];
    
    public Buyer(string address)
    {
        Address = address;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Buyer) obj;

        return Address == other.Address;
    }

    public override int GetHashCode()
    {
        return Address.GetHashCode();
    }
    
}
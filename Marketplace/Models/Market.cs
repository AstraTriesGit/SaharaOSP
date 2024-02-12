namespace Marketplace.Models;

public class Market
{
    public static Dictionary<Seller, List<Product>> SellerInventory = new();
    public static Dictionary<Buyer, List<Product>> Wishlists = new();
    public static Dictionary<Buyer, List<Product>> RatedProducts = new();
    
}
namespace Marketplace.Models;

public static class Market
{
    public static readonly Dictionary<Seller, List<Product>> SellerInventory = new();
    public static readonly List<Buyer> Buyers = [];

}
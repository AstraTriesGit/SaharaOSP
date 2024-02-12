using RpcComm;

namespace Marketplace.Models;

public class Product
{
    private static int _uniqueId = 1;
    
    public string ProductName;
    public Category Category;
    public int Quantity;
    public string Description;
    public string SellerAddress;
    public decimal PricePerUnit;

    internal int Id;
    internal decimal Rating;
    internal int NoOfRatings = 0;

    public Product(string productName, Category category, int quantity, string description,
        string sellerAddress, decimal pricePerUnit)
    {
        PricePerUnit = pricePerUnit;
        ProductName = productName;
        Category = category;
        Quantity = quantity;
        Description = description;
        SellerAddress = sellerAddress;

        Id = _uniqueId++;
        Rating = new decimal(0.0);
    }
    
    public override string ToString()
    {
        string[] lines =
        [
            "Id: " + Id,
            "Name: " + ProductName,
            "Price/unit: " + PricePerUnit,
            "Quantity: " + Quantity,
            "Category: " + Category,
            "Description: " + Description,
            "Seller Address: " + SellerAddress,
            "Rating: " + Rating
        ];
        return string.Join('\n', lines);
    }
}
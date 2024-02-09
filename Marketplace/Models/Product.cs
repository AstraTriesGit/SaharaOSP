namespace Marketplace.Models;

public class Product
{
    public string productName;
    public Category category;
    public int quantity;
    public string description;
    public string sellerAddress;
    public double pricePerUnit;

    internal int Id;
    internal double Rating;
    
    public override string ToString()
    {
        string[] lines =
        [
            "Id: " + Id,
            "Name: " + productName,
            "Price/unit: " + pricePerUnit,
            "Quantity: " + quantity,
            "Category: " + category,
            "Description: " + description,
            "Seller Address: " + sellerAddress,
            "Rating: " + Rating
        ];
        return string.Join('\n', lines);
    }
}
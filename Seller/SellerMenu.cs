using RpcComm;
using RpcComm.Seller;

namespace Seller;

public static class SellerMenu
{
    public static SellerToMarket.SellerToMarketClient Client = null!;
    public static Marketplace.Models.Seller CurrentSeller = null!;
    public static MarketNotification.MarketNotificationClient NotificationClient = null!;
    
    private static bool _run = true;
    private const string Menu =
        """
        Please select an option...
        1) Register as a seller
        2) Sell an item
        3) Update existing item
        4) Delete existing item
        5) Display your items
        6) Exit SaharaOSP...
        """;

    public static void SellerMain()
    {
        while (_run)
        {
            Console.WriteLine(Menu);
            var input = int.Parse(Console.ReadLine()!);
            switch (input)
            {
                case 1:
                    RegisterSeller();
                    break;
                case 2:
                    SellItem();
                    break;
                case 3:
                    UpdateItem();
                    break;
                case 4:
                    DeleteItem();
                    break;
                case 5:
                    DisplayItems();
                    break;
                case 6:
                    _run = false;
                    break;
                default:
                    Console.WriteLine("Invalid option entered!");
                    break;
            }
        }
    }

    private static async void RegisterSeller()
    {
        var reply = await Client.RegisterSellerAsync(new RegisterSellerRequest
        {
            Address = CurrentSeller.Address,
            Uuid = CurrentSeller.Uuid
        });
        Console.WriteLine(reply.Status);
    }

    private static async void SellItem()
    {
        // long line of I/O
        Console.Write("Enter the product name: ");
        var name = Console.ReadLine()!;

        var category = GetCategory();

        Console.Write("Enter the quantity: ");
        var quantity = int.Parse(Console.ReadLine()!);

        Console.Write("Enter the description: ");
        var description = Console.ReadLine()!;

        Console.Write("Enter the price per unit: ");
        var price = float.Parse(Console.ReadLine()!);

        var reply = await Client.SellItemAsync(new SellItemRequest
        {
            Category = category,
            Description = description,
            PricePerUnit = price,
            ProductName = name,
            Quantity = quantity,
            SellerAddress = CurrentSeller.Address
        });
        Console.WriteLine(reply.Status);
    }

    private static Category GetCategory()
    {
        const string choices = """
                               Select a category:
                               1) Electronics
                               2) Fashion
                               3) Others
                               """;
        Console.WriteLine(choices);
        var choice = int.Parse(Console.ReadLine()!);
        return choice switch
        {
            1 => Category.Electronics,
            2 => Category.Fashion,
            3 => Category.Others,
            _ => Category.All
        };
    }  

    private static async void UpdateItem()
    {
        Console.Write("Enter the product Id to update details: ");
        var id = int.Parse(Console.ReadLine()!);
        
        Console.Write("Enter the new price: ");
        var price = float.Parse(Console.ReadLine()!);
        
        Console.Write("Enter the new quantity: ");
        var quantity = int.Parse(Console.ReadLine()!);
        
    }

    private static async void DeleteItem()
    {
        Console.Write("Enter the Id of the product to delete: ");
        var id = int.Parse(Console.ReadLine()!);
        
        var reply = await Client.DeleteItemAsync(new DeleteItemRequest
        {
            Address = CurrentSeller.Address,
            Id = id,
            Uuid = CurrentSeller.Uuid
        });
        Console.WriteLine(reply.Status);
    }

    private static async void DisplayItems()
    {
        var reply = await Client.DisplaySellerItemsAsync(new DisplaySellerItemsRequest
        {
            Address = CurrentSeller.Address,
            Uuid = CurrentSeller.Uuid
        });
        if (reply.Status == "FAIL")
        {
            Console.WriteLine("FAIL");
            return;
        }
        Console.WriteLine(reply.Output);
    }
}
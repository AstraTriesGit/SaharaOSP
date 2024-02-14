using RpcComm.Buyer;
using RpcComm;

namespace Buyer;

public static class BuyerMenu
{
    public static BuyerToMarket.BuyerToMarketClient Client = null!;
    public static Marketplace.Models.Buyer CurrentBuyer = null!;
    public static MarketNotification.MarketNotificationClient NotificationClient = null!;
    
    private static bool _run = true;
    private const string Menu =
        """
        Please select an option...
        1) Search for items
        2) Buy an item
        3) Add item to wishlist
        4) Rate an item
        5) Exit SaharaOSP...
        """;

    public static void BuyerMain()
    {
        while (_run)
        {
            Console.WriteLine(Menu);
            var input = int.Parse(Console.ReadLine()!);
            switch (input)
            {
                case 1:
                    SearchItems();
                    break;
                case 2:
                    BuyItem();
                    break;
                case 3:
                    AddToWishlist();
                    break;
                case 4:
                    RateItem();
                    break;
                case 5:
                    _run = false;
                    break;
                default:
                    Console.WriteLine("Invalid option entered!");
                    break;
            }
            Thread.Sleep(2000);
        }
    }

    private static async void SearchItems()
    {
        Console.WriteLine("Enter the name (leave blank to get all products): ");
        var name = Console.ReadLine();
        var category = GetCategory();

        var reply = await Client.SearchItemAsync(new SearchItemRequest
        {
            Category = category,
            Name = name
        });
        if (reply.Status == "FAIL")
        {
            Console.WriteLine("FAIL");
            return;
        }

        Console.WriteLine(reply.Items);
    }
    
    private static Category GetCategory()
    {
        const string choices = """
                               Select a category:
                               1) Electronics
                               2) Fashion
                               3) Others
                               4) All
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

    private static async void BuyItem()
    {
        
    }

    private static async void AddToWishlist()
    {
        Console.Write("Enter the Id of the product: ");
        var id = int.Parse(Console.ReadLine()!);

        var reply = await Client.AddToWishListAsync(new WishListRequest
        {
            BuyerAddress = CurrentBuyer.Address,
            Id = id
        });
        Console.WriteLine(reply.Status);
    }

    private static async void RateItem()
    {
        Console.Write("Enter the product Id of the product to be rated: ");
        var id = int.Parse(Console.ReadLine()!);
        
        Console.Write("Enter an integral rating from 1-5, inclusive: ");
        var rating = int.Parse(Console.ReadLine()!);

        if (rating is < 1 or > 5)
        {
            Console.WriteLine("FAIL");
            return;
        }

        var reply = await Client.RateItemAsync(new RateItemRequest
        {
            BuyerAddress = CurrentBuyer.Address,
            Id = id,
            Rating = rating
        });
        Console.WriteLine(reply.Status);
    }
}
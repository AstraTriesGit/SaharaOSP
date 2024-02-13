using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Buyer;
using Grpc.Net.Client;
using RpcComm;
using RpcComm.Buyer;

// It's an attempt to find the one public IPv4 address
var ipv4Addresses = Array.FindAll(
    Dns.GetHostEntry(string.Empty).AddressList,
    a => a.AddressFamily == AddressFamily.InterNetwork);
var list = ipv4Addresses.ToImmutableList();
list = list.Remove(IPAddress.Loopback);

// Create seller credentials here
var address = list[0].ToString();
var uuid = Guid.NewGuid().ToString();
Console.WriteLine($"You're connecting from {address} with uuid {uuid}.");
var buyer = new Marketplace.Models.Buyer(address);

// create communication channels
using var channel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
var client = new BuyerToMarket.BuyerToMarketClient(channel);
var cancellationTokenSource = new CancellationTokenSource();

Console.WriteLine("Welcome to SaharaOSP.");

// set up notifs
using var notificationChannel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
var notificationClient = new MarketNotification.MarketNotificationClient(notificationChannel);

// Start a new task to listen for notifications when seller updates the products
Task.Run(async () =>
{
    using var call = notificationClient.UpdateItem(null);
    while (await call.ResponseStream.MoveNext(cancellationTokenSource.Token))
    {
        var response = call.ResponseStream.Current;
        // Handle the notification here
        Console.WriteLine($"Product with id {response.Id} in your wishlist " +
                          $"now selling at {response.NewPrice}. " +
                          $"Only {response.NewQuantity} left!");
    }
});

// set up the BuyerMenu object
BuyerMenu.CurrentBuyer = buyer;
BuyerMenu.Client = client;
BuyerMenu.NotificationClient = notificationClient;
BuyerMenu.BuyerMain();
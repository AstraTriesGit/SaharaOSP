using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Grpc.Net.Client;
using RpcComm;
using RpcComm.Seller;
using Seller;

// It's an attempt to find the one public IPv4 address
var ipv4Addresses = Array.FindAll(
    Dns.GetHostEntry(string.Empty).AddressList,
    a => a.AddressFamily == AddressFamily.InterNetwork);
var list = ipv4Addresses.ToImmutableList();
list = list.Remove(IPAddress.Loopback);

// Create seller credentials here
var address = list[0].ToString();
var uuid = Guid.NewGuid().ToString();
Console.WriteLine($"You're connecting from {address} with uuid {uuid}");
var seller = new Marketplace.Models.Seller(address, uuid);

// create communication channels
using var channel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
var client = new SellerToMarket.SellerToMarketClient(channel);
var cancellationTokenSource = new CancellationTokenSource();

Console.WriteLine("Welcome to SaharaOSP.");


using var notificationChannel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
var notificationClient = new MarketNotification.MarketNotificationClient(notificationChannel);

// Start a new task to listen for notifications when buyer buys a product
Task.Run(async () =>
{
    using var call = notificationClient.BuyItem(null);
    while (await call.ResponseStream.MoveNext(cancellationTokenSource.Token))
    {
        var response = call.ResponseStream.Current;
        // Handle the notification here
        Console.WriteLine($"Buyer {response.BuyerAddress} just purchased" +
                          $" {response.Quantity} of your product with id {response.Id}");
    }
});

// set up the SellerMenu object
SellerMenu.CurrentSeller = seller;
SellerMenu.Client = client;
SellerMenu.NotificationClient = notificationClient;
SellerMenu.SellerMain();

    
using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Grpc.Net.Client;
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

// set up the SellerMenu object
Console.WriteLine("Welcome to SaharaOSP.");
SellerMenu.CurrentSeller = seller;
SellerMenu.Client = client;
SellerMenu.SellerMain();

var senderTask = Task.Run(async () =>
{
    try
    {
        
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
});

// var receieverTask = Task.Run(async () =>
// {
//     try
//     {
//         await foreach (var update in client)
//     }
//     catch (Exception e)
//     {
//         Console.WriteLine(e);
//         throw;
//     }
// });
    
using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Buyer;
using Grpc.Net.Client;
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
Console.WriteLine($"You're connecting from {address} with uuid {uuid}");
var buyer = new Marketplace.Models.Buyer(address);

// create communication channels
using var channel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
var client = new BuyerToMarket.BuyerToMarketClient(channel);
var cancellationTokenSource = new CancellationTokenSource();

// set up the BuyerMenu object
Console.WriteLine("Welcome to SaharaOSP.");
BuyerMenu.CurrentBuyer = buyer;
BuyerMenu.Client = client;
BuyerMenu.BuyerMain();

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
using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Grpc.Net.Client;
using RpcComm.Seller;

// It's an attempt to find the one public IPv4 address
var ipv4Addresses = Array.FindAll(
    Dns.GetHostEntry(string.Empty).AddressList,
    a => a.AddressFamily == AddressFamily.InterNetwork);
var list = ipv4Addresses.ToImmutableList();
list = list.Remove(IPAddress.Loopback);

// Create seller credentials here
var address = list[0].ToString();
var uuid = Guid.NewGuid().ToString();

 using var channel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
 var client = new SellerToMarket.SellerToMarketClient(channel);
 var reply = await client.RegisterSellerAsync(
     new RegisterSellerRequest 
     {
         Address = address,
         Uuid = uuid 
     }
     );

Console.WriteLine(reply.Status);
    
using Grpc.Net.Client;
using RpcComm.Seller;

using var channel = GrpcChannel.ForAddress("http://127.0.0.1:6969");
var client = new SellerToMarket.SellerToMarketClient(channel);
var reply = await client.RegisterSellerAsync(
    new RegisterSellerRequest 
    {
        Address = "Hell",
        Uuid = Guid.NewGuid().ToString() 
    }
    );

Console.WriteLine(reply.Status);
    
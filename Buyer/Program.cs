// See https://aka.ms/new-console-template for more information

using Grpc.Net.Client;


Console.WriteLine("Hello, World!");

using var connection = GrpcChannel.ForAddress("http://127.0.0.1:6969");

using Grpc.Core;
using RpcComm;
using Marketplace.Models;

namespace Marketplace.Services;

public class MarketNotificationService : MarketNotification.MarketNotificationBase
{
    private readonly ILogger _logger;

    public MarketNotificationService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MarketNotificationService>();
    }

    public override async Task BuyItem(BuyItemRequest request, IServerStreamWriter<BuyItemResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Buy request quantity {} of item {}, from {}",
            request.Quantity, request.Id, request.BuyerAddress);
        
        // do the warehouse updating
        
        foreach (var products in Market.SellerInventory.Values)
        {
            var productToBuy = products.FirstOrDefault(product => product.Id == request.Id);
            if (productToBuy == null) continue;
            if (productToBuy.Quantity > request.Quantity)
            {
                productToBuy.Quantity -= request.Quantity;
                _logger.LogDebug("Buy request quantity {} of item {}, from {} succeeded",
                    request.Quantity, request.Id, request.BuyerAddress);
                await responseStream.WriteAsync(new BuyItemResponse
                {
                    BuyerAddress = request.BuyerAddress,
                    Quantity = request.Quantity,
                    Id = request.Id,
                    Status = "SUCCESS"
                });
                return;
            }
            _logger.LogWarning("Buy request quantity {} of item {}, from {} failed: " +
                               "Quantity requested not available",
                request.Quantity, request.Id, request.BuyerAddress);
            break;
        }
        
        _logger.LogDebug("Buy request quantity {} of item {}, from {} failed: " +
                         "Product with Id not found",
            request.Quantity, request.Id, request.BuyerAddress);
    }

    public override async Task UpdateItem(UpdateItemRequest request, IServerStreamWriter<UpdateItemResponse> responseStream, ServerCallContext context)
    {
        
    }
}
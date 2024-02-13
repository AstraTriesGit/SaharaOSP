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
        _logger.LogInformation("Update Item {} request from {}",
            request.Id, request.Address);
        
        var productToUpdate = Market.SellerInventory.Values.SelectMany(products => products)
            .FirstOrDefault(product => product.Id == request.Id);
        if (productToUpdate != null)
        {
            productToUpdate.PricePerUnit = new decimal(request.NewPrice);
            productToUpdate.Quantity = request.NewQuantity;
            _logger.LogDebug("Update Item {} request from {} succeeded", request.Id, request.Address);
            
        }
        else
        {
            _logger.LogWarning("Update Item {} request from {} failed: " + "Product with Id not found", request.Id,
                request.Address);
        }
        
        // find the buyers whose Market.Wishlists[buyer] contains productToUpdate using LINQ
        var buyersToUpdate = Market.Wishlists.Where(wishlist => wishlist.Value.Contains(productToUpdate))
            .Select(wishlist => wishlist.Key);
        foreach (var buyer in buyersToUpdate)
        {
            await responseStream.WriteAsync(new UpdateItemResponse
            {
                BuyerId = buyer.Address,
                Id = request.Id,
                NewPrice = request.NewPrice,
                NewQuantity = request.NewQuantity
            });
        }
    }
}
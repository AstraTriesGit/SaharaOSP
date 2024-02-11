/* 1) Log the event.
 * 2) Authenticate.
 * 3) Update internal state.
 * 4) Send response to seller.
 */

using Grpc.Core;
using RpcComm.Seller;
using Marketplace.Models;


namespace Marketplace.Services;

public class SellerToMarketService : SellerToMarket.SellerToMarketBase
{
    private readonly ILogger _logger;

    public SellerToMarketService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SellerToMarketService>();
    }

    /*
     * We are not accepting multiple registrations from the same person!
     * Returns SUCCESS if the seller is not registered, else FAIL.
     */
    public override Task<RegisterSellerResponse> RegisterSeller(RegisterSellerRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("Seller join request from {}, uuid = {}", request.Address, request.Uuid);

        var seller = new Seller(request.Address, request.Uuid);
        if (Market.SellerInventory.ContainsKey(seller))
        {
            _logger.LogWarning("Seller join request from {}, uuid = {} failed: " +
                                   "Seller already exists", request.Address, request.Uuid);
            return Task.FromResult(new RegisterSellerResponse { Status = "FAIL" });
        }

        Market.SellerInventory.Add(seller, []);
        
        _logger.LogDebug("Seller join request from {}, uuid = {} succeeded", request.Address, request.Uuid);
        return Task.FromResult(new RegisterSellerResponse { Status = "SUCCESS" });

    }

    /*
     * Returns SUCCESS if the seller is registered and object updated, else FAIL.
     */
    public override Task<SellItemResponse> SellItem(SellItemRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("Sell Item request from {}", request.SellerAddress);

        var seller = new Seller(request.SellerAddress, request.SellerUuid);
        if (!Market.SellerInventory.TryGetValue(seller, out var value))
        {
            _logger.LogWarning("Sell Item request from {} failed: " +
                               "Seller not found", request.SellerAddress);
            return Task.FromResult(new SellItemResponse { Status = "FAIL" });
        }
        
        var newProduct = new Product(
            request.ProductName, request.Category, request.Quantity,
            request.Description, request.SellerAddress, new decimal(request.PricePerUnit)
            );
        value.Add(newProduct);
        
        _logger.LogDebug("Sell Item request from {}", request.SellerAddress);
        return Task.FromResult(new SellItemResponse{ Status = "SUCCESS" });
    }

    /*
     * 
     */
    public override Task<UpdateItemResponse> UpdateItem(UpdateItemRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("Update Item {} request from {}",request.Id, request.Address);

        var seller = new Seller(request.Address, request.Uuid);
        if (!Market.SellerInventory.TryGetValue(seller, out var value))
        {
            _logger.LogWarning("Update Item {} request from {} failed: " +
                                   "Seller not registered",request.Id, request.Address);
            return Task.FromResult(new UpdateItemResponse { Status = "FAIL" });
        }

        if (Market.SellerInventory[seller].Where(product => product.Id == request.Id).ToHashSet().Count == 0)
        {
            _logger.LogWarning("Update Item {} request from {} failed: " +
                "Product with Id not found",request.Id, request.Address);
            return Task.FromResult(new UpdateItemResponse { Status = "FAIL" });
        }
        foreach (var product in Market.SellerInventory[seller].Where(product => product.Id == request.Id))
        {
            product.PricePerUnit = request.NewPrice;
            product.Quantity = request.NewQuantity;
        }
        
        _logger.LogDebug("Update Item {} request from {} succeeded",request.Id, request.Address);
        return Task.FromResult(new UpdateItemResponse { Status = "SUCCESS" });
    }

    public override Task<DeleteItemResponse> DeleteItem(DeleteItemRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation(@"Delete Item {} request from {}", request.Id, request.Address);
        var seller = new Seller(request.Address, request.Uuid);
        if (!Market.SellerInventory.ContainsKey(seller))
        {
            _logger.LogWarning(@"Delete Item {} request from {} failed: " + 
                "Seller not found", request.Id, request.Address);
            return Task.FromResult(new DeleteItemResponse { Status = "FAIL" });
        }
        
        if (Market.SellerInventory[seller].Where(product => product.Id == request.Id).ToHashSet().Count == 0)
        {
            _logger.LogWarning("Delete Item {} request from {} failed: " +
                               "Product with Id not found",request.Id, request.Address);
            return Task.FromResult(new DeleteItemResponse { Status = "FAIL" });
        }
        foreach (var product in Market.SellerInventory[seller].Where(product => product.Id == request.Id))
        {
            Market.SellerInventory[seller].Remove(product);
        }
        _logger.LogDebug("Delete Item {} request from {} succeeded", request.Id, request.Address);
        return Task.FromResult(new DeleteItemResponse { Status = "SUCCESS" });
    }

    public override Task<DisplaySellerItemsResponse> DisplaySellerItems(DisplaySellerItemsRequest request,
        ServerCallContext ctx)
    {
        _logger.LogInformation("Display Items request from {}", request.Address);

        var seller = new Seller(request.Address, request.Uuid);
        if (!Market.SellerInventory.TryGetValue(seller, out var value))
        {
            _logger.LogWarning("Display Items request from {} failed: " +
                               "Seller not found", request.Address);
            return Task.FromResult(new DisplaySellerItemsResponse { Output = "", Status = "FAIL" });
        }

        var output = "";
        foreach (var product in value)
        {
            output += product.ToString();
            output += "\n______________\n";
        }

        _logger.LogDebug("Display Items request from {} succeeded", request.Address);
        return Task.FromResult(new DisplaySellerItemsResponse
        {
            Output = output,
            Status = "SUCCESS"
        });
    }
    
}
using Grpc.Core;
using RpcComm;
using RpcComm.Buyer;
using Marketplace.Models;

namespace Marketplace.Services;

public class BuyerToMarketService : BuyerToMarket.BuyerToMarketBase
{
    private readonly ILogger _logger;

    public BuyerToMarketService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BuyerToMarketService>();
    }

    public override Task<SearchItemResponse> SearchItem(SearchItemRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("Search request for Item name: {}, Category: {}",
            request.Name, request.Category);

        if (request.Category == Category.All)
        {
            var allOutput = "";
            foreach (var product in 
                     from productList in Market.SellerInventory.Values
                     from product in productList select product)
            {
                allOutput += product.ToString();
                allOutput += "\n______________\n";
            }
            _logger.LogDebug("Search request for Item name: {}, Category: {} succeeded",
                request.Name, request.Category);
            return Task.FromResult(new SearchItemResponse{Items = allOutput, Status = "SUCCESS"});
        }

        var output = "";
        foreach (var product in 
                 from productList in Market.SellerInventory.Values 
                 from product in productList 
                 where product.ProductName == request.Name && product.Category == request.Category
                 select product)
        {
            output += product.ToString();
            output += "\n______________\n";
        }
        _logger.LogDebug("Search request for Item name: {}, Category: {} succeeded",
            request.Name, request.Category);
        return Task.FromResult(new SearchItemResponse{Items = output, Status = "SUCCESS"});
    }

    public override Task<WishListResponse> AddToWishList(WishListRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("Wishlist request of item {} from {}",
            request.Id, request.BuyerAddress);
        
        // use Market.Buyers to find the buyer
        var foundBuyer = Market.Buyers.FirstOrDefault(
            b => b.Address == request.BuyerAddress
        ); 
        if (foundBuyer == null)
        {
            foundBuyer = new Buyer(request.BuyerAddress);
            Market.Buyers.Add(foundBuyer);
        }
        
        var foundProduct = Market.SellerInventory
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(product => product.Id == request.Id);
        if (foundProduct != null)
        {
            if (!foundBuyer.Wishlist.Contains(foundProduct))
            {
                foundBuyer.Wishlist.Add(foundProduct);
                
                _logger.LogInformation("Wishlist request of item {} from {} succeeded",
                    request.Id, request.BuyerAddress);
                return Task.FromResult(new WishListResponse { Status = "SUCCESS" });
            }
            _logger.LogWarning("Wishlist request of item {} from {} failed: " +
                               "Product already in wishlist",
                request.Id, request.BuyerAddress);
            return Task.FromResult(new WishListResponse { Status = "FAIL" });
        }
        
        _logger.LogWarning("Wishlist request of item {} from {} failed: " +
                           "Product with Id not found",
            request.Id, request.BuyerAddress);
        return Task.FromResult(new WishListResponse { Status = "FAIL" });
    }
    

    public override Task<RateItemResponse> RateItem(RateItemRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("{} rating item request for {}[id] with {} stars",
            request.BuyerAddress, request.Id, request.Rating);
        
        // use Market.Buyers to find the buyer
        var foundBuyer = Market.Buyers.FirstOrDefault(
            b => b.Address == request.BuyerAddress
            ); 
        if (foundBuyer == null)
        {
            foundBuyer = new Buyer(request.BuyerAddress);
            Market.Buyers.Add(foundBuyer);
        }
        
        var foundProduct = Market.SellerInventory
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(product => product.Id == request.Id);
        if (foundProduct != null)
        {
            if (!foundBuyer.RatedProducts.Contains(foundProduct))
            {
                foundProduct.Rating += (request.Rating - foundProduct.Rating) 
                                       / ++foundProduct.NoOfRatings;
                foundBuyer.RatedProducts.Add(foundProduct);
                
                _logger.LogInformation("{} rating item request for {}[id] with {} stars succeeded",
                    request.BuyerAddress, request.Id, request.Rating);
                return Task.FromResult(new RateItemResponse { Status = "SUCCESS" });
            }
            _logger.LogWarning("{} rating item request for {}[id] with {} stars failed: " +
                               "Product already rated",
                request.BuyerAddress, request.Id, request.Rating);
            return Task.FromResult(new RateItemResponse { Status = "FAIL" });
        }
        
        _logger.LogWarning("{} rating item request for {}[id] with {} stars failed: " +
                           "Product with Id not found",
            request.BuyerAddress, request.Id, request.Rating);
        return Task.FromResult(new RateItemResponse { Status = "FAIL" });
    }    
    
}
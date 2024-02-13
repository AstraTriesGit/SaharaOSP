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

    public override async Task BuyItem(IAsyncStreamReader<BuyItemRequest> requestStream, IServerStreamWriter<BuyItemResponse> responseStream, ServerCallContext context)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            _logger.LogInformation("Buy request {} of item {}[item id], from {}",
                request.Quantity, request.Id, request.BuyerAddress);
            var buyResponse = NotifySeller(request);
            await responseStream.WriteAsync(buyResponse);
        }
    }

    private static BuyItemResponse NotifySeller(BuyItemRequest request)
    {
        foreach (var sellerInventory in Market.SellerInventory)
        {
            foreach (var product in sellerInventory.Value.Where(product => product.Id == request.Id))
            {
                product.Quantity -= request.Quantity;
                break;
            }
        }

        return new BuyItemResponse { BuyerAddress = request.BuyerAddress, Status = "SUCCESS"};
    }

    public override Task<WishListResponse> AddToWishList(WishListRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("Wishlist request of item {}, from {}",
            request.Id, request.BuyerAddress);
        
        // add buyer to wishlists if not done yet
        var buyer = new Buyer(request.BuyerAddress);
        if (!Market.Wishlists.TryGetValue(buyer, out var value))
        {
            value = [];
            Market.Wishlists.Add(buyer, value);
        }
        
        // check if product exists
        if (!Market.SellerInventory.Values.Any(
                products => products.Any(product => product.Id == request.Id)
            ))
        {
            _logger.LogWarning("Wishlist request of item {}, from {} failed:" +
                               "Product with Id not found",
                request.Id, request.BuyerAddress);
            return Task.FromResult(new WishListResponse { Status = "FAIL" });
        }
        
        // find that product
        foreach (var wishlistProduct in Market.SellerInventory.Values.Select(productList => productList.FirstOrDefault(
                     product => product.Id == request.Id
                 )).OfType<Product>())
        {
            if (!value.Contains(wishlistProduct))
            {
                value.Add(wishlistProduct);
            }
            break;
        }
        
        _logger.LogDebug("Wishlist request of item {}, from {} succeeded",
                         request.Id, request.BuyerAddress);
        return Task.FromResult(new WishListResponse { Status = "SUCCESS" });
    }

    public override Task<RateItemResponse> RateItem(RateItemRequest request, ServerCallContext ctx)
    {
        _logger.LogInformation("{} rating item request for {}[id] with {} stars",
            request.BuyerAddress, request.Id, request.Rating
            );
        
        
        // add buyer to wishlists if not done yet
        var buyer = new Buyer(request.BuyerAddress);
        if (!Market.RatedProducts.TryGetValue(buyer, out var value))
        {
            value = [];
            Market.RatedProducts.Add(buyer, value);
        }
        
        // check if product exists
        if (!Market.SellerInventory.Values.Any(
                products => products.Any(product => product.Id == request.Id)
            ))
        {
            _logger.LogWarning("{} rating item request for {}[id] with {} stars failed: " +
                               "Product with Id not found",
                request.BuyerAddress, request.Id, request.Rating
            );
            return Task.FromResult(new RateItemResponse { Status = "FAIL" });
        }
        
        // now do the product updating
        foreach (var productList in Market.SellerInventory.Values)
        {
            var productToRate = productList.FirstOrDefault(
                product => product.Id == request.Id
            );
            if (productToRate == null) continue;
            
            // found you!
            if (!Market.RatedProducts[buyer].Contains(productToRate))
            {
                productToRate.Rating += (request.Rating - productToRate.Rating) / ++productToRate.NoOfRatings;
                Market.RatedProducts[buyer].Add(productToRate);
            }
            break;
        }
        
        _logger.LogDebug("{} rating item request for {}[id] with {} stars succeeded ",
            request.BuyerAddress, request.Id, request.Rating
        );
        return Task.FromResult(new RateItemResponse { Status = "FAIL" });
    }
    
    
}
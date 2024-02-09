using Buyer;
using Grpc.Core;

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
        
    }

    public override Task<BuyItemResponse> BuyItem(BuyItemRequest request, ServerCallContext ctx)
    {
        
    }

    public override Task<WishListResponse> AddToWishList(WishListRequest request, ServerCallContext ctx)
    {
        
    }

    public override Task<RateItemResponse> RateItem(RateItemRequest request, ServerCallContext ctx)
    {
        
    }



}
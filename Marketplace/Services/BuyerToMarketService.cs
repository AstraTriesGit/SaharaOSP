using Grpc.Core;
using RpcComm.Buyer;

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
        return null;
    }

    public override Task<BuyItemResponse> BuyItem(BuyItemRequest request, ServerCallContext ctx)
    {
        return null;
    }

    public override Task<WishListResponse> AddToWishList(WishListRequest request, ServerCallContext ctx)
    {
        return null;
    }

    public override Task<RateItemResponse> RateItem(RateItemRequest request, ServerCallContext ctx)
    {
        return null;
    }



}
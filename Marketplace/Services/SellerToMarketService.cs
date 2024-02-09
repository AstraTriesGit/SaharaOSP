using Grpc.Core;

namespace Marketplace.Services;

public class SellerToMarketService : SellerToMarket.SellerToMarketBase
{
    private readonly ILogger _logger;

    public SellerToMarketService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SellerToMarketService>();
    }

    public override Task<RegisterSellerResponse> RegisterSeller(RegisterSellerRequest request, ServerCallContext ctx)
    {
        
    }

    public override Task<SellItemResponse> SellItem(SellItemRequest request, ServerCallContext ctx)
    {
        
    }

    public override Task<UpdateItemResponse> UpdateItem(UpdateItemRequest request, ServerCallContext ctx)
    {
        
    }

    public override Task<DeleteItemResponse> DeleteItem(DeleteItemRequest request, ServerCallContext ctx)
    {
        
    }

    public override Task<DisplaySellerItemsResponse> DisplaySellerItems(DisplaySellerItemsRequest request,
        ServerCallContext ctx)
    {
        
    }
    
}
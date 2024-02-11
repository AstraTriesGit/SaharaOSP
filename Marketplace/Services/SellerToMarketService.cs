using Grpc.Core;
using RpcComm.Seller;

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
        _logger.LogInformation("Registering {RequestUuid} at address {RequestAddress}", request.Uuid, request.Address);
        return Task.FromResult(new RegisterSellerResponse
        {
            Status = $"What's up, {request.Uuid} from {request.Address}!"
        });
    }

    public override Task<SellItemResponse> SellItem(SellItemRequest request, ServerCallContext ctx)
    {
        return null;
    }

    public override Task<UpdateItemResponse> UpdateItem(UpdateItemRequest request, ServerCallContext ctx)
    {
        return null;
    }

    public override Task<DeleteItemResponse> DeleteItem(DeleteItemRequest request, ServerCallContext ctx)
    {
        return null;
    }

    public override Task<DisplaySellerItemsResponse> DisplaySellerItems(DisplaySellerItemsRequest request,
        ServerCallContext ctx)
    {
        return null;
    }
    
}
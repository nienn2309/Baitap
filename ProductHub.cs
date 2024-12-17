using Baitap.Models;
using Microsoft.AspNetCore.SignalR;

namespace Baitap
{
    public class ProductHub : Hub
    {
        public async Task NotifyProductCreated(Product product)
        {
            await Clients.All.SendAsync("ReceiveProductCreated", product);
        }

        public async Task NotifyProductDeleted(string productId)
        {
            await Clients.All.SendAsync("ReceiveProductDeleted", productId);
        }
    }
}

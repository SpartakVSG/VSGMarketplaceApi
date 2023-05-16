﻿using Microsoft.Extensions.Caching.Memory;
using VSGMarketplaceApi.Data.Models;
using VSGMarketplaceApi.ViewModels;

namespace VSGMarketplaceApi.Services.Interfaces
{
    public interface IOrdersService
    {
        public Task<string> BuyAsync(NewOrderAddModel input);

        public Task<string> CompleteAsync(int code);

        public Task<string> DeleteAsync(int code, int userId);

        public Task<IEnumerable<PendingOrderViewModel>> GetAllPendingOrdersAsync();

        public Task<Order> GetByCodeAsync(int code);

        public Task<IEnumerable<MyOrdersViewModel>> GetByUserId(int userId);

    }
}

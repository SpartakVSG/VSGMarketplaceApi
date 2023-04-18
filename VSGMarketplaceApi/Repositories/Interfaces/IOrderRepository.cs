﻿using VSGMarketplaceApi.DTOs;
using VSGMarketplaceApi.Models;

namespace VSGMarketplaceApi.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByCodeAsync(int code);
        Task<int> AddAsync(NewOrderAddModel entity);
        Task<int> CompleteAsync(int code);
        Task<int> DeleteAsync(int code, int userId);
        Task<IEnumerable<MyOrdersViewModel>> GetByUserId(int userId);
        Task<IEnumerable<PendingOrderViewModel>> GetAllPendingOrdersAsync();
    }
}
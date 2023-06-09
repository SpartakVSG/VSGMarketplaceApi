﻿using Data.Models;
using Data.Repositories.Interfaces;
using Data.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Services.Interfaces;
using System.Transactions;

namespace Services
{
    public class LoansService : ILoansService
    {
        private readonly ILoanRepository loanRepository;
        private readonly IItemRepository itemRepository;
        private readonly IValidator<Loan> validator;
        private IMemoryCache memoryCache;

        private HttpContextAccessor httpContextAccessor;

        public LoansService(ILoanRepository loanRepository, IValidator<Loan> validator, IItemRepository itemRepository, IMemoryCache memoryCache)
        {
            this.loanRepository = loanRepository;
            this.validator = validator;
            this.itemRepository = itemRepository;
            this.memoryCache = memoryCache;

            this.httpContextAccessor = new HttpContextAccessor();
        }

        public async Task<List<LentItemViewModel>> GetAllLoansAsync()
        {
            if (memoryCache.TryGetValue(Constants.LOANS_CACHE_KEY, out List<LentItemViewModel> loans))
            {
                return loans;
            }

            Loan[] allLoansArray = await this.loanRepository.GetAllLoansAsync();

            var emailsAndLoans = allLoansArray.GroupBy(x => x.OrderedBy);
            List<LentItemViewModel> outputLoans = new List<LentItemViewModel>();

            foreach (var item in emailsAndLoans)
            {
                outputLoans.Add(new LentItemViewModel { Email = item.Key, Count = item.Count() });
            }

            var options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            memoryCache.Set(Constants.INVENTORY_ITEMS_CACHE_KEY, outputLoans, options);

            return outputLoans;
        }

        public async Task<Loan[]> GetMyLoansAsync(string userEmail)
        {
            var email = httpContextAccessor.HttpContext.User.Claims.First(x => x.Value.Contains("vsgbg.com")).Value;

            if (email.ToLower() != userEmail)
            {
                if (!httpContextAccessor.HttpContext.User.Claims.Any(x => x.Value == Constants.AdminGroup))
                {
                    throw new Exception("You are not an admin!");
                };
            }

            if (memoryCache.TryGetValue(Constants.LOANS_CACHE_KEY + userEmail, out Loan[] myLoans))
            {
                return myLoans;
            }

            Loan[] outputLoans = await this.loanRepository.GetLoansByUserEmailAsync(userEmail);

            var options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            memoryCache.Set(Constants.LOANS_CACHE_KEY + userEmail, outputLoans, options);

            return outputLoans;
        }

        public async Task LoanAsync(int itemId, CreateLoanViewModel input)
        {
            Loan loan = new Loan
            {
                LoanStartDate = DateTime.Now,
                OrderedBy = input.OrderedBy.ToLower(), //remove when add asure accounts
                Quantity = int.Parse(input.Quantity),
                LoanEndDate = null,
                ItemId = itemId,
            };

            var validationResult = validator.Validate(loan);
            if (!validationResult.IsValid) { throw new Exception(Constants.ValidationError); }

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int changesByCreatingLoan = await this.loanRepository.CreateLoanAsync(loan);

                if (changesByCreatingLoan == 0)
                {
                    throw new Exception("Loan not approved!");
                }

                int changesByReducingAvailableQuantity = await this.itemRepository.ReduceAvailableQuantity(loan);

                if (changesByReducingAvailableQuantity == 0)
                {
                    throw new Exception("Not available quantity!");
                }

                transactionScope.Complete();
                transactionScope.Dispose();
            }

            memoryCache.Remove(Constants.INVENTORY_ITEMS_CACHE_KEY);
            memoryCache.Remove(Constants.LOANS_CACHE_KEY);
            memoryCache.Remove(Constants.LOANS_CACHE_KEY + input.OrderedBy);
        }

        public async Task ReturnLoanAsync(int loanId)
        {
            int changes = await this.loanRepository.ReturnLoanAsync(loanId);

            if (changes == 0)
            {
                throw new Exception("Zero changes");
            }

            string userEmail = await this.loanRepository.GetUserEmailByLoanId(loanId);

            memoryCache.Remove(Constants.INVENTORY_ITEMS_CACHE_KEY);
            memoryCache.Remove(Constants.LOANS_CACHE_KEY);
            memoryCache.Remove(Constants.LOANS_CACHE_KEY + userEmail);
        }
    }
}

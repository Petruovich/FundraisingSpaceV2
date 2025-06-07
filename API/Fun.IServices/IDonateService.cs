using Fun.Application.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IServices
{
    public interface IDonateService
    {
        //Task DonateAsync(int fundraisingId, decimal amount, int userId);
        Task/*<string>*/ DonateAsync(int fundraisingId, decimal amount, int userId);
        Task<List<DonorResponseModel>> GetTopDonorsAsync(int fundraisingId);
        Task<List<DonateResponseModel>> GetMyDonationsDetailedAsync();
    }
}

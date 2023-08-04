using Microsoft.IdentityModel.Tokens;
using Model;
using Model.EFDataAccess;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IUserService
    {
        Task<List<UserDetailsModel>> GetAllUsers();
        Task<UserDetailsList> GetUserList(DataTableParams dataTableParams, DateTime? fromDate, DateTime? toDate,string? locationIds, string? locationBy);
        Task<Int64> SaveUserDetails(UserDetailsModel userDetailsModel);
        Task<UserDetailsModel> GetUserById(Int64 userId);
        Task<bool> DeleteUserById(Int64 userId);
        Task<bool> CheckUserNameExist(Int64 userId, string userName);
        Int64 LogOutUser(Int64 userId);
        Task<Int64> UpdateOrClearLoginAttempt(UserDetailsModel userDetails, bool isClear = false);
        int ConvertDateTimeToMinute(DateTime? lastAttemptTime);
        Task<UserDetailsModel> LoginUser(string userName);
        Task<List<Country>> GetCountryList();
        Task<List<State>> GetStateListByCountryId(Int64 countryId);
        Task<List<City>> GetCityListByStateId(Int64 stateId);



        Task<List<City>> GetAllCities();

    }
}

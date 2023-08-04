using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.EFDataAccess;
using System.Linq.Dynamic.Core;
using System.Data;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly UserManagementContext _userManagementContext;
        public UserService(UserManagementContext userManagementContext)
        {
            _userManagementContext = userManagementContext;
        }


        public async Task<UserDetailsModel> GetUserById(Int64 userId)
        {

            var userDetail = await (from UserDetails u in _userManagementContext.UserDetails
                                    join Address ad in _userManagementContext.Address on u.AddressId equals ad.AddressId
                                    into result1
                                    from ad in result1.DefaultIfEmpty()
                                    join Country c in _userManagementContext.Country on ad.CountryId equals c.CountryId
                                    join State s in _userManagementContext.State on ad.StateId equals s.StateId
                                    join City ct in _userManagementContext.City on ad.CityId equals ct.CityId
                                    where u.UserId == userId
                                    select new UserDetailsModel
                                    {
                                        UserId = u.UserId,
                                        UserName = u.UserName,
                                        Password = u.Password,
                                        Gender = u.Gender.ToString(),
                                        Email = u.Email,
                                        BirthDate = u.BirthDate,
                                        MobileNo = u.MobileNo,
                                        ProfileImageBlob = u.ProfileImageBlob,
                                        ProfileImageName = u.ProfileImageName,
                                        IsActive = u.IsActive,
                                        RoleId = u.RoleId,
                                        CreatedDate = u.CreatedDate,
                                        ModifiedDate = u.ModifiedDate,
                                        AddressId = u.AddressId,
                                        RefreshToken = u.RefreshToken,
                                        JwtToken = u.JwtToken,
                                        Address = new AddressModel
                                        {
                                            ZipCode = ad.ZipCode,
                                            CountryName = c.CountryName,
                                            StateName = s.StateName,
                                            CityName = ct.CityName,
                                            CountryId = c.CountryId,
                                            StateId = s.StateId,
                                            CityId = ct.CityId,
                                            AddressId = ad.AddressId,
                                        }
                                    }).FirstOrDefaultAsync();

            return userDetail;

        }
        public async Task<bool> CheckUserNameExist(long userId, string userName)
        {
            int count = await _userManagementContext.UserDetails.Where(x => x.UserId != userId && x.UserName == userName).CountAsync();
            return count == 0;
        }
        public async Task<UserDetailsList> GetUserList(DataTableParams dataTableParams, DateTime? fromDate, DateTime? toDate, string? locationIds, string? locationBy)
        {

            var query = (from UserDetails u in _userManagementContext.UserDetails
                         join Address ad in _userManagementContext.Address on u.AddressId equals ad.AddressId
                         into result1
                         from ad in result1.DefaultIfEmpty()
                         join Country c in _userManagementContext.Country on ad.CountryId equals c.CountryId
                         join State s in _userManagementContext.State on ad.StateId equals s.StateId
                         join City ct in _userManagementContext.City on ad.CityId equals ct.CityId
                         where !u.IsDeleted
                         select new UserDetailsModel
                         {
                             UserId = u.UserId,
                             UserName = u.UserName,
                             Gender = u.Gender.ToString() == "M" ? "Male" : "Female",
                             IsActive = u.IsActive,
                             CreatedDate = u.CreatedDate,
                             ModifiedDate = u.ModifiedDate,
                             BirthDate = u.BirthDate,
                             ProfileImageBlob = u.ProfileImageBlob,
                             Email = u.Email,
                             MobileNo = u.MobileNo,
                             LoginAttemptDateTime = u.LoginAttemptDateTime ?? DateTime.Now,
                             Address = new AddressModel
                             {
                                 ZipCode = ad.ZipCode,
                                 CountryName = c.CountryName,
                                 StateName = s.StateName,
                                 CityName = ct.CityName,
                                 CountryId = ad.CountryId,
                                 StateId = ad.StateId,
                                 CityId = ad.CityId,
                             }
                         }).AsQueryable();

            if (fromDate != null && toDate != null)
                query = query.Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate);

            if (!string.IsNullOrEmpty(locationIds) && !string.IsNullOrEmpty(locationBy))
            {
                Int64[] arrIds = Common.ConvertStringToInt64Array(locationIds);
                switch (locationBy.ToLower())
                {
                    //to avoid null error
                    case string s when locationBy == LocationEnum.City:
                        query = query.Where(x => arrIds.Contains(x.Address.CityId));
                        break;
                    case string s when locationBy == LocationEnum.State:
                        query = query.Where(x => arrIds.Contains(x.Address.StateId));
                        break;
                    case string s when locationBy == LocationEnum.Country:
                        query = query.Where(x => arrIds.Contains(x.Address.CountryId));
                        break;
                }
            }

            if (!string.IsNullOrEmpty(dataTableParams.sortColumn))
            {
                query = query.OrderBy(dataTableParams.sortColumn + " " + dataTableParams.sortColumnDir);
            }
            else
            {
                query = query.OrderByDescending(x => x.UserId);
            }
            if (!string.IsNullOrEmpty(dataTableParams.searchValue) && !string.IsNullOrEmpty(dataTableParams.searchIn))
            {
                switch (dataTableParams.searchIn.ToLower())
                {
                    case UserDetailsEnum.UserName:
                        query = query.Where(x => x.UserName.Contains(dataTableParams.searchValue));
                        break;
                    case UserDetailsEnum.Email:
                        query = query.Where(x => x.Email.Contains(dataTableParams.searchValue));
                        break;
                }
            }

            PaginationModel<UserDetailsModel> paginationModel = await PaginationHelper<UserDetailsModel>.CreateList(query, dataTableParams.CurrentPage, dataTableParams.PageSize);

            UserDetailsList userDetailsList = new UserDetailsList();
            userDetailsList.lstUserDetails = paginationModel.DataList;
            userDetailsList.TotalCount = paginationModel.TotalCount;
            userDetailsList.CurrentPage = paginationModel.CurrentPage;
            userDetailsList.TotalPages = paginationModel.TotalPages;
            userDetailsList.PageSize = paginationModel.PageSize;
            return userDetailsList;
        }


        public async Task<Int64> SaveUserDetails(UserDetailsModel userDetailsModel)
        {
            if (userDetailsModel.ProfileImageFile != null && userDetailsModel.ProfileImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    userDetailsModel.ProfileImageFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    userDetailsModel.ProfileImageBlob = Convert.ToBase64String(fileBytes);
                    userDetailsModel.ProfileImageName = userDetailsModel.ProfileImageFile.FileName;
                }
            }
            UserDetails efUserDetails = new UserDetails();
            efUserDetails.UserId = userDetailsModel.UserId;
            efUserDetails.UserName = userDetailsModel.UserName;
            efUserDetails.Password = userDetailsModel.Password;
            efUserDetails.Email = userDetailsModel.Email;
            efUserDetails.MobileNo = userDetailsModel.MobileNo;
            efUserDetails.BirthDate = userDetailsModel.BirthDate;
            efUserDetails.Gender = userDetailsModel.Gender?.ToCharArray()[0];
            efUserDetails.IsActive = userDetailsModel.IsActive;
            efUserDetails.RoleId = userDetailsModel.RoleId;
            efUserDetails.ProfileImageName = userDetailsModel.ProfileImageName;
            efUserDetails.ProfileImageBlob = userDetailsModel.ProfileImageBlob;


            efUserDetails.Address.AddressId = userDetailsModel.AddressId;
            efUserDetails.Address.CountryId = userDetailsModel.Address.CountryId;
            efUserDetails.Address.StateId = userDetailsModel.Address.StateId;
            efUserDetails.Address.CityId = userDetailsModel.Address.CityId;
            efUserDetails.Address.ZipCode = userDetailsModel.Address.ZipCode;

            using (var transaction = _userManagementContext.Database.BeginTransaction())
            {
                try
                {
                    _userManagementContext.ChangeTracker.Clear();

                    if (userDetailsModel.UserId == 0)
                    {
                        efUserDetails.CreatedDate = userDetailsModel.CreatedDate;
                        efUserDetails.CreatedBy = userDetailsModel.CreatedBy;

                        _userManagementContext.Address.Add(efUserDetails.Address);
                        await _userManagementContext.SaveChangesAsync();
                        efUserDetails.AddressId = efUserDetails.Address.AddressId;
                        _userManagementContext.UserDetails.Add(efUserDetails);
                        await _userManagementContext.SaveChangesAsync();

                    }
                    else
                    {
                        efUserDetails.ModifiedDate = userDetailsModel.ModifiedDate;
                        efUserDetails.ModifiedBy = userDetailsModel.ModifiedBy;

                        _userManagementContext.Entry(efUserDetails.Address).State = EntityState.Modified;
                        await _userManagementContext.SaveChangesAsync();
                        _userManagementContext.Entry(efUserDetails).State = EntityState.Modified;
                        await _userManagementContext.SaveChangesAsync();
                    }
                    transaction.Commit();
                    return efUserDetails.UserId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Int64 LogOutUser(Int64 userId)
        {

            UserDetails efUserDetails = new UserDetails();
            _userManagementContext.ChangeTracker.Clear();

            efUserDetails.UserId = userId;
            efUserDetails.RefreshToken = null;
            efUserDetails.RefreshToken = null;
            _userManagementContext.Entry(efUserDetails).Property(x => x.RefreshToken).IsModified = true;
            _userManagementContext.Entry(efUserDetails).Property(x => x.JwtToken).IsModified = true;
            //_userManagementContext.Entry(efUserDetails).State = EntityState.Modified;
            _userManagementContext.SaveChanges();

            return efUserDetails.UserId;
        }
        public async Task<Int64> UpdateOrClearLoginAttempt(UserDetailsModel userDetails, bool isClear = false)
        {
            UserDetails efUserDetails = new UserDetails();
            _userManagementContext.ChangeTracker.Clear();

            efUserDetails.UserId = userDetails.UserId;
            efUserDetails.LoginAttempt = isClear ? 0 : userDetails.LoginAttempt + 1;
            efUserDetails.LoginAttemptDateTime = System.DateTime.UtcNow;
            _userManagementContext.Entry(efUserDetails).Property(x => x.LoginAttempt).IsModified = true;
            _userManagementContext.Entry(efUserDetails).Property(x => x.LoginAttemptDateTime).IsModified = true;
            //_userManagementContext.Entry(efUserDetails).State = EntityState.Modified;
            return await _userManagementContext.SaveChangesAsync();
        }
        public int ConvertDateTimeToMinute(DateTime? lastAttemptTime)
        {
            var timeDiff = System.DateTime.UtcNow - lastAttemptTime;//52min
            int timeDiffInmin = timeDiff?.Minutes ?? 0;

            return timeDiffInmin;
        }

        public async Task<UserDetailsModel> LoginUser(string userName)
        {

            var userDetails = await (from UserDetails u in _userManagementContext.UserDetails
                                     join M_UserRole r in _userManagementContext.M_UserRole on u.RoleId equals r.RoleId
                                     where u.UserName == userName
                                     select new UserDetailsModel
                                     {
                                         UserId = u.UserId,
                                         UserName = u.UserName,
                                         Password = u.Password,
                                         IsActive = u.IsActive,
                                         LoginAttempt = u.LoginAttempt,
                                         LoginAttemptDateTime = u.LoginAttemptDateTime,
                                         RoleName = r.RoleName
                                     }).FirstOrDefaultAsync();
            return userDetails;
        }

        public async Task<List<Country>> GetCountryList()
        {
            return await _userManagementContext.Country.ToListAsync();
        }

        public async Task<List<State>> GetStateListByCountryId(Int64 countryId)
        {
            //Int64[] CountryId = countryIds.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
            //return await _userManagementContext.State.Where(x => CountryId.Contains(x.CountryId)).ToListAsync();
            return await _userManagementContext.State.Where(x => x.CountryId == countryId).ToListAsync();
        }

        public async Task<List<City>> GetCityListByStateId(Int64 stateId)
        {
            return await _userManagementContext.City.Where(x => x.StateId == stateId).ToListAsync();
        }

        public async Task<bool> DeleteUserById(long userId)
        {
            UserDetails efUserDetails = new UserDetails();
            _userManagementContext.ChangeTracker.Clear();
            efUserDetails.UserId = userId;
            efUserDetails.IsDeleted = true;

            _userManagementContext.Entry(efUserDetails).Property(x => x.IsDeleted).IsModified = true;

            int response = await _userManagementContext.SaveChangesAsync();
            return response != 0;
        }

        public async Task<List<UserDetailsModel>> GetAllUsers()
        {
            var users = await (from UserDetails u in _userManagementContext.UserDetails
                               join Address a in _userManagementContext.Address on u.UserId equals a.AddressId into addressJoin
                               from addressData in addressJoin.DefaultIfEmpty()
                               join Country c in _userManagementContext.Country on addressData.CountryId equals c.CountryId into countryJoin
                               from countryData in countryJoin.DefaultIfEmpty()
                               join State s in _userManagementContext.State on addressData.StateId equals s.StateId into stateJoin
                               from stateData in stateJoin.DefaultIfEmpty()
                               join City ct in _userManagementContext.City on addressData.CityId equals ct.CityId into cityJoin
                               from cityData in cityJoin.DefaultIfEmpty()
                               join M_UserRole r in _userManagementContext.M_UserRole on u.RoleId equals r.RoleId into roleJoin
                               from roleData in roleJoin.DefaultIfEmpty()
                               where !u.IsDeleted 
                               select new UserDetailsModel
                               {
                                   UserId = u.UserId,
                                   UserName = u.UserName,
                                   Gender = u.Gender.ToString() == "M" ? "Male" : "Female",
                                   BirthDate = u.BirthDate,
                                   Email = u.Email,
                                   MobileNo = u.MobileNo,
                                   RoleName = roleData.RoleName,
                                   Address = new AddressModel
                                   {
                                       CountryName = countryData.CountryName,
                                       StateName = stateData.StateName,
                                       CityName = cityData.CityName,
                                   }
                               }).ToListAsync();

            return users;

        }
        public async Task<List<City>> GetAllCities()
        {
            return await _userManagementContext.City.ToListAsync();
        }
    }
}
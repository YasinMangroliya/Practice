using Model;
using Model.EFDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTest.MockData
{
    public class UserDetailsData
    {
        public static List<UserDetailsModel> GetUserDetailsList()
        {
            return new List<UserDetailsModel>{
                new UserDetailsModel
                {
                    UserName = "test",
                    Password = "test",
                    IsActive = true,
                    LoginAttempt=0,
                    LoginAttemptDateTime= System.DateTime.UtcNow,
                },
                new UserDetailsModel
                {
                    //Inactive User
                    UserName = "test1",
                    Password = "test1",
                    IsActive = false,
                    LoginAttempt=0,
                    LoginAttemptDateTime= System.DateTime.UtcNow,
                },
                new UserDetailsModel
                {
                    //Maximum Attempt Reach
                    UserName = "test2",
                    Password = "test2",
                    IsActive = true,
                    LoginAttempt=4,
                    LoginAttemptDateTime= System.DateTime.UtcNow.AddMinutes(-2),
                },
                 new UserDetailsModel
                {
                     //Maximum Attempt reach but before X time so valid for new attempts
                    UserName = "test3",
                    Password = "test3",
                    IsActive = true,
                    LoginAttempt=4,
                    LoginAttemptDateTime=  System.DateTime.UtcNow.AddMinutes(-15),
                },
            };
        }
    }
}

using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Model;
using Moq;
using NUnitTest.MockData;
using System.ComponentModel.DataAnnotations;
using UserManagement.Controllers;

namespace NUnitTest
{
    public class LoginTest
    {
        [SetUp]
        public void Setup()
        {
        }
        //private readonly ITestOutputHelper _outputHelper;

        private readonly Mock<IUserService> userService;
        public LoginTest(/*ITestOutputHelper outputHelper*/)
        {
            userService = new Mock<IUserService>();
            //_outputHelper = outputHelper;
        }


        [Test]
        public async Task UserNotFound()
        {
            var result = await LoginResult("Unknown", "test") as BadRequestObjectResult;

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(LoginValidationEnum.UserNotFound, result.Value);
        }
        [Test]
        public async Task InvalidPassword()
        {
            var result = await LoginResult("test", "Unknown") as BadRequestObjectResult;

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(LoginValidationEnum.InvalidPassword, result.Value);
        }
        [Test]
        public async Task InactiveUser()
        {
            var result = await LoginResult("test1", "test1") as BadRequestObjectResult;

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(LoginValidationEnum.InactiveUser, result.Value);
        }
        [Test]
        public async Task MaximumAtteptReachError()
        {
            var result = await LoginResult("test2", "test2") as BadRequestObjectResult;

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(LoginValidationEnum.MaximumAtempt, result.Value);
        }

        //Success
        [Test]
        public async Task AllowAtempt()
        {
            var result = await LoginResult("test3", "test3") as OkObjectResult;

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(true, result.Value);
        }

        [Theory]
        [TestCase("", "")] // Empty 
        [TestCase("Test", "With Space")] //Password white space validation
        [TestCase("ABC", "ABC")] //Character less then 4 
        [TestCase("This test is to check character count validation", "Test")] //Character Greater then 20
        public async Task DataAnnotationTest(string userName, string password)
        {
            var loginData = new LoginModel { UserName = userName, Password = password };
            var checkModel = ValidateModel(loginData);

            Console.WriteLine(string.Join("\n", checkModel.ToList()));

            Assert.True(checkModel.Count > 0);
        }

        //To Validate Data Annotations 
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        public async Task<dynamic> LoginResult(string userName, string password)
        {
            var loginData = new LoginModel { UserName = userName, Password = password };

            var userData = UserDetailsData.GetUserDetailsList().Where(x => x.UserName == loginData.UserName).FirstOrDefault();

            userService.Setup(x => x.LoginUser(loginData.UserName))
                .ReturnsAsync(userData);

            if (userData != null)
                userService.Setup(x => x.ConvertDateTimeToMinute(userData.LoginAttemptDateTime))
                    .Returns(ConvertDateTimeToMinuteTest(userData.LoginAttemptDateTime));

            var testController = new TestController(userService.Object);


            return await testController.Login(loginData);
        }
        public static int ConvertDateTimeToMinuteTest(DateTime? lastAttemptTime)
        {
            var timeDiff = System.DateTime.UtcNow - lastAttemptTime;//52min
            int timeDiffInmin = timeDiff?.Minutes ?? 0;

            return timeDiffInmin;
        }

    }
}

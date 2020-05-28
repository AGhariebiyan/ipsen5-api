using GMAPI.Data;
using GMAPI.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IpsenApiTesting
{
    class AuthTest
    {

        private Mock<IAuthRepository> authRepoMock;
        private Account mockAccount;
      

        [SetUp]
        public void Setup()
        {
            authRepoMock = new Mock<IAuthRepository>();
            mockAccount = new Account { 
               Id = Guid.Parse("f2c8ba9a-2680-4584-b975-d6b44e381d08"),
               Email = "oetze@live.nl",
               FirstName = "Oetze",
               MiddleName = "van den",
               LastName = "Broek"
            };
            
        }

        [Test]
        public void TestLoginFunctionality()
        {
            authRepoMock.Setup(a => a.Login("oetze@live.nl", "12345678")).Returns(Task.FromResult(mockAccount));

            IAuthRepository authRepo = authRepoMock.Object;

            Account account = authRepo.Login("oetze@live.nl", "12345678").Result;
            
            Assert.IsTrue(account.Email == "oetze@live.nl");
            
        }


    }
}

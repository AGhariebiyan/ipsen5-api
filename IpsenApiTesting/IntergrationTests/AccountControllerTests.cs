using GMAPI;
using GMAPI.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;


namespace IpsenApiTesting
{
    public class AccountControllerTests: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public AccountControllerTests(CustomWebApplicationFactory<Startup> factory)

        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task CanGetAccounts() {
            var httpResponse = await _client.GetAsync("/api/accounts");

            Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized);

           /* var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var accounts = JsonConvert.DeserializeObject<IEnumerable<Account>>(stringResponse);
            //Assert.Contains(accounts, a => a. == "Oetze");
            Assert.Contains(accounts, a => a.FirstName =="oetze");*/
        }

    }
}

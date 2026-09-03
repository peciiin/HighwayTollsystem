using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayTollsystem.Tests.Integration
{
    public class VehicleControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        public VehicleControllerTests(CustomWebApplicationFactory customWebApplicationFactory) 
        {
            _factory = customWebApplicationFactory;
            _client = _factory.CreateClient();
        }

    }
}

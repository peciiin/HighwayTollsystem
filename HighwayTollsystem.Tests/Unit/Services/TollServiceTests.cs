using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using HighwayTollsystem.Models;
using Moq;
using HighwayTollsystem.Interfaces;


namespace HighwayTollsystem.Tests.Unit.Services
{
    public class TollServiceTests
    {
        private readonly HighwayTollContext _context;
        private readonly TollService _sut;
        private readonly Mock<IVignetteService> _mockVignetteService = new Mock<IVignetteService>();
        private readonly Mock<ISpeedService> _mockSpeedService = new Mock<ISpeedService>();
        private readonly Mock<IVehicleInspectionService> _mockVehicleInspectionService = new Mock<IVehicleInspectionService>();


        public TollServiceTests() 
        {
            var options = new DbContextOptionsBuilder<HighwayTollContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockSpeedService = new Mock<ISpeedService>();
            _mockVehicleInspectionService = new Mock<IVehicleInspectionService>();
            _mockVignetteService = new Mock<IVignetteService>();
            _context = new HighwayTollContext(options);


            _sut = new TollService(_context, _mockVignetteService.Object, _mockSpeedService.Object, _mockVehicleInspectionService.Object);

        }




    }
}

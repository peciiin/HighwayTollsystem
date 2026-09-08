using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using HighwayTollsystem.DTOs;
using HighwayTollsystem.Validators;

namespace HighwayTollsystem.Tests.Unit.Validator
{
    public class RegisterNewVehicleDtoValidator
    {
        private readonly RegisterNewVehicleDtoValidator _validator;

        public RegisterNewVehicleDtoValidator()
        {
            _validator = new RegisterNewVehicleDtoValidator();
        }

    }
}

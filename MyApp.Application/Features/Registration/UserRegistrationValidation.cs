using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Features.Registration
{
    public class UserRegistrationValidation : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ConfirmPassword).NotEmpty();
            RuleFor(x => x.Password).Equal(x => x.ConfirmPassword).WithMessage("Password Mismatch");
        }
    }
}

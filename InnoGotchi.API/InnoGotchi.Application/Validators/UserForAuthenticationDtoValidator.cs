using FluentValidation;
using InnoGotchi.Application.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoGotchi.Application.Validators
{
    public class UserForAuthenticationDtoValidator : AbstractValidator<UserForAuthenticationDto>
    {
        public UserForAuthenticationDtoValidator()
        {
            RuleFor(u => u.Email)
                .EmailAddress();
        }
    }
}

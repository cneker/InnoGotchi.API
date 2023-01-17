﻿using FluentValidation;
using InnoGotchi.Application.DataTransferObjects;

namespace InnoGotchi.Application.Validators
{
    public class PetForCreationDtoValidator : AbstractValidator<PetForCreationDto>
    {
        public PetForCreationDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .Length(1, 20);
            RuleFor(p => p.Eye)
                .IsInEnum();
            RuleFor(p => p.Nose)
                .IsInEnum();
            RuleFor(p => p.Mouth)
                .IsInEnum();
            RuleFor(p => p.Body)
                .IsInEnum();
        }
    }
}

using FluentValidation;
using InnoGotchi.Application.DataTransferObjects.Pet;

namespace InnoGotchi.Application.Validators.Pet
{
    public class PetForUpdateDtoValidator : AbstractValidator<PetForUpdateDto>
    {
        public PetForUpdateDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .Length(1, 20);
        }
    }
}

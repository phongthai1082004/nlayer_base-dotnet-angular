using BusinessLogicLayer.DTOs.Authentication;
using DataAccessLayer.Constants.Messages;
using FluentValidation;

namespace BusinessLogicLayer.Validators.Authentication
{
    public class LoginByEmailRequestValidator : AbstractValidator<LoginByEmailDtoRequest>
    {
        public LoginByEmailRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(AuthenticationMessages.EmailRequired)
                    .EmailAddress().WithMessage(AuthenticationMessages.EmailInvalidFormat)
                    .MaximumLength(100).WithMessage(AuthenticationMessages.EmailMaxLength);

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage(AuthenticationMessages.PasswordRequired)
            .Length(8, 100).WithMessage(AuthenticationMessages.PasswordLength) 
            .Matches(@"[A-Z]").WithMessage(AuthenticationMessages.PasswordComplex) 
            .Matches(@"[a-z]").WithMessage(AuthenticationMessages.PasswordComplex) 
            .Matches(@"[0-9]").WithMessage(AuthenticationMessages.PasswordComplex) 
            .Matches(@"[\W_]").WithMessage(AuthenticationMessages.PasswordComplex); 
        }
    }
}

using DataAccess.DAL;
using Domain.Dto;
using Domain.Dto.V1.Request;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators.ProjectsValidator
{
    public class CreateProjectValidator : AbstractValidator<ProjectRequestDto>
    {
        private readonly TaskContext _context;
        public CreateProjectValidator(TaskContext context)
        {
            _context = context;
          
            Include(new ProjectValidator(context));
            RuleFor(x => x)
                .Must(y => !_context.Projects.IgnoreQueryFilters().Any(z => z.Name == y.Name ))
                .WithMessage("There is already project with the same name.");

            RuleFor(x => x.UserIds).Cascade(CascadeMode.Stop)
               .Must(x => x.Count() == x.Distinct().Count())
               .When(x => x.UserIds != null)
               .WithMessage("There are a duplicates in the set of the provided ids.");

            RuleForEach(x => x.UserIds).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("All values in the array needs to have value.")
                .Must(x => _context.Users.Any(y => y.Id == x))
                 .When(x => x.UserIds != null)
                .WithMessage("Value {PropertyValue} doesn't corresponding to any task in the system");
        }

    }
}

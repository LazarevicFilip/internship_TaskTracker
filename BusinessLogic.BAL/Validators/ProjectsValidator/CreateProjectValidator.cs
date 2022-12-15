using DataAccess.DAL;
using Domain.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators.ProjectsValidator
{
    public class CreateProjectValidator : AbstractValidator<ProjectDto>
    {
        private readonly TaskContext _context;
        public CreateProjectValidator(TaskContext context)
        {
            _context = context;
            Include(new ProjectValidator(context));
            RuleFor(x => x.Name)
                .Must(y => !_context.Projects.Any(z => z.Name == y))
                .WithMessage("There is already project with the same name.");
        }
    }
}

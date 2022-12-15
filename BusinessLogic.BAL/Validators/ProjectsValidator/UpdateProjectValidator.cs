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
    public class UpdateProjectValidator : AbstractValidator<ProjectDto>
    {
        private readonly TaskContext _context;
        public UpdateProjectValidator(TaskContext context)
        {
            _context = context;
            Include(new ProjectValidator(context));
            RuleFor(x => x)
                .Must(y => !_context.Projects.Any(z => z.Name == y.Name && z.Id != y.Id))
                .WithMessage("There is already project with the same name.");

        }
    }
}

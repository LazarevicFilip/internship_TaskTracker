using DataAccess.DAL;
using Domain.Dto;
using Domain.Dto.V1.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators.ProjectsValidator
{
    public class ProjectValidator : AbstractValidator<ProjectRequestDto>
    {
        private readonly TaskContext _context;
        public ProjectValidator(TaskContext context)
        {
            _context = context;
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required parameter")
                .Length(2, 30).WithMessage("Name of the project must be between 2 and 20 characters.");
                

            RuleFor(x => x.ProjectStatus)
                 .IsInEnum().WithMessage("Available statuses of the projects are 0,1 and 2.");

            RuleFor(x => x.ProjectPriority)
                .IsInEnum().WithMessage("Priority of the project must be number between 0 and 3.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required parameter.");

            RuleFor(x => x)
                .Must(x => DateTime.Compare(x.StartDate, (DateTime)x.CompletionDate) < 0).WithMessage("StartDate must be earlier than CompletionDate.")
                .When(p => p.CompletionDate != null);
        }
    }
}

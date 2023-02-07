using DataAccess.DAL;
using DataAccess.DAL.Core;
using Domain.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators.TaskValidators
{
    public class TaskValidator : AbstractValidator<TaskDto>
    {
        private readonly TaskContext _context;

        public TaskValidator(TaskContext context)
        {
            _context = context;
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required parameter.")
                .Length(2, 20).WithMessage("Name of the task must be between 2 and 20 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Available statuses of the tasks are 0,1 and 2.");

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage("Description of the task can't be longer that 100 characters.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority of the task must be number between 0 and 3.");

            RuleFor(x => x.ProjectId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ProjectId is required parameter.")
                .Must(x => _context.Projects.Any(p => p.Id == x)).WithMessage("Provided value for ProjectId doesn't match any project in the sytem.");
        }
    }
}

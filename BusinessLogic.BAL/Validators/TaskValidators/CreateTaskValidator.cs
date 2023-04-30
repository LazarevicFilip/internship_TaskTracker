using DataAccess.DAL;
using Domain.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators.TaskValidators
{
    public  class CreateTaskValidator : AbstractValidator<TaskDto>
    {
        private readonly TaskContext _context;

        public CreateTaskValidator(TaskContext context)
        {
            _context = context;

            Include(new TaskValidator(context));
            RuleFor(x => x)
                .Must(y => !_context.Tasks.Any(z => z.Name == y.Name && z.ProjectId == y.ProjectId))
                .WithMessage("There is already task with same the name for the provided project.");
            RuleFor(x => x.UserIds).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User Ids are required parameter.")
                .Must(x => x.Count() == x.Distinct().Count())
                .WithMessage("There are a duplicates in the set of the provided ids.");
            RuleForEach(x => x.UserIds).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("All values in the array needs to have value.")
                .Must(x => _context.Users.Any(y => y.Id == x))
                .WithMessage("Value {PropertyValue} doesn't corresponding to any task in the system");
        }
    }
}

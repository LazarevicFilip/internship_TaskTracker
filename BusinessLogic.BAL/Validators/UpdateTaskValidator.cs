using DataAccess.DAL;
using Domain.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators
{
    public class UpdateTaskValidator : AbstractValidator<TaskDto>
    {
        private readonly TaskContext _context;
        public UpdateTaskValidator(TaskContext context)
        {
            _context = context;
            Include(new TaskValidator(context));
            RuleFor(x => x)
                .Must(y => !_context.Tasks.Any(z => z.Name == y.Name && z.ProjectId == y.ProjectId && z.Id != y.Id))
                .WithMessage("There is already task with same the name for the provided project.");
        }
    }
}

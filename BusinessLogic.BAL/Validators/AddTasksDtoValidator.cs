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
    public class AddTasksDtoValidator : AbstractValidator<AddTasksDto>
    {
        private readonly TaskContext _context;
        public  AddTasksDtoValidator(TaskContext context)
        {
            _context = context;
            RuleFor(x => x.Tasks).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Tasks are required parameter.")
                .Must(x => x.Count() == x.Distinct().Count())
                .WithMessage("There are a duplicates in the set of the provided ids.");
            RuleForEach(x => x.Tasks).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("All values in the array needs to have value.")
                .Must(x => _context.Tasks.Any(y => y.Id == x))
                .WithMessage("Value {PropertyValue} doesn't corresponding to any task in the system");

        }
    }
}

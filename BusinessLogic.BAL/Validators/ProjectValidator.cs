﻿using DataAccess.DAL;
using Domain.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators
{
    public class ProjectValidator : AbstractValidator<ProjectDto>
    {
        private readonly TaskContext _context;
        public ProjectValidator(TaskContext context)
        {
            _context = context;
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required parameter")
                .Length(2, 20).WithMessage("Name of the project must be between 2 and 20 characters.")
                .Must(y => !_context.Projects.Any(z => z.Name == y)).WithMessage("There is already project with the same name.");

            RuleFor(x => x.ProjectStatus)
                 .IsInEnum().WithMessage("Available statuses of the projects are 0,1 and 2.");

            RuleFor(x => x.ProjectPriotiry)
                .InclusiveBetween(0, 3).WithMessage("Priority of the project must be number between 0 and 3.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required parameter.");

            RuleFor(x => x)
                .Must(x => DateTime.Compare(x.StartDate,(DateTime)x.CompletionDate) < 0).WithMessage("CompletionDate can be larger than StartDate.")
                .When(p => p.CompletionDate != null);
        }
    }
}

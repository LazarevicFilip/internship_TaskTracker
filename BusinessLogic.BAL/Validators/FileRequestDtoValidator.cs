using DataAccess.DAL;
using Domain.Dto;
using Domain.Dto.V1.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Validators
{
    public class FileRequestDtoValidator : AbstractValidator<FileRequestDto>
    {
        private readonly TaskContext _context;
        public FileRequestDtoValidator(TaskContext context)
        {
            _context = context;
            var allowedContentTypes = new List<string> { "image/jpeg", "image/png" };
            RuleFor(x => x.File).NotEmpty().WithMessage("File is required parameter");
            RuleFor(x => x.File.Length).Must(x => x < 3000000).WithMessage("File can not be larger that 3mb");
            RuleFor(x => x.File.ContentType).Must(x => allowedContentTypes.Contains(x)).WithMessage("File type must be img or png ");
            RuleFor(x => x.TaskId).Must(x => _context.Tasks.Any(y => y.Id == int.Parse(x))).WithMessage("Provided Id doesn't exists in the system");

        }
    }
}

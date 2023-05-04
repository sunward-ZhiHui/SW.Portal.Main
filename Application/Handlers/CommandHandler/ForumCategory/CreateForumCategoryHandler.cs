using Application.Command.ForumCategory;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.ForumCategory
{
    public class CreateForumCategoryHandler : IRequestHandler<CreateForumCategoryCommand, ForumCategoryResponse>
    {
        private readonly IFourmCategoryCommandRepository _categoryCommandRepository;
        public CreateForumCategoryHandler(IFourmCategoryCommandRepository categoryCommandRepository)
        {
            _categoryCommandRepository = categoryCommandRepository;
        }
        public async Task<ForumCategoryResponse> Handle(CreateForumCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryEntity = RoleMapper.Mapper.Map<ForumCategorys>(request);

            if (categoryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCategory = await _categoryCommandRepository.AddAsync(categoryEntity);
            var categoryResponse = RoleMapper.Mapper.Map<ForumCategoryResponse>(newCategory);
            return categoryResponse;
        }
    }
}
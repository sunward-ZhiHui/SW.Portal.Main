using Application.Command.ForumCategory;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.ForumCategory
{
    public class EditFourmCategoryHandler : IRequestHandler<EditFourmCategoryCommand, ForumCategoryResponse>
    {
        private readonly IFourmCategoryCommandRepository _categoryCommandRepository;
        private readonly IFourmCategoryQueryRepository _forumCategoryQueryRepository;

        public EditFourmCategoryHandler(IFourmCategoryCommandRepository categoryCommandRepository, IFourmCategoryQueryRepository forumCategoryQueryRepository)
        {
            _categoryCommandRepository = categoryCommandRepository;
            _forumCategoryQueryRepository = forumCategoryQueryRepository;
        }
        public async Task<ForumCategoryResponse> Handle(EditFourmCategoryCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<ForumCategorys>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _categoryCommandRepository.UpdateAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedCustomer = await _forumCategoryQueryRepository.GetByIdAsync(request.ID);
            var customerResponse = RoleMapper.Mapper.Map<ForumCategoryResponse>(modifiedCustomer);

            return customerResponse;
        }
    }
    
}

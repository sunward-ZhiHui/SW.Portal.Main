using Application.Command.ForumCategory;
using Application.Commands;
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
    public class DeleteFourmCategoryHandler : IRequestHandler<DeleteFourmCategoryCommand, String>
    {
        private readonly IFourmCategoryCommandRepository _categoryCommandRepository;
        private readonly IForumCategoryQueryRepository _categoryQueryRepository;
        public DeleteFourmCategoryHandler(IFourmCategoryCommandRepository categoryRepository, IForumCategoryQueryRepository categoryQueryRepository)
        {
            _categoryCommandRepository = categoryRepository;
            _categoryQueryRepository = categoryQueryRepository;
        }

        public async Task<string> Handle(DeleteFourmCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerEntity = await _categoryQueryRepository.GetByIdAsync(request.Id);

                await _categoryCommandRepository.DeleteAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Customer information has been deleted!";
        }
    }
}




using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllfileprofiletypeHandler : IRequestHandler<GetAllfileprofiletypeQuery, List<Fileprofiletype>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetAllfileprofiletypeHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<Fileprofiletype>> Handle(GetAllfileprofiletypeQuery request, CancellationToken cancellationToken)
        {
            return (List<Fileprofiletype>)await _fileprofileQueryRepository.GetAllAsync(request.FileProfileTypeID);
        }

    }
    public class GetAllselectedfileprofiletypeHandler : IRequestHandler<GetAllSelectedfileprofiletypeQuery, List<view_GetFileProfileTypeDocument>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetAllselectedfileprofiletypeHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<view_GetFileProfileTypeDocument>> Handle(GetAllSelectedfileprofiletypeQuery request, CancellationToken cancellationToken)
        {
            return (List<view_GetFileProfileTypeDocument>)await _fileprofileQueryRepository.GetAllSelectedFileAsync(request.selectedFileProfileTypeID);
        }

    }

}

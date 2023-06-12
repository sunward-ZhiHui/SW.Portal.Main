using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.AssetCatalogMasters
{
    public class DeleteAssetCatalogMasterCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteAssetCatalogMasterCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.AssetPartsMaintenaceMasters
{
    public class DeleteAssetPartsMaintenaceMasterCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteAssetPartsMaintenaceMasterCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteAssetEquipmentMaintenaceMasterCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteAssetEquipmentMaintenaceMasterCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}

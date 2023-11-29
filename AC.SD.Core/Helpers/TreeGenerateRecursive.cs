using Core.EntityModels;
using DevExpress.XtraPrinting.Native;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AC.SD.Core.Helpers
{
    public class TreeGenerateRecursive
    {

        public List<DocumentsModel> TreeGenerateRecursives(List<DocumentsModel> fileProfileTypeDocumentsAll)
        {
            var lookup = fileProfileTypeDocumentsAll.ToLookup(x => x.ParentId);
            Func<long?, List<DocumentsModel>> build = null;
            build = pid =>
                lookup[pid]
                    .Select(x => new DocumentsModel()
                    {
                        FileProfileTypeId = x.FileProfileTypeId,
                        ProfileID = x.ProfileID,
                        ParentId = x.ParentId,
                        FileName = x.FileName,
                        HasChildren = true,
                        Children = build(x.FileProfileTypeId).Count > 0 ? build(x.FileProfileTypeId) : new List<DocumentsModel>(),
                    })
                    .ToList();
            return build(null).ToList();
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.AttributeDynamicData
{
    public class UserData
    {
        public long? SingleSelectId { get; set; }
        [FieldType(FieldType.ComboBox)]
        public object SingleObject { get; set; }
         public List<long?> MultipleSelectIds { get; set; }
        [FieldType(FieldType.List)]
        public object MultipleObjects { get; set; }
    }
}


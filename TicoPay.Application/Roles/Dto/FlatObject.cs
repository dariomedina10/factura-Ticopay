using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Roles.Dto
{
    public class FlatObject
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Data { get; set; }

        public FlatObject(string name, string id, string parentId)
        {
            Data = name;
            Id = id;
            ParentId = parentId;
        }

    }
    public class RecursiveObject
    {
        public string Data { get; set; }
        public string Id { get; set; }
        public FlatTreeAttribute Attr { get; set; }
        public List<RecursiveObject> Children { get; set; }
    }

    public class FlatTreeAttribute
    {
        public string id;
        public bool selected;
    }
}

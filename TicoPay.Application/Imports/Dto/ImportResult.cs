using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Imports.Dto
{
    public class ImportResult
    {
        public int ImportedItemsCount { get; set; }
        public IEnumerable ItemsWithErrors { get; set; }
        public bool HasErrors { get; set; }
    }
}

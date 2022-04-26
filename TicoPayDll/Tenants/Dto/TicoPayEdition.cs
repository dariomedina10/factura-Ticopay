using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Tenants.Enums;

namespace TicoPayDll.Tenants.Dto
{
    public class TicoPayEdition
    {
        public int Price { get; set; }
        public string DisplayName { get; set; }
        public TicopayEditionType? EditionType { get; set; }
        public bool CloseForSale { get; set; }
    }
}

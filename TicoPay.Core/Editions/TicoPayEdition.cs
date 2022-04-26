using Abp.Application.Editions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Editions
{
    public class TicoPayEdition : Edition
    {
        public virtual int Price { get; set; }
        [StringLength(160)]
        public override string DisplayName { get; set; }
        public virtual TicopayEditionType? EditionType { get; set; }
        public bool CloseForSale { get; set; }
    }

    public enum TicopayEditionType
    {
        Monthly = 0,
        Annual
    }
}

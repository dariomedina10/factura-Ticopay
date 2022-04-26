using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Items
{
    public class AddItem
    {
        public string irecurso { get; set; }
        public ItemBasicInformation infobasica { get; set; }

        public AddItem()
        {
            infobasica = new ItemBasicInformation();
            irecurso = "";
        }
    }

    public class ItemBasicInformation
    {
        public string irecurso { get; set; }
        public string nrecurso { get; set; }
        public string nunidad { get; set; }
        public string igrupoinv { get; set; }
        public string bcontrolinv { get; set; }
        public string bventa { get; set; }
        public string bvisible { get; set; } // F o T

        public ItemBasicInformation()
        {
            irecurso = "";
            nrecurso = "";
            nunidad = "";
            igrupoinv = "";
            bcontrolinv = "";
            bventa = "";
            bvisible = "";
        }
    }
}

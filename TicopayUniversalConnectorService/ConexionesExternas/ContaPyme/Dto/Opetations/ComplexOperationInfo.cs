using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations
{
    public class ComplexOperationInfo
    {
        public string accion { get; set; } // CREATE , NEW , 
        public OperationType[] operaciones { get; set; }
        public object oprdata { get; set; }

        public ComplexOperationInfo(string _accion, string _operationType)
        {
            accion = _accion;
            operaciones = new OperationType[1] { new OperationType(_operationType)};
        }
    }

    public class OperationType
    {
        public string itdoper { get; set; }
        public string inumoper { get; set; }

        public OperationType(string _idOper)
        {
            itdoper = _idOper;
            inumoper = "0";
        }
    }

}

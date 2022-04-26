using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Printers
{
    public interface IModelPrinter
    {
        string print(DocumentPrint docuemnt);
        string BuildHeader(DocumentPrint doc);
        string BuildClientInfo(DocumentPrint doc);
        string BuildDetails(DocumentPrint doc);
        string BuildFooter(DocumentPrint doc);
        string HeaderDetails(string str);
    }
}

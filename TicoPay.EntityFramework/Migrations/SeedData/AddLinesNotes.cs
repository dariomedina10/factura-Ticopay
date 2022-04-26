using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Editions;
using TicoPay.EntityFramework;
using TicoPay.Invoices;

namespace TicoPay.Migrations.SeedData
{    

    public class AddLinesNotes
    {
        private readonly TicoPayDbContext _context;

        public AddLinesNotes(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var notes = _context.Notes.Where(r => r.NotesLines.Count()  == 0).ToList();
            foreach (var note in notes)
            {
                var title = "Nota " + note.NoteType;
                note.AssignNoteLine(note.TenantId, note.Amount, note.TaxAmount, 0, string.Empty, null, title, 1, LineType.Service, null, null, note, 1,
                    note.Invoice.InvoiceLines.First().Tax, note.Invoice.InvoiceLines.First().TaxId, note.Invoice.InvoiceLines.First().UnitMeasurement, note.Invoice.InvoiceLines.First().UnitMeasurementOthers);

                note.SetInvoiceTotalCalculate(note.TaxAmount, 0, note.NotesLines.Where(d => d.TaxAmount > 0).Sum(d => (d.PricePerUnit * d.Quantity)), note.NotesLines.Where(d => d.TaxAmount == 0).Sum(d => (d.PricePerUnit * d.Quantity)));
                
            }
            _context.SaveChanges();
        }
    }
}

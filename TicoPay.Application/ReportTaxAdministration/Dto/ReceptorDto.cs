using System;

namespace TicoPay.ReportTaxAdministration.Dto
{
    public class ReceptorDto
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastName { get; internal set; }
    }
}
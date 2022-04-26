using Abp.Dependency;
using Abp.Domain.Services;
using System;

namespace TicoPay.General
{
    public class CodeGenerator : ICodeGenerator, IDomainService
    {
        public string GetCode()
        {
            return "ABC";
        }
    }
}

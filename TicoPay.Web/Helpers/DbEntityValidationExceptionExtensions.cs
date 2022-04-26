using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Data.Entity.Validation
{
    public static class DbEntityValidationExceptionExtensions
    {
        public static string GetModelErrorMessage(this DbEntityValidationException ex)
        {
            var results = new List<string>();
            foreach (var model in ex.EntityValidationErrors)
            {
                foreach (var error in model.ValidationErrors)
                {
                    results.Add(" * " + error.ErrorMessage);
                }
            }
            return string.Join(Environment.NewLine, results.ToArray());
        }

        public static ModelStateDictionary GetModelErrors(this DbEntityValidationException ex)
        {
            var results = new ModelStateDictionary();
            foreach (var model in ex.EntityValidationErrors)
            {
                foreach (var error in model.ValidationErrors)
                {
                    results.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            return results;
        }

        public static string GetModelErrorMessage(this AbpValidationException ex, bool AddMember = true)
        {
            var results = new List<string>();
            foreach (var model in ex.ValidationErrors)
            {
                foreach (var member in model.MemberNames)
                {
                    results.Add((AddMember) ? (" * " + member + ": " + model.ErrorMessage) : model.ErrorMessage);
                }
            }
            return string.Join(Environment.NewLine, results.ToArray());
        }

        public static ModelStateDictionary GetModelErrors(this AbpValidationException ex)
        {
            var results = new ModelStateDictionary();
            foreach (var model in ex.ValidationErrors)
            {
                foreach (var member in model.MemberNames)
                {
                    results.AddModelError(member, model.ErrorMessage);
                }
            }
            return results;
        }
    }
}
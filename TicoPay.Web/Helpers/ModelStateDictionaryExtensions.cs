using Abp.Dependency;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddRange(this ModelStateDictionary modelState, ModelStateDictionary range)
        {
            if (range != null)
            {
                foreach (var item in range)
                {
                    modelState.Add(item.Key, item.Value);
                }
            }
        }

        public static string ToErrorMessage(this ModelStateDictionary modelState)
        {
            List<string> errors = new List<string>();
            if (modelState.Count > 0)
            {
                foreach (var error in modelState)
                {
                    foreach (var item in error.Value.Errors)
                    {
                        errors.Add(item.ErrorMessage);
                    }
                }
            }
            return string.Join(Environment.NewLine, errors.ToArray());
        }

        public static void AddValidationErrors(this ModelStateDictionary modelState, ICustomValidate viewModel, IIocResolver iocResolver)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            viewModel.AddValidationErrors(new CustomValidationContext(result, iocResolver));
            int i = 0;
            foreach (var validationResult in result)
            {
                foreach (var item in result[i].MemberNames)
                {
                    modelState.AddModelError(item.ToString(), result[i].ErrorMessage);
                }
                i++;
            }
        }
    }
}
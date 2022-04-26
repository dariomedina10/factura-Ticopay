namespace TicoPay.Web.DataAnnotation
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class EmailAttribute : RegularExpressionAttribute
    {
        private string displayName;
        public EmailAttribute()
            : base(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$")
        {
            this.ErrorMessage = "La dirección de correo en '{0}' no es válida.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var attributes = validationContext.ObjectType.GetProperty(validationContext.MemberName).GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (attributes != null)
                this.displayName = (attributes[0] as DisplayNameAttribute).DisplayName;
            else
                this.displayName = validationContext.DisplayName;

            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, displayName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using TicoPay.ReportsSettings.Dto;

namespace System.Web.Mvc
{
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return helper.FileFor(expression, null);
        }

        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var builder = new TagBuilder("input");
            var id = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            builder.GenerateId(id);
            builder.MergeAttribute("name", id);
            builder.MergeAttribute("type", "file");
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }


        public static MvcHtmlString FontsDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<FontListSelectItem> selectList, string optionLabel = null, object htmlAttributes = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string name = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            return FontsDropDownList(htmlHelper, metadata, name, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString FontsDropDownList(this HtmlHelper htmlHelper, ModelMetadata metadata, string name, IEnumerable<FontListSelectItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }

            TagBuilder dropdown = new TagBuilder("select");
            dropdown.Attributes.Add("name", fullName);
            dropdown.MergeAttributes(htmlAttributes);
            dropdown.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            StringBuilder options = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(optionLabel))
            {
                options.Append("<option value='" + string.Empty + "'>" + optionLabel + "</option>");
            }

            foreach (var item in selectList)
            {
                item.Selected = (metadata.Model != null && item.Value == metadata.Model.ToString());
                options.Append($"<option value='{item.Value}' {(item.Selected ? "selected" : "")} style='font-family:\"{item.FontFamily}\"''>{item.Text}</option>");
            }
            dropdown.InnerHtml = options.ToString();
            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }
    }
}

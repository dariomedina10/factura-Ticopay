using Abp.Web.Mvc.Views;

namespace TicoPay.Web.Views
{
    public abstract class TicoPayWebViewPageBase : TicoPayWebViewPageBase<dynamic>
    {

    }

    public abstract class TicoPayWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected TicoPayWebViewPageBase()
        {
            LocalizationSourceName = TicoPayConsts.LocalizationSourceName;
        }
    }
}
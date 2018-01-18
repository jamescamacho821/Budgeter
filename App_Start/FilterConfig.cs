using System.Web;
using System.Web.Mvc;

namespace jcamacho_budgeter1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

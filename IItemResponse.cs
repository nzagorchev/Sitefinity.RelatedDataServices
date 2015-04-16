using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SitefinityWebApp.ServiceStackCustom
{
    public interface IItemResponse
    {
        Dictionary<string, object> CustomProps { get; set; }
        Dictionary<string, object> CustomTaxonomyProps { get; set; }
    }
}

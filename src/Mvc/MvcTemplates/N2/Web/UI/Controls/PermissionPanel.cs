using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Security;
using N2.Web.UI;

namespace N2.Edit.Web.UI.Controls
{
    public class PermissionPanel : PlaceHolder
    {
        private CustomValidator cv = new CustomValidator
        {
            CssClass = "info",
            Text = "You do not have sufficient permissions.",
            Display = ValidatorDisplay.Dynamic
        };

        public Permission RequiredPermission { get; set; }

        public string Text
        {
            get { return cv.Text; }
            set { cv.Text = value; }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            cv.Page = Page;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            ////TraceSources.AzureTraceSource.TraceInformation("Render");

            if (RequiredPermission == Permission.None)
                RequiredPermission = Page.GetType().GetCustomAttributes(typeof(IPermittable), true).OfType<IPermittable>()
                    .Select(p => PermissionMap.GetMaximumPermission(p.RequiredPermission))
                    .OrderByDescending(rp => rp)
                    .FirstOrDefault();

            var item = new SelectionUtility(this, Page.GetEngine()).SelectedItem;
            if (!Page.GetEngine().SecurityManager.IsAuthorized(Page.User, item, RequiredPermission))
            {
                cv.IsValid = false;
                cv.RenderControl(writer);
            }
            else
                base.Render(writer);
        }
    }
}

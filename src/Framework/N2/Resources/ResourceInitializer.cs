using N2.Engine;
using N2.Plugin;

namespace N2.Resources
{
	[Service]
	public class ResourceInitializer : IAutoStart
	{
		readonly Configuration.ConfigurationManagerWrapper configFactory;

		public ResourceInitializer(Configuration.ConfigurationManagerWrapper configFactory)
		{
			this.configFactory = configFactory;
		}

		#region IAutoStart Members

		/// <summary>
		/// Initializes the paths to the resource files from the N2 configuration. This happens AFTER the
		/// N2.Resources.Register() constructor which initializes the defaults.
		/// </summary>
		/// <see cref="N2.Resources.Register"/>
		public void Start()
		{
			if (configFactory.Sections.Web.Resources.Debug.HasValue)
				Register.Debug = configFactory.Sections.Web.Resources.Debug.Value;

			Register.PreviewJQueryJsPath = configFactory.Sections.Web.PreviewResources.JQueryJsPath;
			Register.PreviewAngularJsRoot = configFactory.Sections.Web.PreviewResources.AngularJsRoot;

			Register.JQueryJsPath = configFactory.Sections.Web.Resources.JQueryJsPath;
            Register.JQueryJsSriHash = configFactory.Sections.Web.Resources.JQueryJsSriHash;
            Register.JQueryUiPath = configFactory.Sections.Web.Resources.JQueryUiPath;
            Register.JQueryUiSriHash = configFactory.Sections.Web.Resources.JQueryUiSriHash;
            Register.JQueryPluginsPath = configFactory.Sections.Web.Resources.JQueryPluginsPath;

			Register.AngularJsRoot = configFactory.Sections.Web.Resources.AngularJsRoot;
            Register.AngularJsSriHash = configFactory.Sections.Web.Resources.AngularJsSriHash;
            Register.AngularJsResourceSriHash = configFactory.Sections.Web.Resources.AngularJsResourceSriHash;
            Register.AngularJsRouteSriHash = configFactory.Sections.Web.Resources.AngularJsRouteSriHash;
            Register.AngularJsSanitizeSriHash = configFactory.Sections.Web.Resources.AngularJsSanitizeSriHash;
            Register.AngularStrapJsPath = configFactory.Sections.Web.Resources.AngularStrapJsPath;
            Register.AngularStrapJsSriHash = configFactory.Sections.Web.Resources.AngularStrapJsSriHash;
            Register.AngularUiJsPath = configFactory.Sections.Web.Resources.AngularUiJsPath;
            Register.AngularUiJsSriHash = configFactory.Sections.Web.Resources.AngularUiJsSriHash;

            Register.CkEditorJsPath = configFactory.Sections.Web.Resources.CkEditorPath;
            Register.CkEditorJsSriHash = configFactory.Sections.Web.Resources.CkEditorSriHash;

            Register.FancyboxJsPath = configFactory.Sections.Web.Resources.FancyboxJsPath;
            Register.FancyboxJsSriHash = configFactory.Sections.Web.Resources.FancyboxJsSriHash;
            Register.FancyboxCssPath = configFactory.Sections.Web.Resources.FancyboxCssPath;

			Register.PartsJsPath = configFactory.Sections.Web.Resources.PartsJsPath;
			Register.PartsCssPath = configFactory.Sections.Web.Resources.PartsCssPath;

			Register.BootstrapCssPath = configFactory.Sections.Web.Resources.BootstrapCssPath;
			Register.BootstrapJsPath = configFactory.Sections.Web.Resources.BootstrapJsPath;
            Register.BootstrapJsSriHash = configFactory.Sections.Web.Resources.BootstrapJsSriHash;
            Register.BootstrapDatePickerCssPath = configFactory.Sections.Web.Resources.BootstrapDatePickerCssPath;
			Register.BootstrapDatePickerJsPath = configFactory.Sections.Web.Resources.BootstrapDatePickerJsPath;
			Register.BootstrapTimePickerCssPath = configFactory.Sections.Web.Resources.BootstrapTimePickerCssPath;
			Register.BootstrapTimePickerJsPath = configFactory.Sections.Web.Resources.BootstrapTimePickerJsPath;

			Register.IconsCssPath = configFactory.Sections.Web.Resources.IconsCssPath;
		}

		public void Stop()
		{
		}

		#endregion
	}
}

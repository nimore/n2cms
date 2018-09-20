using System;
using System.ComponentModel;
using System.Configuration;
using N2.Resources;

namespace N2.Configuration
{
	public class ResourcesElement : ConfigurationElement
	{
		/// <summary>Whether to make registered web resources debuggable.</summary>
		[ConfigurationProperty("debug")]
		public bool? Debug
		{
			get { return (bool?)base["debug"]; }
			set { base["debug"] = value; }
		}

		/// <summary>The path to the included jQuery javascript resource.</summary>
		[ConfigurationProperty("jQueryJsPath", DefaultValue = Register.DefaultJQueryJsPath)]
		public string JQueryJsPath
		{
			get { return (string)base["jQueryJsPath"]; }
			set { base["jQueryJsPath"] = value; }
		}

        [ConfigurationProperty("jQueryJsSriHash", DefaultValue = Register.DefaultJQueryJsSriHash)]
        public string JQueryJsSriHash
        {
            get { return (string)base["jQueryJsSriHash"]; }
            set { base["jQueryJsSriHash"] = value; }
        }

        /// <summary>The path to the included jQuery UI javascript resource.</summary>
        [ConfigurationProperty("jQueryUiPath", DefaultValue = Register.DefaultJQueryUiJsPath)]
		public string JQueryUiPath
		{
			get { return (string)base["jQueryUiPath"]; }
			set { base["jQueryUiPath"] = value; }
		}

        [ConfigurationProperty("jQueryUiSriHash", DefaultValue = Register.DefaultJQueryUiJsSriHash)]
        public string JQueryUiSriHash
        {
            get { return (string)base["jQueryUiSriHash"]; }
            set { base["jQueryUiSriHash"] = value; }
        }

        /// <summary>The path to the included jQuery plgins javascript resource.</summary>
        [ConfigurationProperty("jQueryPluginsPath", DefaultValue = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + Register.JQueryVersion)]
		public string JQueryPluginsPath
		{
			get { return (string)base["jQueryPluginsPath"]; }
			set { base["jQueryPluginsPath"] = value; }
		}

		/// <summary>The path to the included angular javascript resource.</summary>
		[ConfigurationProperty("angularJsRoot", DefaultValue = Register.DefaultAngularJsRoot)]
		public string AngularJsRoot
		{
			get { return (string)base["angularJsRoot"]; }
			set { base["angularJsRoot"] = value; }
		}

        [ConfigurationProperty("angularJsSriHash", DefaultValue = Register.DefaultAngularJsSriHash)]
        public string AngularJsSriHash
        {
            get { return (string)base["angularJsSriHash"]; }
            set { base["angularJsSriHash"] = value; }
        }

        [ConfigurationProperty("angularJsResourceSriHash", DefaultValue = Register.DefaultAngularJsResourceSriHash)]
        public string AngularJsResourceSriHash
        {
            get { return (string)base["angularJsResourceSriHash"]; }
            set { base["angularJsResourceSriHash"] = value; }
        }

        [ConfigurationProperty("angularJsRouteSriHash", DefaultValue = Register.DefaultAngularJsRouteSriHash)]
        public string AngularJsRouteSriHash
        {
            get { return (string)base["angularJsRouteSriHash"]; }
            set { base["angularJsRouteSriHash"] = value; }
        }

        [ConfigurationProperty("angularJsSanitizeSriHash", DefaultValue = Register.DefaultAngularJsSanitizeSriHash)]
        public string AngularJsSanitizeSriHash
        {
            get { return (string)base["angularJsSanitizeSriHash"]; }
            set { base["angularJsSanitizeSriHash"] = value; }
        }

        /// <summary>The path to the included angular-strap javascript resource.</summary>
        [ConfigurationProperty("angularStrapJsPath", DefaultValue = Register.DefaultAngularStrapJsRoot)]
		public string AngularStrapJsPath
		{
			get { return (string)base["angularStrapJsPath"]; }
			set { base["angularStrapJsPath"] = value; }
		}

        [ConfigurationProperty("angularStrapJsSriHash", DefaultValue = Register.DefaultAngularStrapJsSriHash)]
        public string AngularStrapJsSriHash
        {
            get { return (string)base["angularStrapJsSriHash"]; }
            set { base["angularStrapJsSriHash"] = value; }
        }

        /// <summary>The path to the included angular-ui javascript resource.</summary>
        [ConfigurationProperty("angularUiJsPath", DefaultValue = Register.DefaultAngularUiJsPath)]
		public string AngularUiJsPath
		{
			get { return (string)base["angularUiJsPath"]; }
			set { base["angularUiJsPath"] = value; }
		}

        [ConfigurationProperty("angularUiJsSriHash", DefaultValue = Register.DefaultAngularUiJsSriHash)]
        public string AngularUiJsSriHash
        {
            get { return (string)base["angularUiJsSriHash"]; }
            set { base["angularUiJsSriHash"] = value; }
        }

        /// <summary>The path to the included tiny MCE javascript resource.</summary>
        [ConfigurationProperty("ckEditorJsPath", DefaultValue = Register.DefaultCkEditorPath)]
		public string CkEditorPath
		{
			get { return (string)base["ckEditorJsPath"]; }
			set { base["ckEditorJsPath"] = value; }
		}

        [ConfigurationProperty("ckEditorJsSriHash", DefaultValue = Register.DefaultCkEditorSriHash)]
        public string CkEditorSriHash
        {
            get { return (string)base["ckEditorJsSriHash"]; }
            set { base["ckEditorJsSriHash"] = value; }
        }

        [ConfigurationProperty("fancyboxJsPath", DefaultValue = N2.Resources.Register.DefaultFancyboxJsPath)]
		public string FancyboxJsPath
		{
			get { return (string)base["fancyboxJsPath"]; }
			set { base["fancyboxJsPath"] = value; }
		}

        [ConfigurationProperty("fancyboxJsSriHash", DefaultValue = N2.Resources.Register.DefaultFancyboxJsSriHash)]
        public string FancyboxJsSriHash
        {
            get { return (string)base["fancyboxJsSriHash"]; }
            set { base["fancyboxJsSriHash"] = value; }
        }

        [ConfigurationProperty("fancyboxCssPath", DefaultValue = N2.Resources.Register.DefaultFancyboxCssPath)]
		public string FancyboxCssPath
		{
			get { return (string)base["fancyboxCssPath"]; }
			set { base["fancyboxCssPath"] = value; }
		}

		[ConfigurationProperty("partsJsPath", DefaultValue = "{ManagementUrl}/Resources/Js/parts.js?v=" + Register.JQueryVersion)]
		public string PartsJsPath
		{
			get { return (string)base["partsJsPath"]; }
			set { base["partsJsPath"] = value; }
		}

		[ConfigurationProperty("partsCssPath", DefaultValue = "{ManagementUrl}/Resources/Css/parts.css?v=" + Register.JQueryVersion)]
		public string PartsCssPath
		{
			get { return (string)base["partsCssPath"]; }
			set { base["partsCssPath"] = value; }
		}

		[ConfigurationProperty("bootstrapJsPath", DefaultValue = Register.DefaultBootstrapJsPath)]
		public string BootstrapJsPath
		{
			get { return (string)base["bootstrapJsPath"]; }
			set { base["bootstrapJsPath"] = value; }
		}

        [ConfigurationProperty("bootstrapJsSriHash", DefaultValue = Register.DefaultBootstrapJsSriHash)]
        public string BootstrapJsSriHash
        {
            get { return (string)base["bootstrapJsSriHash"]; }
            set { base["bootstrapJsSriHash"] = value; }
        }

        [ConfigurationProperty("bootstrapCssPath", DefaultValue = Register.DefaultBootstrapCssPath)]
		public string BootstrapCssPath
		{
			get { return (string)base["bootstrapCssPath"]; }
			set { base["bootstrapCssPath"] = value; }
		}

		[ConfigurationProperty("bootstrapVersion", DefaultValue = Register.DefaultBootstrapVersion)]
		[TypeConverter(typeof(Utility.String2Version))]
		public Version BootstrapVersion
		{
			get { return Version.Parse((string)base["bootstrapVersion"]); }
			set { base["bootstrapVersion"] = value.ToString(); }
		}

		[ConfigurationProperty("bootstrapDatePickerJsPath", DefaultValue = Register.DefaultBootstrapDatePickerJsPath)]
		public string BootstrapDatePickerJsPath
		{
			get { return (string)base["bootstrapDatePickerJsPath"]; }
			set { base["bootstrapDatePickerJsPath"] = value; }
		}

		[ConfigurationProperty("bootstrapDatePickerCssPath", DefaultValue = Register.DefaultBootstrapDatePickerCssPath)]
		public string BootstrapDatePickerCssPath
		{
			get { return (string)base["bootstrapDatePickerCssPath"]; }
			set { base["bootstrapDatePickerCssPath"] = value; }
		}

		[ConfigurationProperty("bootstrapTimePickerJsPath", DefaultValue = Register.DefaultBootstrapTimePickerJsPath)]
		public string BootstrapTimePickerJsPath
		{
			get { return (string)base["bootstrapTimePickerJsPath"]; }
			set { base["bootstrapTimePickerJsPath"] = value; }
		}

		[ConfigurationProperty("bootstrapTimePickerCssPath", DefaultValue = Register.DefaultBootstrapTimePickerCssPath)]
		public string BootstrapTimePickerCssPath
		{
			get { return (string)base["bootstrapTimePickerCssPath"]; }
			set { base["bootstrapTimePickerCssPath"] = value; }
		}

		[ConfigurationProperty("iconsCssPath", DefaultValue = N2.Resources.Register.DefaultIconsCssPath)]
		public string IconsCssPath
		{
			get { return (string)base["iconsCssPath"]; }
			set { base["iconsCssPath"] = value; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Edit;
using System.Web;

namespace N2.Resources
{
	/// <summary>
	/// Methods to register styles and javascripts.
	/// </summary>
	public static class Register
	{
		/// <summary>
		/// Set the resource paths to the defaults. This happens BEFORE the config file is loaded, right at the app start-up. 
		/// </summary>
		static Register()
		{
			AngularJsRoot = DefaultAngularJsRoot;
            AngularJsSriHash = DefaultAngularJsSriHash;
            AngularJsResourceSriHash = DefaultAngularJsResourceSriHash;
            AngularJsSanitizeSriHash = DefaultAngularJsSanitizeSriHash;
            AngularJsRouteSriHash = DefaultAngularJsRouteSriHash;
            AngularStrapJsPath = DefaultAngularStrapJsRoot;
            AngularStrapJsSriHash = DefaultAngularStrapJsSriHash;
            BootstrapCssPath = DefaultBootstrapCssPath;
			BootstrapJsPath = DefaultBootstrapJsPath;
            BootstrapJsSriHash = DefaultBootstrapJsSriHash;
            BootstrapDatePickerJsPath = DefaultBootstrapDatePickerJsPath;
			BootstrapTimePickerJsPath = DefaultBootstrapTimePickerJsPath;
			BootstrapDatePickerCssPath = DefaultBootstrapDatePickerCssPath;
			BootstrapTimePickerCssPath = DefaultBootstrapTimePickerCssPath;
			BootstrapVersion = new Version(DefaultBootstrapVersion);
			CkEditorJsPath = DefaultCkEditorPath;
            CkEditorJsSriHash = DefaultCkEditorSriHash;
			FancyboxCssPath = DefaultFancyboxCssPath;
			FancyboxJsPath = DefaultFancyboxJsPath;
            FancyboxJsSriHash = DefaultFancyboxJsSriHash;
            IconsCssPath = DefaultIconsCssPath;
			JQueryJsPath = DefaultJQueryJsPath;
            JQueryJsSriHash = DefaultJQueryJsSriHash;
            JQueryUiPath = DefaultJQueryUiJsPath;
            JQueryUiSriHash = DefaultJQueryUiJsSriHash;
			JQueryPluginsPath = DefaultJQueryPluginsPath;
			PartsJsPath = DefaultPartsJsPath;
			PartsCssPath = DefaultPartsCssPath;
			AngularUiJsPath = DefaultAngularUiJsPath;
            AngularUiJsSriHash = DefaultAngularUiJsSriHash;
        }

		private static bool? _debug;
		/// <summary>Whether javascript resources should be uncompressed.</summary>
		public static bool Debug
		{
			get { return _debug ?? (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled); }
			set { _debug = value; }
		}

		/// <summary>The jQuery version used by N2.</summary>
		public const string JQueryVersion = "1.12.4";
		public const string JQueryUiVersion = "1.11.4";
        public const string AngularJsVersion = "1.5.11";
        public const string CkEditorVersion = "4.5.11";
        public const string DefaultBootstrapVersion = "2.3.2";
              
        public const string DefaultIconsCssPath = "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css";

        public const string DefaultJQueryJsPath = "https://cdnjs.cloudflare.com/ajax/libs/jquery/" + JQueryVersion + "/jquery.min.js";
        public const string DefaultJQueryJsSriHash = "sha384-nvAa0+6Qg9clwYCGGPpDQLVpLNn0fRaROjHqs13t4Ggj3Ez50XnGQqc/r8MhnRDZ";

        public const string DefaultJQueryUiJsPath = "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/" + JQueryUiVersion + "/jquery-ui.js";
        public const string DefaultJQueryUiJsSriHash = "sha384-YwCdhNQ2IwiYajqT/nGCj0FiU5SR4oIkzYP3ffzNWtu39GKBddP0M0waDU7Zwco0";

        public const string DefaultFancyboxJsPath = "https://cdnjs.cloudflare.com/ajax/libs/fancybox/2.1.5/jquery.fancybox.min.js";
        public const string DefaultFancyboxJsSriHash = "sha384-7DcX2vpkwuiZt14mAoeYYh/IvB5WppqF8nOJMjIJtCVQfU0u+jSfDVrnKegwUQJV";
        public const string DefaultFancyboxCssPath = "https://cdnjs.cloudflare.com/ajax/libs/fancybox/2.1.5/jquery.fancybox.min.css";

        public const string DefaultAngularJsRoot = "https://cdnjs.cloudflare.com/ajax/libs/angular.js/" + AngularJsVersion + "/";
        public const string DefaultAngularJsSriHash = "sha384-t/XqMIpw+CI1H2c4qO79zYSV54Et7mMfonN2OkkgbCYWUPNXS+rm8nqILevZA3ht";
        public const string DefaultAngularJsResourceSriHash = "sha384-2bH0z8xgocV4e/bTu00sH9kj5JppKYJ+5HYZUXRcTeFanrhh9GWVdF36Jnb4wdzU";
        public const string DefaultAngularJsSanitizeSriHash = "sha384-u0KheaOmrqwkS9SRC4UcjkZdAHZn/ZE8ym1O+H2jR0kRY6KMwj8LZwhszlqeWerE";
        public const string DefaultAngularJsRouteSriHash = "sha384-R/2tpppeU5PA8CPq72mH1sJkPw3nEvuJmTM7dIUUhnhFWMdz1FYU5P8INQq+zghE";

        public const string DefaultAngularStrapJsRoot = "https://cdnjs.cloudflare.com/ajax/libs/angular-strap/0.7.8/angular-strap.min.js";
        public const string DefaultAngularStrapJsSriHash = "sha384-aOS9c5m91Nal2cWUqO4zkHuT8iDc8k8Y/XmAodM+7kRdFwYQiHaTFfSKYoOzf/gX";
        public const string DefaultAngularUiJsPath = "https://cdnjs.cloudflare.com/ajax/libs/angular-ui/0.4.0/angular-ui.min.js";
        public const string DefaultAngularUiJsSriHash = "sha384-J6KJ8D7R1fJwlV3cw1jQjqHKTZIJ9yqbNxToVNMj/+Z8IGIAiyDpik27lLQKWj+c";   
        
        public const string DefaultBootstrapJsPath = "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/" + DefaultBootstrapVersion + "/js/bootstrap.min.js";
        public const string DefaultBootstrapJsSriHash = "sha384-7sVK908dLMjOwvGD47EHg9cxk32sTpllx4Qqg0vzxEIlyBSsK9UauzrqZl8SPP0+";
        public const string DefaultBootstrapCssPath = "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/" + DefaultBootstrapVersion + "/css/bootstrap.min.css";

        public const string DefaultBootstrapDatePickerJsPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-datepicker.js";
		public const string DefaultBootstrapDatePickerCssPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-datepicker.css";
		public const string DefaultBootstrapTimePickerJsPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-timepicker.js";
		public const string DefaultBootstrapTimePickerCssPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-timepicker.css";

        //public const string DefaultCkEditorPath = "https://cdn.ckeditor.com/" + CkEditorVersion + "/full/ckeditor.js"; // no SRI support for older versions
        public const string DefaultCkEditorPath = "https://cdnjs.cloudflare.com/ajax/libs/ckeditor/" + CkEditorVersion + "/ckeditor.js";
        public const string DefaultCkEditorSriHash = "sha384-e7o0wl2TgSTFUBPYrfaUb+Y7PAolCx1cpl9NdhIYVKgWckNZmiWdeRPfVfWqLWmy";        

        public const string DefaultJQueryPluginsPath = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + JQueryVersion;
		public const string DefaultPartsJsPath = "{ManagementUrl}/Resources/Js/parts.js?v=" + JQueryVersion;
		public const string DefaultPartsCssPath = "{ManagementUrl}/Resources/Css/parts.css?v=" + JQueryVersion;
        public const string DefaultFlagsCssPath = "{ManagementUrl}/Resources/icons/flags.css";

        /// <summary>Path to jQuery.</summary>
        public static string JQueryJsPath { get; set; }

        public static string JQueryJsSriHash { get; set; }

        public static string PreviewJQueryJsPath { get; set; }

		/// <summary>The path to jQuery UI javascript bundle.</summary>
		public static string JQueryUiPath { get; set; }

        public static string JQueryUiSriHash { get; set; }

        /// <summary>The path to the jquery plugins used by N2.</summary>
        public static string JQueryPluginsPath { get; set; }

		/// <summary> The path to angularjs folder used by N2. </summary>
		public static string AngularJsRoot { get; set; }

		public static string PreviewAngularJsRoot { get; set; }

		/// <summary> The path to angularjs used by N2. </summary>
		public static string AngularJsPath { get { return AngularJsRoot + "angular.js"; } }

        public static string AngularJsSriHash { get; set; }

        /// <summary> The path to angular-resources used by N2. </summary>
        public static string AngularJsResourcePath { get { return AngularJsRoot + "angular-resource.js"; } }

        public static string AngularJsResourceSriHash { get; set; }

        /// <summary> The path to angular-resources used by N2. </summary>
        public static string AngularJsSanitizePath { get { return AngularJsRoot + "angular-sanitize.js"; } }

        public static string AngularJsSanitizeSriHash { get; set; }

        public static string AngularJsRouteSriHash { get; set; }

        /// <summary>The path to the CKeditor script</summary>
        public static string CkEditorJsPath { get; set; }

        public static string CkEditorJsSriHash { get; set; }

        /// <summary>The path to the parts script.</summary>
        public static string PartsJsPath { get; set; }

		/// <summary>The path to the parts css.</summary>
		public static string PartsCssPath { get; set; }

		/// <summary>The path to Twitter Bootstrap CSS library.</summary>
		public static string BootstrapCssPath { get; set; }

		/// <summary>The path to Twitter Bootstrap JS library.</summary>
		public static string BootstrapJsPath { get; set; }

        public static string BootstrapJsSriHash { get; set; }

        public static Version BootstrapVersion { get; set; }

		/// <summary>The path to the icon css classes.</summary>
		public static string IconsCssPath { get; set; }

		/// <summary>The path to Fancybox JS library.</summary>
		public static string FancyboxJsPath { get; set; }

        public static string FancyboxJsSriHash { get; set; }

        /// <summary>The path to Fancybox CSS resources.</summary>
        public static string FancyboxCssPath { get; set; }

		public static string AngularStrapJsPath { get; set; }

        public static string AngularStrapJsSriHash { get; set; }

        public static string AngularUiJsPath { get; set; }

        public static string AngularUiJsSriHash { get; set; }

        public static string BootstrapDatePickerCssPath { get; set; }

		public static string BootstrapDatePickerJsPath { get; set; }

		public static string BootstrapTimePickerCssPath { get; set; }

		public static string BootstrapTimePickerJsPath { get; set; }

		#region page StyleSheet

		/// <summary>Register an embedded style sheet reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="type">The type whose assembly contains the embedded style sheet.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public static void StyleSheet(this Page page, Type type, string resourceName)
		{
			StyleSheet(page, page.ClientScript.GetWebResourceUrl(type, resourceName), Media.All);
		}

		/// <summary>Register a style sheet reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="resourceUrl">The url to the style sheet to register.</param>
		public static void StyleSheet(this Page page, string resourceUrl)
		{
			StyleSheet(page, resourceUrl, Media.All);
		}

		/// <summary>Register a style sheet reference in the page's header with media type.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="resourceUrl">The url to the style sheet to register.</param>
		/// <param name="media">The media type to assign, e.g. print.</param>
		public static void StyleSheet(this Page page, string resourceUrl, Media media)
		{
			if (page == null) throw new ArgumentNullException("page");
			if (resourceUrl == null) throw new ArgumentNullException("resourceUrl");

			resourceUrl = Url.ToAbsolute(resourceUrl);

			if (page.Items[resourceUrl] == null)
			{
				var holder = GetPlaceHolder(page);
				if (holder == null)
					return;

				var link = new HtmlLink {Href = Url.ResolveTokens(resourceUrl)};
				link.Attributes["type"] = "text/css";
				link.Attributes["media"] = media.ToString().ToLower();
				link.Attributes["rel"] = "stylesheet";
				holder.Controls.Add(link);

				page.Items[resourceUrl] = true;
			}
		}

		#endregion

		#region page JavaScript

		/// <summary>Register an embedded javascript resource reference in the page header.</summary>
		/// <param name="page">The page in whose header to register the javascript.</param>
		/// <param name="type">The type in whose assembly the javascript is embedded.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public static void JavaScript(this Page page, Type type, string resourceName)
		{
			JavaScript(page, page.ClientScript.GetWebResourceUrl(type, resourceName));
		}

		/// <summary>Register an embedded javascript resource reference in the page header with options.</summary>
		/// <param name="page">The page in whose header to register the javascript.</param>
		/// <param name="type">The type in whose assembly the javascript is embedded.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <param name="options">Options flag.</param>
		public static void JavaScript(this Page page, Type type, string resourceName, ScriptOptions options)
		{
			JavaScript(page, page.ClientScript.GetWebResourceUrl(type, resourceName), options);
		}

		/// <summary>Registers a script block on a page.</summary>
		/// <param name="page">The page onto which to added the script.</param>
		/// <param name="script">The script to add.</param>
		/// <param name="position">Where to add the script.</param>
		/// <param name="options">Script registration options.</param>
		public static void JavaScript(this Page page, string script, ScriptPosition position, ScriptOptions options, string sriHash = null)
		{
			if (page == null) throw new ArgumentNullException(nameof(page));

			if (position == ScriptPosition.Header)
			{
				JavaScript(page, script, options, sriHash);
			}
			else if (position == ScriptPosition.Bottom)
			{
				string key = script.GetHashCode().ToString(CultureInfo.InvariantCulture);
                if (options.Is(ScriptOptions.None))
                {
                    page.ClientScript.RegisterStartupScript(typeof(Register), key, script);
                }
                else if (options.Is(ScriptOptions.ScriptTags))
                {
                    page.ClientScript.RegisterStartupScript(typeof(Register), key, script, true);
                }
                else if (options.Is(ScriptOptions.DocumentReady))
                {
                    page.JQuery();
                    page.ClientScript.RegisterStartupScript(typeof(Register), key, EmbedDocumentReady(script), true);
                }
                else if (options.Is(ScriptOptions.Include))
                {
                    page.ClientScript.RegisterClientScriptInclude(key, Url.ResolveTokens(script));
                }
                else
                {
                    throw new ArgumentException("options");
                }
            }
			else
            {
                throw new ArgumentException("position");
            }
        }

		private static string EmbedDocumentReady(string script)
		{
			return "jQuery(document).ready(function(){" + script + "});";
		}

		public static void JavaScript(this Page page, string script, ScriptOptions options, string sriHash = null)
		{
			if (page == null) return;

			if (page.Items[script] == null)
			{
				PlaceHolder holder = GetPlaceHolder(page);

				if (options.Is(ScriptOptions.Include))
				{
					AddScriptInclude(page, script, holder, options.Is(ScriptOptions.Prioritize), sriHash);
				}
				else if (options.Is(ScriptOptions.None))
				{
					holder.Page.Items[script] = AddString(holder, script, options.Is(ScriptOptions.Prioritize));
				}
				else
				{
					Script scriptHolder = GetScriptHolder(page);
					if (options.Is(ScriptOptions.ScriptTags))
					{
						holder.Page.Items[script] = AddString(scriptHolder, script + Environment.NewLine, Is(options, ScriptOptions.Prioritize));
					}
					else if (options.Is(ScriptOptions.DocumentReady))
					{
						JQuery(page);
						holder.Page.Items[script] = AddString(scriptHolder, EmbedDocumentReady(script) + Environment.NewLine, options.Is(ScriptOptions.Prioritize));
					}
				}
			}
		}

		private class Script : Control
		{
			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write("<script type=\"text/javascript\">//<![CDATA[\n");
				base.Render(writer);
				writer.Write("\n//]]>\n</script>");
			}
		}

		private static Literal AddString(Control holder, string script, bool priority)
		{
			var l = new Literal { Text = script };
			if (priority)
				holder.Controls.AddAt(0, l);
			else
				holder.Controls.Add(l);
			return l;
		}

		private static Control AddScriptInclude(Page page, string resourceUrl, Control holder, bool priority, string sriHash = null)
		{
			if (page == null) throw new ArgumentNullException(nameof(page));

			var script = new HtmlGenericControl("script");
			page.Items[resourceUrl] = script;

			resourceUrl = Url.ResolveTokens(resourceUrl);

			script.Attributes["src"] = resourceUrl;
			script.Attributes["type"] = "text/javascript";

            if(!string.IsNullOrWhiteSpace(sriHash))
            {
                script.Attributes["crossorigin"] = "anonymous";
                script.Attributes["integrity"] = sriHash;
            }

			if (priority)
            {
                holder.Controls.AddAt(0, script);
            }
            else
            {
                holder.Controls.Add(script);
            }

            return script;
		}

        /// <summary>Registers a script reference in the page's header.</summary>
        /// <param name="page">The page onto which to register the javascript.</param>
        /// <param name="resourceUrl">The url to the javascript to register.</param>
        /// <param name="sriHash">SRI is a new W3C specification that allows web developers to ensure that resources hosted on third-party servers have not been tampered with. Use of SRI is recommended as a best-practice, whenever libraries are loaded from a third-party source.</param>
        public static void JavaScript(this Page page, string resourceUrl, string sriHash = null)
		{
			if (page == null) throw new ArgumentNullException(nameof(page));
			if (resourceUrl == null) throw new ArgumentNullException(nameof(resourceUrl));

			JavaScript(page, resourceUrl, ScriptOptions.Include, sriHash);
		}

		public static void JQuery(this Page page)
		{
			JavaScript(page, Url.ResolveTokens(JQueryJsPath), ScriptPosition.Header, ScriptOptions.Prioritize | ScriptOptions.Include, JQueryJsSriHash);
		}

		private static Script GetScriptHolder(Page page)
		{
			var holder = GetPlaceHolder(page);
			var scripts = page.Items["N2.Resources.scripts"] as Script;
			if (scripts == null)
			{
				page.Items["N2.Resources.scripts"] = scripts = new Script();
				holder.Controls.Add(scripts);
			}
			return scripts;
		}

		private static PlaceHolder GetPlaceHolder(Page page)
		{
			if (page == null) return null;

			var holder = page.Items["N2.Resources.holder"] as PlaceHolder;
			if (holder != null)
				return holder;

			page.Items["N2.Resources.holder"] = holder = new PlaceHolder();

			if (page.Header == null)
				page.Controls.Add(holder);
			else if (page.Header.Controls.Count > 0)
				page.Header.Controls.AddAt(1, holder);
			else
				page.Header.Controls.Add(holder);

			return holder;
		}

		#region TabPanel

		public static void TabPanel(Page page, string selector, bool registerTabCss)
		{
			var key = "N2.Resources.TabPanel" + selector;
			if (page.Items[key] != null)
				return;

			JQuery(page);
			JQueryPlugins(page);

			var script = String.Format("jQuery(\"{0}\").n2tabs(\"{1}\",location.hash);", selector, selector.Replace('.', '_'));
			JavaScript(page, script, ScriptOptions.DocumentReady);
			page.Items[key] = new object();
			if (registerTabCss)
			{
				StyleSheet(page, Url.ResolveTokens("{ManagementUrl}/Resources/Css/TabPanel.css"));
			}
		}

		public static void TabPanel(Page page, string selector)
		{
			TabPanel(page, selector, true);
		}

		private static bool Is(this ScriptOptions options, ScriptOptions expectedOption)
		{
			return (options & expectedOption) == expectedOption;
		}
		#endregion

		public static void JQueryPlugins(this Page page)
		{
			page.JQuery();
			page.JavaScript(JQueryPluginsPath.ResolveUrlTokens(), ScriptPosition.Header, ScriptOptions.Include);
		}

		public static void JQueryUi(this Page page)
		{
			page.JQuery();
			page.JavaScript(JQueryUiPath.ResolveUrlTokens(), ScriptPosition.Header, ScriptOptions.Include, JQueryUiSriHash);
		}

		public static void CkEditor(this Page page)
		{
			JavaScript(page, CkEditorJsPath.ResolveUrlTokens(), CkEditorJsSriHash);
		}

		#endregion

		#region MVC
		public static bool RegisterResource(ICollection<string> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return true;

			stateCollection.Add(resourceUrl);
			return false;
		}

		public static bool IsRegistered(ICollection<string> stateCollection, string resourceUrl)
		{
			return stateCollection.Contains(resourceUrl);
		}

		public static string JavaScript(ICollection<string> stateCollection, string resourceUrl, string sriHash = null)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return null;

			RegisterResource(stateCollection, resourceUrl);

            if (!string.IsNullOrWhiteSpace(sriHash))
            {
                return String.Format("<script type=\"text/javascript\" src=\"{0}\" crossorigin=\"anonymous\" integrity=\"{1}\"></script>", Url.ResolveTokens(resourceUrl), sriHash);
            }
            else
            {
                return String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Url.ResolveTokens(resourceUrl));
            }
		}

		public static string JavaScript(ICollection<string> stateCollection, string script, ScriptOptions options, string cspScriptNonce = "")
		{
			const string scriptFormat = "<script type=\"text/javascript\" nonce=\"{1}\">//<![CDATA[\n{0}//]]></script>";

			if (IsRegistered(stateCollection, script))
				return null;

			RegisterResource(stateCollection, script);

			if (options == ScriptOptions.Include)
				return JavaScript(stateCollection, script);
			if (options == ScriptOptions.None)
				return script;
			if (options == ScriptOptions.ScriptTags)
				return String.Format(scriptFormat, script, cspScriptNonce);
			if (options == ScriptOptions.DocumentReady)
				return String.Format(scriptFormat, EmbedDocumentReady(script), cspScriptNonce);

			throw new NotSupportedException(options + " not supported");
		}

		public static string JQuery(ICollection<string> stateCollection)
		{
			return JavaScript(stateCollection, JQueryJsPath.ResolveUrlTokens(), JQueryJsSriHash);
		}

		public static string JQueryPlugins(ICollection<string> stateCollection)
		{
			return JQuery(stateCollection) + JavaScript(stateCollection, JQueryPluginsPath.ResolveUrlTokens());
		}

		public static string JQueryUi(ICollection<string> stateCollection)
		{
			return JQuery(stateCollection) + JavaScript(stateCollection, JQueryUiPath.ResolveUrlTokens(), JQueryUiSriHash);
		}

		[Obsolete("TinyMCE no longer supported; use CkEditor() instead.")]
		public static string TinyMCE(ICollection<string> stateCollection)
		{
			return JavaScript(stateCollection, Url.ResolveTokens(CkEditorJsPath));
		}

		public static string StyleSheet(ICollection<string> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return null;

			RegisterResource(stateCollection, resourceUrl);

			return String.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", Url.ResolveTokens(resourceUrl));
		}
		#endregion

		internal static string SelectedQueryKeyRegistrationScript()
		{
			return "n2SelectedQueryKey = '" + SelectionUtility.SelectedQueryKey + "';";
		}

		internal static void FrameInteraction(this Page page)
		{
			JavaScript(page, "{ManagementUrl}/Resources/Js/frameInteraction.js");
		}
	}
}

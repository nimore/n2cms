using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Configuration;
using N2.Resources;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Details;
using System.Text.RegularExpressions;

namespace N2.Management.Files.FileSystem
{
	public partial class FileUpload : System.Web.UI.UserControl
	{
		protected int maxFileSize = 4096 * 1024 - 10000;
		private readonly Engine.Logger<FileUpload> logger;
		protected SelectionUtility Selection { get; set; }

		protected string GetLocalResourceString(string resourceKey, string defaultText = null)
		{
			try
			{
				return (string)GetLocalResourceObject(resourceKey);
			}
			catch (InvalidOperationException)
			{
				return defaultText;
			}
		}

		protected void OnAlternativeCommand(object sender, CommandEventArgs args)
		{
			if (fuAlternative.PostedFile.ContentLength > 0)
			{
				string filename = Regex.Replace(System.IO.Path.GetFileName(fuAlternative.PostedFile.FileName), EditableFileUploadAttribute.InvalidCharactersExpression, "-");
				string url = Selection.SelectedItem.Url.TrimEnd('/') + "/" + filename;
				N2.Context.Current.Resolve<IFileSystem>().WriteFile(url, fuAlternative.PostedFile.InputStream);
				Page.RefreshManagementInterface(Selection.SelectedItem);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			Selection = Page.GetSelection();

			base.OnInit(e);

			InitMaxFileSize();

			Page.JQueryUi();
		}

		private void InitMaxFileSize()
		{
			try
			{
				var config = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
				maxFileSize = config.MaxRequestLength * 1024 - 10000;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
		}

		// commands
	}
}

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using PsPdfKitOnWinUI3.Lib;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PsPdfKitOnWinUI3.RequestFilter
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static string GetHeaderString(IDictionary<string, string> headers) =>
            string.Join(Environment.NewLine, headers.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

        public MainWindow()
        {
            this.InitializeComponent();
            webView.Loaded += WebViewOnLoaded;
        }

        private async void WebViewOnLoaded(object sender, RoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();
            var folder = ApplicationData.Current.LocalFolder.Path;

            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.pspdf.zip", $"{folder}\\build.zip");
            FileManager.ExtractZip($"{folder}\\build.zip", folder);

            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.document.pdf", $"{folder}\\document.pdf");
            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.index.html", $"{folder}\\index.html");

            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += CoreWebView2OnWebResourceRequested;

            webView.CoreWebView2.Navigate($"{folder}\\index.html");
        }

        private void CoreWebView2OnWebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            string assetsFilePath = args.Request.Uri.Replace("file:///", "");

            var contentType = StaticContentProvider.GetResponseContentTypeOrDefault(assetsFilePath);
            var headers = GetHeaderString(StaticContentProvider.GetResponseHeaders(contentType));

            try
            {
                var fs = File.OpenRead(assetsFilePath);

                args.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                    fs.AsRandomAccessStream(), 200, "OK", headers);
            }
            catch (Exception)
            {
                args.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                    null, 404, "Not found", "");
            }
        }
    }
}

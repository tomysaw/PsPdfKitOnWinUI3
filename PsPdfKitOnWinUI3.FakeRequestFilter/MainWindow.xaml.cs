using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using PsPdfKitOnWinUI3.Lib;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PsPdfKitOnWinUI3.FakeRequestFilter
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
            var host = "demo";

            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.pspdf.zip", $"{folder}\\build.zip");
            FileManager.ExtractZip($"{folder}\\build.zip", folder);

            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.document.pdf", $"{folder}\\document.pdf");
            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.index-arts.html", $"{folder}\\index.html");

            webView.CoreWebView2.AddWebResourceRequestedFilter("http://artifacts.com/*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += CoreWebView2OnWebResourceRequested;

            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(host, folder, CoreWebView2HostResourceAccessKind.Allow);
            webView.CoreWebView2.Navigate($"http://{host}/index.html");
        }

        private void CoreWebView2OnWebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            string assetsFilePath = ApplicationData.Current.LocalFolder.Path + "\\" + args.Request.Uri.Substring("http://artifacts.com/*".Length - 1).Replace('/', '\\');

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

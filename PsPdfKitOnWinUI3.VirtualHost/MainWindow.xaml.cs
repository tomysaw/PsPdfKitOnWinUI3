using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using Windows.Storage;
using PsPdfKitOnWinUI3.Lib;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PsPdfKitOnWinUI3.VirtualHost
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            webView.Loaded += WebViewOnLoaded;
        }

        private async void WebViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await webView.EnsureCoreWebView2Async();
            var folder = ApplicationData.Current.LocalFolder.Path;
            var host = "demo";

            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.pspdf.zip", $"{folder}\\build.zip");
            FileManager.ExtractZip($"{folder}\\build.zip", folder);

            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.document.pdf", $"{folder}\\document.pdf");
            FileManager.WriteResourceToFile("PsPdfKitOnWinUI3.Lib.index.html", $"{folder}\\index.html");

            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(host, folder, CoreWebView2HostResourceAccessKind.Allow);
            webView.CoreWebView2.Navigate($"http://{host}/index.html");
        }
    }
}
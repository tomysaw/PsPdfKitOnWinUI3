# PsPdfKit integration with WinUI3
## PsPdfKitOnWinUI3.VirtualHost
JS file doesn't load because it requires charset: utf-8

## PsPdfKitOnWinUI3.RequestFilter
The document doesn't load
Not allowed to load local resource: blob:null/d2798a91-f4a4-45d4-b18c-70546ecafd12
(probably because the browser tries to access the local file)

## PsPdfKitOnWinUI3.FakeRequestFilter
The hack that loads js lib from fake "artifacts.com" website. These requests are intercepted and mapped to local files.
WASM is blocked by CORS. "disableWebAssemblyStreaming" doesn't help for some reason

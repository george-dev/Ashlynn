namespace Ashlynn.UI

open System
open System.Web

module Utils = 
    
    let GetBaseUrl(request: HttpRequestBase) = 
        let url = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath
        url.TrimEnd('/')
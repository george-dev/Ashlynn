namespace Ashlynn.UI

open System.Configuration

type UIConfig() = 
    static member Title 
        with get() = ConfigurationManager.AppSettings.["Title"]

    static member User 
        with get() = ConfigurationManager.AppSettings.["User"]

    static member Password
        with get() = ConfigurationManager.AppSettings.["Password"]


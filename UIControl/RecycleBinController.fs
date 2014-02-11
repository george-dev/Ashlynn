namespace Ashlynn.UI

open System
open System.Web
open System.Web.Mvc
open Ashlynn.Logic
open Ashlynn.Utils

[<HandleError>]
[<Authorize>]
type RecycleBinController() =
    inherit Controller()

    member x.Index () =
        x.View(loadBin()) :> ActionResult

    member x.Delete id =
        id |> parseGuid |> delete
        EmptyResult() :> ActionResult

    member x.Restore id =
        id |> parseGuid |> restore
        EmptyResult() :> ActionResult
namespace Ashlynn.UI

open System
open System.Web
open System.Web.Mvc
open Ashlynn.Logic
open Ashlynn.Utils

[<HandleError>]
[<Authorize>]
type HomeController() =
    inherit Controller()

    member x.Index () =
        x.View() :> ActionResult

    member x.Search query = 
        base.PartialView("EntryList", query |> search)

    member x.LoadRecent () = 
        base.PartialView("EntryList", loadLast 20)

    member x.MoveToBin id = 
        id |> parseGuid |> moveToBin
        EmptyResult() :> ActionResult

    member x.DisplayText id = 
        let entry = id |> parseGuid |> load
        JsonResult(Data = entry.Text)

    [<HttpGet>]
    member x.DownloadFile entryID fileID = 
        let attachment = entryID |> parseGuid |> load |> getAttachment (fileID |> parseGuid)
        x.File(attachment.Content, attachment.ContentType, attachment.Name)
namespace Ashlynn.UI

open System
open System.Collections
open System.Collections.Generic
open System.Web
open System.Web.Mvc
open System.IO
open Ashlynn.Object
open Ashlynn.Utils
open Ashlynn.Logic

type SaveModel () = 
    member val ID = "" with get, set
    member val Name = "" with get, set
    member val Tags = "" with get, set
    [<AllowHtml>] member val Details = "" with get, set
    member val Attachments = (Array.empty<string> :> ICollection<string>) with get, set

[<HandleError>]
[<Authorize>]
type AddController() =
    inherit Controller()

    let getAttachment (file: HttpPostedFileBase) = 
        let contents = Array.zeroCreate (file.ContentLength)
        file.InputStream.Read(contents, 0, file.ContentLength) |> ignore
        { ID = Guid.Empty; Name = Path.GetFileName(file.FileName); ContentType = file.ContentType; Content = contents }

    member x.Index (id: string) =
        let entry = match id |> parseGuid with
                    | id when id = Guid.Empty -> Entry.empty
                    | id -> load id
        x.View(entry) :> ActionResult

    member x.Save (saveModel: SaveModel) = 
        let id = saveModel.ID |> parseGuid
        let newAttachments = x.Request.Files.AllKeys
                             |> Array.map (fun name -> x.Request.Files.[name])
                             |> Array.filter (fun file -> file <> null && file.ContentLength > 0)
                             |> Array.map getAttachment

        let existingAttachments = if saveModel.Attachments <> null && id <> Guid.Empty then
                                    saveModel.Attachments |> Seq.map parseGuid
                                                          |> Seq.filter (fun id -> id <> Guid.Empty)
                                                          |> Seq.map (fun id -> { Attachment.empty with ID = id } )
                                                          |> Seq.toArray
                                  else [| |]

        let entry = { ID = id
                      Date = DateTime.Now
                      Name = saveModel.Name
                      Tags = saveModel.Tags
                      Text = saveModel.Details
                      Attachments = Array.concat [ newAttachments; existingAttachments] }
        entry |> save
        x.RedirectToAction("Index", "Home") :> ActionResult
        

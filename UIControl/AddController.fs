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

    member x.Save (id, name, tags, details, attachments: ICollection<string>) = 
        let id = id |> parseGuid
        let newAttachments = x.Request.Files.AllKeys
                             |> Array.map (fun name -> x.Request.Files.[name])
                             |> Array.filter (fun file -> file <> null && file.ContentLength > 0)
                             |> Array.map getAttachment

        let existingAttachments = if attachments <> null && id <> Guid.Empty then
                                    attachments |> Seq.map parseGuid
                                                |> Seq.filter (fun id -> id <> Guid.Empty)
                                                |> Seq.map (fun id -> { Attachment.empty with ID = id } )
                                                |> Seq.toArray
                                  else [| |]

        let entry = { ID = id
                      Date = DateTime.Now
                      Name = name
                      Tags = tags
                      Text = details
                      Attachments = Array.concat [ newAttachments; existingAttachments] }
        entry |> save
        x.RedirectToAction("Index", "Home") :> ActionResult
        

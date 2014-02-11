namespace Ashlynn

open System
open Ashlynn.DataAccess
open Ashlynn.Utils
open Ashlynn.Object
open Ashlynn.Indexing
open Ashlynn.Config
open Ashlynn.Archiving

module Logic = 

    let getAttachment id entry = 
        match entry.Attachments |> Array.tryFind (fun att -> att.ID = id) with
        | Some attachment -> let archive = loadAttachmentArchive entry.ID
                             match archive with
                             | Some bytes -> { attachment with Content = loadAttachment bytes (id.ToString()) }
                             | None -> attachment
        | None -> failwith "Attachment could not be loaded."

    let private getAttachments entry = 
        let attachments = entry.Attachments |> defaultIfNull Array.empty

        if entry.ID = Guid.Empty then
            attachments |> Array.map (fun att -> { att with ID = Guid.NewGuid() })
        else
            let oldAttachments = attachments |> Array.filter (fun att -> att.ID <> Guid.Empty)
            let oldAttachmentsMap = match oldAttachments, DataAccess.loadAttachmentArchive (entry.ID) with
                                    | [| |], _ -> Map.empty
                                    | _, None  -> Map.empty
                                    | items, Some archive -> let oldEntry = DataAccess.load (entry.ID)
                                                             let attMap = oldEntry.Attachments
                                                                         |> Array.map (fun att -> att.ID, att)
                                                                         |> Map.ofArray
                                                             items
                                                             |> Array.collect (fun att -> match attMap |> Map.tryFind (att.ID) with
                                                                                          | Some data -> [| data.ID, { ID = data.ID
                                                                                                                       Name = data.Name
                                                                                                                       ContentType = data.ContentType
                                                                                                                       Content = loadAttachment archive (data.ID.ToString()) } |]
                                                                                          | None -> Array.empty)
                                                             |> Map.ofArray
            attachments
            |> Array.map (fun att -> if att.ID = Guid.Empty then { att with ID = Guid.NewGuid() }
                                     else oldAttachmentsMap.[att.ID])
    
    let save entry = 
        if String.IsNullOrEmpty(entry.Name) then failwith "Name cannot be empty."

        let entry = { ID = if entry.ID = Guid.Empty then Guid.NewGuid() else entry.ID
                      Date = entry.Date
                      Name = entry.Name |> defaultIfNull ""
                      Text = entry.Text |> defaultIfNull ""
                      Tags = entry.Tags |> defaultIfNull ""
                      Attachments = entry |> getAttachments }

        DataAccess.save entry
        Indexing.indexEntry (entry.ID)

    let load id = 
        DataAccess.load id

    let search(query: string) = 
        match query with
        | Empty -> Array.empty
        | Value query -> query |> Indexing.search 
                               |> Array.map DataAccess.load

    let moveToBin id = 
        id |> moveEntry entryContainerName binContainerName
        id |> Indexing.deleteEntry
        ()
        
    let restore id = 
        id |> moveEntry binContainerName entryContainerName
        id |> Indexing.indexEntry
        ()

    let delete id = 
        id |> DataAccess.deleteEntry

    let loadLast num =
        entryContainerName |> DataAccess.loadLast (Some num) |> Array.ofSeq

    let loadBin () = 
        binContainerName |> DataAccess.loadLast None |> Array.ofSeq
        

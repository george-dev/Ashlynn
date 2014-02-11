namespace Ashlynn

open System
open System.IO
open System.IO.Compression
open Ashlynn.Domain

module Archiving = 
    

    let zipAttachments entry =
        use memoryStream = new MemoryStream()
        using (new ZipArchive(memoryStream, ZipArchiveMode.Create)) (fun archive ->
            entry.Attachments |> Array.iter (fun attachment -> 
                                                let archiveEntry = archive.CreateEntry(attachment.ID.ToString())
                                                use stream = archiveEntry.Open()
                                                stream.Write(attachment.Content, 0, attachment.Content.Length)))
        memoryStream.ToArray()

    let loadAttachment (zipArchive: byte[]) id =
        use memoryStream = new MemoryStream(zipArchive)
        use entryStream = new MemoryStream()
        use archive = new ZipArchive(memoryStream, ZipArchiveMode.Read)
        let entries = archive.Entries 
                      |> Seq.filter (fun entry -> entry.FullName.Equals(id, StringComparison.OrdinalIgnoreCase))
                      |> Seq.map (fun entry -> entry.Open())
        if entries |> Seq.isEmpty then
            failwith (sprintf "Attachment %s was not found." id)
        else
            entries |> Seq.head |> (fun stream -> stream.CopyTo(entryStream))
            entryStream.ToArray()



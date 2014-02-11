namespace Ashlynn

open System
open System.Xml
open System.Xml.Linq
open System.IO
open System.Text
open Ashlynn.Utils
open Ashlynn.Config
open Ashlynn.Object
open Ashlynn.Archiving

module DataAccess = 

    let private xName name =
        XName.Get(name)

    let private xAttr name (element: XElement) = 
        element.Attribute(XName.Get(name)).Value

    let private xElement name (container: XContainer) = 
        container.Element(name |> xName)

    let private xElements name (container: XContainer) = 
        container.Elements(name |> xName)

    let private xElementExists name (container: XContainer) = 
        not (container |> xElements name |> Seq.isEmpty)

    let private xValue (element: XElement) = 
        element.Value

    let save entry =
        let entryName = entry.ID.ToString()
        Cloud.deleteBlob entryContainerName entryName
        Cloud.deleteBlob attachmentContainerName entryName

        let xml = new XElement(xName "Entry",
                    new XElement(xName "Date", entry.Date |> stringifyDate),
                    new XElement(xName "Name", entry.Name),
                    new XElement(xName "Text", entry.Text),
                    new XElement(xName "Tags", entry.Tags.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
                                               |> Array.map (fun x -> new XElement(xName "Tag", x.Trim()))),
                    new XElement(xName "Attachments", entry.Attachments
                                                      |> Array.map (fun attachment -> new XElement(xName "File",
                                                                                        new XElement(xName "ID", attachment.ID.ToString()),
                                                                                        new XElement(xName "Name", attachment.Name),
                                                                                        new XElement(xName "ContentType", attachment.ContentType)))))
        xml.ToString() |> Encoding.UTF8.GetBytes
                       |> Cloud.saveBlob entryContainerName entryName
        if entry.Attachments.Length > 0 then
            entry |> zipAttachments
                  |> Cloud.saveBlob attachmentContainerName entryName

    let parseEntry id bytes = 
        let xml = XElement.Parse(Encoding.UTF8.GetString(bytes))
        let date = xml |> (xElement "Date") |> xValue |> parseDate
        let name = xml |> (xElement "Name") |> xValue
        let text = xml |> (xElement "Text") |> xValue
        let tags = xml |> (xElement "Tags")
                       |> xElements "Tag"
                       |> Seq.map xValue
                       |> Seq.toArray
        let tags = String.Join(", ", tags)
        let attachments = xml |> (xElement "Attachments")
                              |> (xElements "File")
                              |> Seq.map (fun file -> { ID = file |> (xElement "ID") |> xValue |> parseGuid
                                                        Name = file |> (xElement "Name") |> xValue
                                                        ContentType = file |> (xElement "ContentType") |> xValue
                                                        Content = Array.empty })
                              |> Seq.toArray
        { ID = id; Date = date; Name = name; Text = text; Tags = tags; Attachments = attachments }

    let load (id: Guid) = 
        let entryName = id.ToString()
        let blob = Cloud.getBlob entryContainerName entryName
        match blob with
        | Some blob -> parseEntry id blob
        | None -> failwith (sprintf "Entry %s cannot be loaded." entryName)

    let loadAttachmentArchive (id: Guid) = 
        Cloud.getBlob attachmentContainerName (id.ToString())

    let moveEntry source destination (id: Guid) = 
        let entryName = id.ToString()
        let blob = Cloud.getBlob (source) entryName
        match blob with
        | Some blob -> Cloud.deleteBlob (source) entryName
                       Cloud.saveBlob (destination) entryName blob        
        | None -> ()

    let deleteEntry (id: Guid) = 
        let entryName = id.ToString()
        Cloud.deleteBlob binContainerName entryName
        Cloud.deleteBlob attachmentContainerName entryName

    let loadLast num containerName = 
        containerName |> Cloud.loadBlobs num
                           |> Seq.map (fun blob -> blob.Name |> parseGuid, blob |> Cloud.getBlobBytes)
                           |> Seq.map (fun (id, bytes) -> parseEntry id bytes)

        
            


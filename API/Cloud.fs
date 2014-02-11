namespace Ashlynn

open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Auth
open Microsoft.WindowsAzure.Storage.Blob
open System
open System.IO
open System.Text
open Ashlynn.Utils

module Cloud = 

    let storageAccount = CloudStorageAccount.Parse(Config.storageConnectionString)

    let getContainer containerName = 
        let blobClient = storageAccount.CreateCloudBlobClient()
        let container = blobClient.GetContainerReference(containerName)
        container

    let saveBlob containerName blobName (blob: byte[]) = 
        let container = getContainer containerName
        container.CreateIfNotExists() |> ignore
        let blockBlob = container.GetBlockBlobReference(blobName)
        use stream = new MemoryStream(blob)
        blockBlob.UploadFromStream(stream)

    let getBlockBlob containerName blobName = 
        let container = getContainer containerName
        let blockBlob = container.GetBlockBlobReference(blobName)
        if blockBlob.Exists() then
            Some(blockBlob)
        else
            None

    let deleteBlob containerName blobName = 
        match getBlockBlob containerName blobName with 
        | Some blob -> blob.Delete()
        | None -> ()

    let getBlobBytes (blob: CloudBlockBlob) =
        use memoryStream = new MemoryStream()
        blob.DownloadToStream(memoryStream)
        memoryStream.ToArray()

    let getBlob containerName blobName = 
        match getBlockBlob containerName blobName with 
        | Some blob -> Some(blob |> getBlobBytes)
        | None -> None

    let listBlobs containerName = 
        let container = getContainer containerName
        match container.Exists() with
        | true -> container.ListBlobs() |> Seq.map (fun blob -> blob.Uri.ToString()) |> Seq.toArray
        | false -> Array.empty

    let getBlockBlobReference (blob: IListBlobItem) (container: CloudBlobContainer) = 
        let blob = (blob :?> CloudBlockBlob)
        let blobName = blob.Name
        container.GetBlockBlobReference(blobName)

    let clearContainer containerName = 
        let container = getContainer containerName
        if container.Exists() then
            container.ListBlobs() |> Seq.iter (fun blob ->
                                                    let blockBlob =  getBlockBlobReference blob container
                                                    blockBlob.Delete())

    let loadBlobs count containerName = 
        let container = getContainer containerName
        if container.Exists() then
            container.ListBlobs() |> Seq.map (fun blob -> getBlockBlobReference blob container)
                                  |> Seq.sortBy (fun blob -> blob.FetchAttributes()
                                                             let time = blob.Properties.LastModified
                                                             if time.HasValue then time.Value else DateTimeOffset.MinValue)
                                  |> Seq.toList
                                  |> List.rev
                                  |> Seq.truncate count
        else
            Seq.empty
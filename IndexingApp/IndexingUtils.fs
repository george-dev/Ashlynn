namespace Penny.IndexingApp

open System
open System.IO
open Penny.API
open Penny.Common
open Penny.Common.Utils

module IndexingUtils = 

    let reIndexEverything () = 
        Cloud.clearContainer "lucene"
        Cloud.listBlobs (Config.blobContainerName)
            |> Array.collect (fun id -> 
                                    match Guid.TryParse id with
                                    | true, id -> [| id |]
                                    | false, _   -> Array.empty)
            |> Array.iter Indexing.indexEntry

    let moveEntriesToCloud () = 
        Cloud.clearContainer (Config.blobContainerName)
        Directory.GetFiles(Config.baseDirectory)
            |> Array.map (fun path -> Path.GetFileName(path), File.ReadAllText(path))
            |> Array.iter (fun (path, text) -> Cloud.saveBlob (Config.blobContainerName) path text)
        


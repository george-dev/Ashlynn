open System
open Penny.IndexingApp

[<EntryPoint>]
let main argv = 
    match argv with
    | [| "clear" |] -> 
        printfn "clearing lucene index..."
        Penny.Common.Cloud.clearContainer "lucene"
        printfn "done. Any key to close."
    | [| "re-index" |] -> 
        printfn "re-indexing..."
        IndexingUtils.reIndexEverything()
        printfn "done. Any key to close."
    | [| "move" |] -> 
        printfn "moving entries to cloud storage..."
        IndexingUtils.moveEntriesToCloud()
        printfn "done. Any key to close."
    | _ ->
        printfn "did nothing - any key to continue"
    Console.ReadKey() |> ignore
    0
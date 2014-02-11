namespace Ashlynn

open System
open System.IO

module Utils = 

    let (|Empty|Value|) (s: string) = 
        match s with
        | null -> Empty
        | s when s.Trim().Length = 0 -> Empty
        | s -> Value (s.Trim())

    let defaultIfNull defaultVal actualVal = 
        match actualVal with
        | null -> defaultVal
        | _    -> actualVal

    let ensureDirectory path = 
        if not (Directory.Exists(path)) then 
            Directory.CreateDirectory(path) |> ignore
        path

    let parseDate date = 
        DateTime.ParseExact(date, Config.dateFormat, null)

    let tryParseDate date = 
        DateTime.TryParseExact(date, Config.dateFormat, null, Globalization.DateTimeStyles.None)

    let stringifyDate (date: DateTime) = 
        date.ToString(Config.dateFormat)

    let parseGuid id =
        match id |> Guid.TryParse  with
        | true, guid -> guid
        | _ -> Guid.Empty
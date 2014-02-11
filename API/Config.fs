namespace Ashlynn

open System
open System.IO
open System.Configuration

module Config = 

    let dateFormat = "yyyyMMdd HH:mm - dddd dd MMMM yyyy"

    let storageConnectionString = ConfigurationManager.AppSettings.["StorageConnectionString"]

    let entryContainerName = "entry"

    let attachmentContainerName = "attachment"

    let binContainerName = "bin"


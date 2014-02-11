namespace Ashlynn

open System

module Object = 
    type Attachment = 
        { ID: Guid
          Name: String
          ContentType: String
          Content: byte[] }

        static member empty = 
            { ID = Guid.Empty; Name = ""; ContentType = ""; Content = Array.empty }

    type Entry = 
        { ID: Guid
          Date: DateTime
          Name: string
          Text: string
          Tags: string
          Attachments: Attachment[] }

        static member empty = 
            { ID = Guid.Empty; Date = DateTime.MinValue; Name = ""; Text = ""; Tags = ""; Attachments = Array.empty }
namespace Ashlynn

open Lucene.Net
open Lucene.Net.Index
open Lucene.Net.QueryParsers
open Lucene.Net.Search
open Lucene.Net.Store
open Lucene.Net.Analysis.Standard
open Lucene.Net.Util
open Lucene.Net.Documents
open System
open System.IO
open Ashlynn.Utils
open Ashlynn.Cloud
open Ashlynn.DataAccess

module Indexing = 
    let private idField = "id"

    let commitToIndex work = 
        use dir = new Azure.AzureDirectory(storageAccount)
        use analyzer = new StandardAnalyzer(Version.LUCENE_30)
        use writer = new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.LIMITED)
        writer |> work
        writer.Optimize()
        writer.Commit()

    let indexEntry id = 
        commitToIndex (fun writer -> 
                            let entry = DataAccess.load id
                            let id = id.ToString()
                            writer.DeleteDocuments(Term(idField, id))

                            let date = entry.Date |> stringifyDate
                            let tags = entry.Tags.Split(',') |> Array.map (fun tag -> tag.Trim())
                            let attachments = entry.Attachments |> Array.map (fun att -> att.Name)
                            let indexDoc = Document()
                            indexDoc.Add(Field(idField, id, Field.Store.YES, Field.Index.NOT_ANALYZED))
                            indexDoc.Add(Field("date", date, Field.Store.NO, Field.Index.ANALYZED))
                            indexDoc.Add(Field("name", entry.Name, Field.Store.NO, Field.Index.ANALYZED))
                            indexDoc.Add(Field("all", date 
                                                      + " " + entry.Name
                                                      + " " + entry.Text 
                                                      + " " + String.Join(" ", tags)
                                                      + " " + String.Join(" ", attachments), Field.Store.NO, Field.Index.ANALYZED))
                            tags |> Array.iter (fun tag -> indexDoc.Add(Field("tag", tag, Field.Store.YES, Field.Index.ANALYZED)))
                            attachments |> Array.iter (fun att -> indexDoc.Add(Field("attachment", att, Field.Store.YES, Field.Index.ANALYZED)))
                            writer.AddDocument(indexDoc))

    let deleteEntry (id: Guid) = 
        commitToIndex (fun writer -> writer.DeleteDocuments(Term(idField, id.ToString())))

    let search query = 
        use analyzer = new StandardAnalyzer(Version.LUCENE_30)
        let parser = QueryParser(Version.LUCENE_30, "all", analyzer)
        let userQuery = parser.Parse(query)
        use dir = new Azure.AzureDirectory(storageAccount)
        use indexSearcher = new IndexSearcher(dir, true)
        let results = indexSearcher.Search(userQuery, 50)
        let resultDocs = results.ScoreDocs 
                        |> Array.map (fun result -> 
                            let doc = indexSearcher.Doc(result.Doc)
                            let id = doc.Get(idField)
                            id |> Guid.Parse)
        resultDocs
        



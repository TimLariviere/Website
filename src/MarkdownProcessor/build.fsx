#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget FSharp.Formatting
nuget Newtonsoft.Json
nuget Legivel //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.SystemHelper
open System.IO
open Newtonsoft.Json
open Legivel
open Legivel.Serialization

type BlogPost =
    { Name: string
      Url: string
      CreatedAt: System.DateTimeOffset }

type BlogIndex =
    { Posts: BlogPost list
      CreatedAt: System.DateTimeOffset }

type BlogPostMetadata =
    { title: string
      date: string }

let tryReadMetadata file =
    file
    |> File.ReadAllLines
    |> Array.take 4
    |> Array.reduce (fun a b -> a + "\n" + b)
    |> (fun s -> printfn "%s" s; s)
    |> Deserialize<BlogPostMetadata>
    |> List.head
    |> function
       | Succes value -> Some value.Data
       | Error err -> printfn "%A" err; None

let getTitle metadata =
    match metadata with
    | Some m -> m.title
    | None -> "No title"

let getCreationDate metadata =
    match metadata with
    | Some m -> System.DateTimeOffset.Parse(m.date)
    | None -> System.DateTimeOffset.UtcNow

Target.create "CreateIndex" (fun _ ->
    let githubUrl = Environment.environVarOrDefault "GitHubUrl" "https://raw.githubusercontent.com/TimLariviere/blog/master"

    let posts =
        [ for mdFile in !! "**/*.md" do
            let relativeFile = mdFile.Substring(mdFile.IndexOf("/blog/") + 6)
            let metadata = tryReadMetadata mdFile

            yield { Name = metadata |> getTitle
                    Url = sprintf "%s/%s" githubUrl relativeFile
                    CreatedAt = metadata |> getCreationDate } ]
    let blogIndex =
        { Posts = posts
          CreatedAt = System.DateTimeOffset.Now }
    
    JsonConvert.SerializeObject blogIndex
    |> (fun json -> File.WriteAllText("output/index.json", json))
)

Target.runOrDefault "CreateIndex"
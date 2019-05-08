namespace Blog.Server

open Microsoft.AspNetCore.Hosting
open Bolero.Remoting.Server
open System.IO
open Bolero
open FSharpx.Control

module RemoteServices =
    let private tryReadFileAsync path = 
        match File.Exists path with
        | false -> async.Return None
        | true ->
            File.ReadAllTextAsync path
            |> Async.AwaitTask
            |> Async.map (Json.Deserialize >> Some)

    type PostService(env: IHostingEnvironment) =
        inherit RemoteHandler<Blog.Client.RemoteServices.PostService>()

        let postListings =
            Path.Combine(env.ContentRootPath, "data/post-listings.json")
            |> tryReadFileAsync
            |> Async.map Option.get

        override this.Handler =
            { 
                getPostListings = fun () -> async {
                    return! postListings
                }
                tryGetPost = fun id -> async {
                    return!
                        Path.Combine(env.ContentRootPath, (sprintf "data/%s.json" id))
                        |> tryReadFileAsync
                }
            }
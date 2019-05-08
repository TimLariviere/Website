namespace Blog.Server

open Microsoft.AspNetCore.Hosting
open Bolero.Remoting.Server
open System.IO
open Bolero
open Blog.Client.Models
open FSharpx.Control

module RemoteServices =
    type PostService(env: IHostingEnvironment) =
        inherit RemoteHandler<Blog.Client.RemoteServices.PostService>()

        let posts =
            Path.Combine(env.ContentRootPath, "data/posts.json")
            |> (File.ReadAllTextAsync >> Async.AwaitTask)
            |> Async.map Json.Deserialize<Post[]>

        override this.Handler =
            { 
                getPosts = fun () -> async {
                    return! posts
                }
            }
namespace Blog.Client

open Blog.Client.Models
open Bolero.Remoting

module RemoteServices =
    type PostService =
        {
            /// Get the list of all posts
            getPosts: unit -> Async<Post[]>
        }
        interface IRemoteService with
            member __.BasePath = "/posts"
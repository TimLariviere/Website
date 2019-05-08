namespace Blog.Client

open Blog.Client.Models
open Bolero.Remoting

module RemoteServices =
    type PostService =
        {
            /// Get the list of all posts
            getPostListings: unit -> Async<PostListing[]>

            /// Get the post with the corresponding id
            tryGetPost: PostId -> Async<Post option> 
        }
        interface IRemoteService with
            member __.BasePath = "/posts"
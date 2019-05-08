module Blog.Client.Main

open Bolero
open Bolero.Html
open Bolero.Remoting
open Bolero.Templating.Client
open Elmish
open Blog.Client.Models
open Blog.Client.RemoteServices

type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/blog">] BlogHome
    | [<EndPoint "/blog/{id}">] BlogPost of id: string

type Model =
    { Page: Page
      PostListings: PostListing array option
      Post: Post option option
      ErrorMessage: string option }

type Message =
    | SetPage of Page
    | PageLoaded
    | GotPostListings of PostListing array
    | GotPost of Post option
    | Error of exn

let router = Router.infer SetPage (fun m -> m.Page)

let initModel =
    { Page = Home
      PostListings = None
      Post = None
      ErrorMessage = None }

let update postService message model =
    match message with
    | SetPage page ->
        let cmd =
            match page with
            | BlogPost postId -> Cmd.ofAsync postService.tryGetPost postId GotPost Error
            | _ -> Cmd.none
        { model with Page = page }, cmd
    | PageLoaded -> model, (Cmd.ofAsync postService.getPostListings () GotPostListings Error)
    | GotPostListings posts -> { model with PostListings = Some posts }, Cmd.none
    | GotPost post -> { model with Post = Some post }, Cmd.none
    | Error exn -> { model with ErrorMessage = Some exn.Message }, Cmd.none

let mainView model dispatch =
    div [] [
        yield text "Home"
        yield br []
        yield
            match model.ErrorMessage, model.PostListings with
            | None, None -> a [ router.HRef (BlogPost "loading")] [ text "Loading posts..." ]
            | Some errorMessage, _ -> text errorMessage
            | None, Some posts ->
                ul [] [
                    for post in posts do
                        yield li [] [
                            a [ router.HRef (BlogPost post.Id) ] [ text post.Title ]
                        ]
                ]
    ]

let postView model dispatch =
    match model.Post with
    | None ->
        div [] [
            text "Loading"
        ]
    | Some None ->
        div [] [
            text "Post not found"
        ]  
    | Some (Some post) ->    
        div [] [
            text post.Title
            br []
            text post.Content
            br []
            a [ router.HRef Home ] [ text "Go to home" ]
        ]

let view model dispatch =
    match model.Page with
    | Home | BlogHome -> mainView model dispatch
    | BlogPost _ -> postView model dispatch

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =
        let postService = this.Remote<PostService>()
        let update = update postService

        Program.mkProgram (fun _ -> initModel, Cmd.ofMsg PageLoaded) update view
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReloading
#endif    
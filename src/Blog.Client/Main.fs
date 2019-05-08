// module Blog.Client.Main

// open Elmish
// open Bolero
// open Bolero.Html
// open Bolero.Templating.Client
// open Bolero.Remoting
// open Blog.Client.RemoteServices
// open Blog.Client.Models
// open Blog.Client.Routing

// type Model =
//     { Page: Page
//       Posts: Post array option
//       ErrorMessage: string option }

// type Message =
//     | SetPage of Page
//     | PageLoaded
//     | GotPosts of Post array
//     | Error of exn

// let router = Router.infer SetPage (fun m -> m.Page)

// let initModel =
//     { Page = Home
//       Posts = None
//       ErrorMessage = None }

// let update postService message model =
//     match message with
//     | SetPage page -> { model with Page = page }, Cmd.none
//     | PageLoaded -> model, (Cmd.ofAsync postService.getPosts () GotPosts Error)
//     | GotPosts posts -> { model with Posts = Some posts }, Cmd.none
//     | Error exn -> { model with ErrorMessage = Some exn.Message }, Cmd.none

// let mainView model dispatch =
//     div [] [
//         match model.ErrorMessage, model.Posts with
//         | Some errorMessage, _ ->
//             yield text errorMessage
//         | None, None ->
//             yield text "Loading..."
//         | None, Some posts ->
//             yield ul [] [
//                 for post in posts do
//                     yield li [] [
//                         div [] [
//                             a [ router.HRef (BlogPost post.Id) ] [
//                                text post.Title
//                             ]
//                         ]
//                     ]
//             ]
//     ]

// let blogPostView blogPostId model dispatch =
//     div [] [
//         text blogPostId
//     ]

// let view model dispatch =
//     match model.Page with
//     | Home | BlogHome -> mainView model dispatch
//     | BlogPost blogPostId -> printfn "Should display BlogPost %s" blogPostId; blogPostView blogPostId model dispatch

// type MyApp() =
//     inherit ProgramComponent<Model, Message>()

//     override this.Program =
//         let postService = this.Remote<PostService>()
//         let update = update postService

//         Program.mkProgram (fun _ -> initModel, Cmd.ofMsg PageLoaded) update view
//         |> Program.withRouter router
// #if DEBUG
//         |> Program.withHotReloading
// #endif

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
      Posts: Post array option
      ErrorMessage: string option }

type Message =
    | SetPage of Page
    | PageLoaded
    | GotPosts of Post array
    | Error of exn

let router = Router.infer SetPage (fun m -> m.Page)

let initModel =
    { Page = Home
      Posts = None
      ErrorMessage = None }

let update postService message model =
    match message with
    | SetPage page -> { model with Page = page }, Cmd.none
    | PageLoaded -> model, (Cmd.ofAsync postService.getPosts () GotPosts Error)
    | GotPosts posts -> { model with Posts = Some posts }, Cmd.none
    | Error exn -> { model with ErrorMessage = Some exn.Message }, Cmd.none

let mainView model dispatch =
    div [] [
        yield text "Home"
        yield br []
        yield
            match model.ErrorMessage, model.Posts with
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

let postView postId model dispatch =
    div [] [
        text postId
        br []
        a [ router.HRef Home ] [ text "Go to home" ]
    ]

let view model dispatch =
    match model.Page with
    | Home | BlogHome -> mainView model dispatch
    | BlogPost postId -> postView postId model dispatch

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
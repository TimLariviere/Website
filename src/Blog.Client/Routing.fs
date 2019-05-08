namespace Blog.Client

open Bolero

module Routing =
    type Page =
        | [<EndPoint "/">] Home
        | [<EndPoint "/blog">] BlogHome
        | [<EndPoint "/blog/{id}">] BlogPost of id: string
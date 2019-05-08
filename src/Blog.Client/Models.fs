namespace Blog.Client

open System

module Models =
    type PostId = string

    type PostListing =
      { Id: PostId
        Title: string
        LegacyLink: string option }

    type Post =
      { Id: PostId
        CreationDate: DateTimeOffset
        ModificationDate: DateTimeOffset
        Title: string
        Content: string
        Category: string
        Tags: string array }
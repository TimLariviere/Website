namespace Blog.Client

module Models =
    type Post =
        { Id: string
          Title: string
          LegacyLink: string option }
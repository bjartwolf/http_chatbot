module Counter

open Terminal.Gui.Elmish
open System.Net
open Terminal.Gui
open ConnectionController
open System

// The model with the server object, not very immutable is it... 
type Model = {
    Connections: IPEndPoint list 
    Server: MailboxProcessor<ConnectionController.ConnectionMsg>
}

type Msg =
    | ConnectionEstablished of IPEndPoint 
    | Tick 

module Commands =
    let listenForConnection =
        fun dispatch ->
            async {
                do! Async.Sleep 20
                dispatch Tick 
            }
            |> Async.StartImmediate
        |> Cmd.ofSub

let init () : Model * Cmd<Msg> =
    let model = {
        Connections = [] 
        Server = Server.server 
    }
    model, Commands.listenForConnection 

let update (msg:Msg) (model:Model) =
    let reply = model.Server.PostAndReply(fun channel -> (GetNewConnection channel))
    match reply with 
        | Some (client, iPEndPoint) -> { model with Connections = iPEndPoint :: model.Connections } , Commands.listenForConnection 
        | None -> model, Commands.listenForConnection 

let view (model:Model) (dispatch:Msg->unit) =
    View.page [
        page.menuBar [
        ]
        prop.children [
            View.label [
                prop.position.x.center
                prop.position.y.at 1
                prop.textAlignment.left
                prop.color (Color.BrightYellow, Color.Green)
                label.text $"hei {model.Connections.Length}" 
            ] 
        ] 
    ]

Program.mkProgram init update view  
|> Program.run


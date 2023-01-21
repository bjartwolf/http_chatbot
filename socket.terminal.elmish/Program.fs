module Counter

open Terminal.Gui.Elmish
open System.Net
open Terminal.Gui
open ConnectionController
open System

let server = Server.server 
// The model with the server object, not very immutable is it... 
type Model = {
    Connections: IPEndPoint list 
//    Server: MailboxProcessor<ConnectionController.ConnectionMsg>
}

type Msg =
    | ConnectionEstablished of IPEndPoint 
    | Tick 

module Commands =
    let listenForConnection =
        fun dispatch ->
            async {
                do! Async.Sleep 20
                let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
                match reply with 
                    | Some (client, iPEndPoint) -> dispatch (ConnectionEstablished iPEndPoint) 
                    | None -> dispatch (Tick)
            }
            |> Async.StartImmediate
        |> Cmd.ofSub

let init () : Model * Cmd<Msg> =
    let model = {
        Connections = [] 
    }
    model, Commands.listenForConnection 

let update (msg:Msg) (model:Model) =
    match msg with 
        | Tick -> model, Commands.listenForConnection
        | ConnectionEstablished conn ->  { model with Connections = conn :: model.Connections }, Commands.listenForConnection

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


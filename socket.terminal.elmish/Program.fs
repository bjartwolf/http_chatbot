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

let init () : Model * Cmd<Msg> =
    let model = {
        Connections = [] 
        Server = Server.server 
    }
    model, Cmd.none

let update (msg:Msg) (model:Model) =
    let reply = model.Server.PostAndReply(fun channel -> (GetNewConnection channel))
    match reply with 
        | Some (client, iPEndPoint) -> { model with Connections = iPEndPoint :: model.Connections } , Cmd.none
        | None -> model, Cmd.none 

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
            View.button [
                prop.position.x.center
                prop.position.y.at 4
                label.text "Refresh"
                prop.onKeyDown (fun _ -> dispatch (Tick))
            ]
        ] 
    ]

Program.mkProgram init update view  
|> Program.run


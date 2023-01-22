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
    SelectedItem: IPEndPoint option
//    Server: MailboxProcessor<ConnectionController.ConnectionMsg>
}

type Msg =
    | ConnectionEstablished of IPEndPoint 
    | ConnectionSelected of IPEndPoint 
    | Tick 

let mutable i = 0
module Commands =
    let listenForConnection =
        fun dispatch ->
            async {
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
        SelectedItem = None 
    }
    model, Commands.listenForConnection 

let update (msg:Msg) (model:Model) =
    match msg with 
        | Tick -> model, Commands.listenForConnection
        | ConnectionEstablished conn ->  { model with Connections = (List.append model.Connections [conn]) }, Cmd.none 
        | ConnectionSelected conn -> { model with SelectedItem = Some conn } , Cmd.none 

let getSelectedItem (items: IPEndPoint list) (selectedItem: IPEndPoint option): int =
    let foo = match selectedItem with 
        | None -> 0 
        | Some selected -> (items |> List.findIndex ( fun c -> c = selected))
    foo

let view (model:Model) (dispatch:Msg->unit) =
    View.page [
        View.window [
            window.title "HTTP Chatbot"
            window.children [
                View.frameView [
                    frameView.title "Connections"
                    prop.position.x.at 0
                    prop.position.y.at 0
                    prop.width.percent 20.0
                    prop.height.filled
                    frameView.children [
                        View.listView [
                            prop.position.x.at 0
                            prop.position.y.at 0
                            prop.width.filled
                            prop.height.filled
                            listView.selectedItem (getSelectedItem model.Connections model.SelectedItem)
                            listView.source (model.Connections |> List.map (fun x -> sprintf "%A:%A" x.Address x.Port))
                            listView.onSelectedItemChanged
                                ( fun c ->
                                        if (c.Item >= model.Connections.Length) then
                                            ()
                                        else if (c.Item <> 0) then 
                                                dispatch (ConnectionSelected (model.Connections.[c.Item]))
                                            else 
                                            let previousConnection = getSelectedItem model.Connections model.SelectedItem
                                            if previousConnection = c.Item then ()
                                            else dispatch (ConnectionSelected (model.Connections.[c.Item]))
                            )
                         ]
                    ]
                ]
                View.frameView [
                    let title = match model.SelectedItem with
                        | Some conn -> sprintf "%A:%A" conn.Address conn.Port
                        | None -> "No connection selected"
                    frameView.title title 
                    prop.position.x.at 20
                    prop.position.y.at 0
                    prop.width.percent 80.0
                    prop.height.filled
                    frameView.children [
                            View.textView [
                                prop.position.x.percent 0
                                prop.position.y.percent 0
                                prop.width.filled
                                prop.height.filled
                                textView.readOnly true 
                                textField.text "foo" ]

                            View.textView [
                                prop.position.x.at 0
                                prop.position.y.percent 50.0
                                prop.width.filled
                                prop.height.filled
                                textView.readOnly true 
                                textField.text "foo" ]
                            View.textField [
                                prop.position.x.at 0
                                prop.position.y.percent 90.0
                                prop.width.filled
                                prop.height.filled
                                textField.text "foo" ]
       ]
                ]
           ] 
        ]
    ]

    
let timerSubscription dispatch =
    let rec loop () =
        async {
            do! Async.Sleep 20
            dispatch Tick 
            return! loop ()
        }
    loop () |> Async.Start

// Should to the "program with subscription thing here to wire in the servercommand-thingy
Program.mkProgram init update view  
    |> Program.withSubscription (fun _ -> Cmd.ofSub timerSubscription)
    |> Program.run


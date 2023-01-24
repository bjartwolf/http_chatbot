﻿module Counter

open Terminal.Gui.Elmish
open System.Net
open ConnectionController
open TcpMailbox
open Terminal.Gui
open Views
open socket.terminal

open Messages
// The model with the server object, not very immutable is it... 
type Model = {
    Connections: Connection list 
    SelectedItem: Connection option
    SelectedConnectionSent: string
    SelectedConnectionRecieved: string
    TextToSend: string
}

let mutable i = 0
let init () : Model * Cmd<Msg> =
    let model = {
        Connections = [] 
        SelectedItem = None 
        SelectedConnectionSent = ""
        SelectedConnectionRecieved = ""
        TextToSend = ""
    }
    model, Commands.listenForConnection 

let update (msg:Msg) (model:Model) =
    match msg with 
        | Tick -> model, Commands.listenForConnection
        | Tack -> match model.SelectedItem with
            | Some activeConnection ->  model, Commands.getSentAndRecieved activeConnection
            | None -> model, Cmd.none
        | ConnectionEstablished conn ->  { model with Connections = (List.append model.Connections [conn]) }, Commands.getSentAndRecieved conn 
        | ConnectionSelected conn -> { model with SelectedItem = Some conn } , Commands.getSentAndRecieved conn 
        | ConnectionDataReceived (recievedData,sentData,_,_) -> { model with SelectedConnectionRecieved = recievedData; SelectedConnectionSent = sentData}, Cmd.none
        | ChangeTextToSend text -> {model with TextToSend = text}, Cmd.none
        | SendText -> match model.SelectedItem with
                        | None -> model, Cmd.none
                        | Some c -> {model with TextToSend = ""}, Commands.sendData c model.TextToSend

let getSelectedItem (connections: Connection list) (selectedConnection: Connection option): int =
    let foo = match selectedConnection with 
        | None -> 0 
        | Some (_,selected)-> (connections|> List.findIndex ( fun (_,c) -> c = selected))
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
                            listView.source (model.Connections |> List.map (fun (_,x) -> sprintf "%A:%A" x.Address x.Port))
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
                        | Some (_,conn) -> sprintf "%A:%A" conn.Address conn.Port 
                        | None -> "No connection selected"
                    frameView.title title 
                    prop.position.x.at 20
                    prop.position.y.at 0
                    prop.width.percent 80.0
                    prop.height.filled
                    frameView.children [
                            contentView 0 "Client" model.SelectedConnectionRecieved
                            contentView 50 "Server" model.SelectedConnectionSent
                            View.textField [
                                prop.position.x.at 0
                                prop.position.y.percent 90.0
                                prop.width.filled
                                prop.height.filled
                                textField.text model.TextToSend 
                                textField.onTextChanging (fun text -> dispatch (ChangeTextToSend text))
                            ]
                            View.button [
                                button.text "Send" 
                                prop.position.x.at 0
                                prop.position.y.percent 95.0
                                button.onClick (fun () -> dispatch SendText)
                            ]
                    ]
                ]
            ] 
        ]
    ]

    
let timerSubscription dispatch =
    let rec loop () =
        async {
            dispatch Tick 
            do! Async.Sleep 50
            return! loop ()
        }
    loop () |> Async.Start

// Should to the "program with subscription thing here to wire in the servercommand-thingy
Program.mkProgram init update view  
    |> Program.withSubscription (fun _ -> Cmd.ofSub timerSubscription)
    |> Program.run


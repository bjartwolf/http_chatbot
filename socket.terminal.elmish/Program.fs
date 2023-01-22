module Counter

open Terminal.Gui.Elmish
open System.Net
open Terminal.Gui
open ConnectionController
open System
open socket.core.TcpWrappers
open TcpMailbox

type Connection = MailboxProcessor<lineFeed>*IPEndPoint

let server = Server.server 
// The model with the server object, not very immutable is it... 
type Model = {
    Connections: Connection list 
    SelectedItem: Connection option
    SelectedConnectionSent: string
    SelectedConnectionRecieved: string
}

type Msg =
    | ConnectionEstablished of Connection 
    | ConnectionSelected of Connection 
    | ConnectionDataReceived of string*string*ConnectionStatus*ConnectionStatus
    | Tick 
    | Tack 

let mutable i = 0
module Commands =
    let listenForConnection =
        fun dispatch ->
            async {
                let reply = server.PostAndReply(fun channel -> (GetNewConnection channel))
                match reply with 
                    | Some (client, endpoint) -> dispatch (ConnectionEstablished (ListenMessages client, endpoint)) 
                    | None -> dispatch (Tick)
            }
            |> Async.StartImmediate
        |> Cmd.ofSub
    let getSentAndRecieved ((client, _): Connection) =
        fun dispatch ->
            async {
                let (sentdata,statusClient)= client.PostAndReply(fun channel -> (GetRecieved channel))
                let (recievedData,statusServer)= client.PostAndReply(fun channel -> (GetSent channel))
                dispatch (ConnectionDataReceived (sentdata, recievedData, statusClient, statusServer))
            }
            |> Async.StartImmediate
        |> Cmd.ofSub


let init () : Model * Cmd<Msg> =
    let model = {
        Connections = [] 
        SelectedItem = None 
        SelectedConnectionSent = ""
        SelectedConnectionRecieved = ""
    }
    model, Commands.listenForConnection 

let update (msg:Msg) (model:Model) =
    match msg with 
        | Tick -> model, Commands.listenForConnection
        | Tack -> match model.SelectedItem with
            | Some activeConnection ->  model, Commands.getSentAndRecieved activeConnection
            | None -> model, Cmd.none
        | ConnectionEstablished conn ->  { model with Connections = (List.append model.Connections [conn]) }, Cmd.none 
        | ConnectionSelected conn -> { model with SelectedItem = Some conn } , Commands.getSentAndRecieved conn 
        | ConnectionDataReceived (recievedData,sentData,_,_) -> { model with SelectedConnectionRecieved = recievedData; SelectedConnectionSent = sentData}, Cmd.none

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
                                textField.text model.SelectedConnectionRecieved ]
                            View.textField [
                                prop.position.x.at 0
                                prop.position.y.percent 90.0
                                prop.width.filled
                                prop.height.filled
                                textField.text model.SelectedConnectionSent ]
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
            dispatch Tack 
            return! loop ()
        }
    loop () |> Async.Start

// Should to the "program with subscription thing here to wire in the servercommand-thingy
Program.mkProgram init update view  
    |> Program.withSubscription (fun _ -> Cmd.ofSub timerSubscription)
    |> Program.run


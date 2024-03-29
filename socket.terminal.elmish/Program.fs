﻿module Counter

open Terminal.Gui.Elmish
open Views
open socket.terminal

open Messages
open Model

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
        | ConnectionEstablished conn ->  
                { model with Connections = (conn :: model.Connections ) }, 
                Commands.getSentAndRecieved conn 
        | ConnectionSelected conn -> 
                { model with SelectedItem = Some conn },
                Commands.getSentAndRecieved conn 
        | RefreshSentReceived -> 
            match model.SelectedItem with
              | Some activeConnection ->  model, 
                                          Commands.getSentAndRecieved activeConnection
              | None  when not model.Connections.IsEmpty -> { model with SelectedItem = Some model.Connections.Head },
                                                            Commands.getSentAndRecieved model.Connections.Head
              | None -> model, Cmd.none
        | ConnectionDataReceived (recievedData,sentData,_,_) -> 
                { model with SelectedConnectionRecieved = recievedData; SelectedConnectionSent = sentData},
                Cmd.none
        | ChangeTextToSend text -> 
                {model with TextToSend = text},
                Cmd.none
        | SendText -> match model.SelectedItem with
                        | None -> model,
                                  Cmd.none
                        | Some c -> {model with TextToSend = ""}, 
                                    Commands.sendData c model.TextToSend
        | ClosedCurrent -> let index = List.tryFindIndex(fun c -> c = model.SelectedItem.Value) model.Connections  
                           match index with
                            | Some i -> {model with Connections = List.removeAt i model.Connections}, Cmd.none
                            | None -> model, Cmd.none 
        | CloseCurrent -> match model.SelectedItem with
                                | None -> model, Cmd.none
                                | Some c -> model, Commands.closeCurrent c
        | Tick -> model, 
                  Commands.listenForConnection

let view (model:Model) (dispatch:Msg->unit) =
    mainView model dispatch
    
let timerSubscription dispatch =
    let rec loop () =
        async {
            do! Async.Sleep 50
            dispatch Tick 
            return! loop ()
        }
    loop () |> Async.Start

Program.mkProgram init update view  
//    |> Program.withTrace (fun f -> Con)
    |> Program.withSubscription (fun _ -> Cmd.ofSub timerSubscription)
    |> Program.run

namespace Socket.AvaloniaUi 

open ConnectionController

module Update =
    open Messages
    open Model
    open Elmish
    open socket.terminal

    let init (server: MailboxProcessor<ConnectionMsg>) (): Model * Cmd<Msg> =
        let model = {
            Connections = [] 
            SelectedItem = None 
            SelectedConnectionSent = ""
            SelectedConnectionRecieved = ""
            TextToSend = ""
        }
        model, (Commands.listenForConnection server)

    let update (server: MailboxProcessor<ConnectionMsg>) (msg: Msg) (model: Model) : Model * Cmd<Msg> =
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
            | Tick -> model, Commands.listenForConnection server
    

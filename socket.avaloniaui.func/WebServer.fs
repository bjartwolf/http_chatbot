namespace Examples.CounterApp

open Avalonia.FuncUI.DSL
open Elmish
open socket.terminal
open Avalonia.FuncUI

module Counter =
    open Avalonia.Controls
    open Avalonia.Layout
    open Messages
    open Model
    open ConnectionController
    open Commands 

    let init () : Model * Cmd<Msg> =
        let model = {
            Connections = [] 
            SelectedItem = None 
            SelectedConnectionSent = ""
            SelectedConnectionRecieved = ""
            TextToSend = ""
        }
        model, Commands.listenForConnection 

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
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
            | Tick -> model, Commands.listenForConnection
    
    let view (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch SendText)
                    Button.content "send"
                    Button.horizontalAlignment HorizontalAlignment.Stretch
                ]                
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch CloseCurrent)
                    Button.content "close"
                    Button.horizontalAlignment HorizontalAlignment.Stretch
                ]                
                ListBox.create [
                    ListBox.dock Dock.Left
                    ListBox.dataItems model.Connections
                    ListBox.itemTemplate (
                        DataTemplateView.create<_,_>(fun ((_,ip): Connection) -> 
                            TextBlock.create [
                            TextBlock.text (sprintf "%A" ip)
                            ]
                        )
                    ) 
                    ListBox.onSelectedItemChanged( fun (x) -> 
                       if x <> null then   
                           let conn = x :?> Connection
                           dispatch (ConnectionSelected conn))
                ]
                TextBox.create [
                    TextBox.background "Gray"
                    TextBox.dock Dock.Bottom
                    TextBox.horizontalAlignment HorizontalAlignment.Stretch
                    TextBox.text model.TextToSend
                    TextBox.onTextChanged (fun x -> dispatch(ChangeTextToSend x))
                ]
                TextBlock.create [
                    TextBlock.background "White"
                    TextBlock.foreground "Black"
                    TextBlock.width 400.0
                    TextBlock.dock Dock.Right
                    TextBlock.text model.SelectedConnectionSent
                ]
                TextBlock.create [
                    TextBlock.background "Gray"
                    TextBlock.width 400.0
                    TextBlock.dock Dock.Right
                    TextBlock.text model.SelectedConnectionRecieved
                ]
             ]
        ]

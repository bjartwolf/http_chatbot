namespace Socket.AvaloniaUi 

module FakeUpdate =
    open Messages
    open Model
    open Elmish
    open socket.terminal

    let init () : Model * Cmd<Msg> =
        let model = {
            Connections = [] 
            SelectedItem = None 
            SelectedConnectionSent = "Foobar\r\n Hei"
            SelectedConnectionRecieved = ""
            TextToSend = "Some text to send"
        }
        model, Commands.listenForConnection 

    let update (_: Msg) (model: Model) : Model * Cmd<Msg> =
        model, Cmd.none
    

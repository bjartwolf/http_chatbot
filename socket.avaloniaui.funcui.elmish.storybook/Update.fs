namespace Socket.AvaloniaUi 

open TcpMailbox
open System.Net

module FakeUpdate =
    open Messages
    open Model
    open Elmish
    open socket.terminal

    let connection = (MailboxProcessor<lineFeed>.Start( fun inbox ->  async {()}), new IPEndPoint(IPAddress.Parse("123.12.12.1"), 800))

    let init () : Model * Cmd<Msg> =
        let model = {
            Connections = [connection] 
            SelectedItem = None 
            SelectedConnectionSent = "Foobar\r\n Hei"
            SelectedConnectionRecieved = ""
            TextToSend = "Some text to send"
        }
        model, Commands.listenForConnection 

    let update (_: Msg) (model: Model) : Model * Cmd<Msg> =
        model, Cmd.none
    

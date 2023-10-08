namespace Socket.AvaloniaUi 

open TcpMailbox
open System.Net
open ConnectionController

module FakeUpdate =
    open Messages
    open Model
    open Elmish
    open socket.terminal


    let init () : Model * Cmd<Msg> =
        let connection = (MailboxProcessor<lineFeed>.Start( fun inbox ->  async {()}), new IPEndPoint(IPAddress.Parse("123.12.12.1"), 800))
        let server: MailboxProcessor<ConnectionMsg> = MailboxProcessor<ConnectionMsg>.Start( fun _ ->  async {()})
        let model = {
            Connections = [connection] 
            SelectedItem = None 
            SelectedConnectionSent = "Foobar\r\n Hei"
            SelectedConnectionRecieved = ""
            TextToSend = "Some text to send"
        }
        model, (Commands.listenForConnection server)

    let update (_: Msg) (model: Model) : Model * Cmd<Msg> =
        model, Cmd.none
    

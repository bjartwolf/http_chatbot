module Tests

open Xunit
open Socket.AvaloniaUi
open Messages
open Model
open Elmish
open ConnectionController

[<Fact>]
let ``My test`` () =
    let model = {
        Connections = [] 
        SelectedItem = None 
        SelectedConnectionSent = ""
        SelectedConnectionRecieved = ""
        TextToSend = ""
    }

    let initView = SocketViews.mainView model (fun (_:Msg) -> ())
    Assert.NotEmpty(initView.Attrs)


[<Fact>]
let ``My test 1`` () =
    let model = {
        Connections = [] 
        SelectedItem = None 
        SelectedConnectionSent = ""
        SelectedConnectionRecieved = ""
        TextToSend = ""
    }

    let msg = ChangeTextToSend  "HTTP/1.1 200 OK"

    let server: MailboxProcessor<ConnectionMsg> = MailboxProcessor<ConnectionMsg>.Start( fun _ ->  async {()})
    let (model', msg')= (Update.update server) msg model

    Assert.Equal("HTTP/1.1 200 OK", model'.TextToSend)
    Assert.Equal<Cmd<Msg>>(Cmd.none, msg')


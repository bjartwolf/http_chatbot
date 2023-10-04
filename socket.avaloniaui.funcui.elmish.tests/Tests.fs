module Tests

open System
open Xunit
open Socket.AvaloniaUi
open Messages
open Model

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

    let (model', msg')= Update.update msg model

    Assert.Equal("HTTP/1.1 200 OK", model'.TextToSend)


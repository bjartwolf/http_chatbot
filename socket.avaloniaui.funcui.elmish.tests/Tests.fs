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

    let initView = SocketViews.mainView model (fun (x:Msg) -> ())
    Assert.NotEmpty(initView.Attrs)

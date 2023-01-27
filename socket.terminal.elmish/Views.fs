module Views

open Terminal.Gui.Elmish
open Terminal.Gui
open Messages

let contentView (position: int) (title: string) (content: string) = 
    View.frameView [
        frameView.title title 
        prop.position.x.at 0
        prop.position.y.percent position 
        prop.height.percent 40.0
        prop.width.filled
        frameView.children [
            View.scrollView [
                prop.position.x.at 0
                prop.position.y.at 0
                prop.height.filled
                prop.width.filled
                scrollView.contentSize (Size(80,80))
                scrollView.children [
                    View.textView [
                        prop.position.x.percent 0
                        prop.position.y.percent 0
                        prop.width.filled
                        prop.height.filled
                        textView.readOnly true 
                        textField.text content
                    ]
                ]
            ]
        ]
    ]


open Model
let mainView (model:Model) (dispatch:Msg->unit) =
    let getSelectedItem (connections: Connection list) (selectedConnection: Connection option): int =
        let foo = match selectedConnection with 
            | None -> 0 
            | Some (_,selected)-> (connections|> List.findIndex ( fun (_,c) -> c = selected))
        foo

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
//                            listView.selectedItem (getSelectedItem model.Connections model.SelectedItem)
                            listView.source (model.Connections |> List.map (fun (_,x) -> sprintf "%A:%A" x.Address x.Port))
                            listView.onSelectedItemChanged
                                ( fun c ->
                                        if (c.Item >= model.Connections.Length) then
                                            ()
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
                            contentView 0 "Client" model.SelectedConnectionRecieved
                            contentView 50 "Server" model.SelectedConnectionSent
                            View.textField [
                                prop.position.x.at 0
                                prop.position.y.percent 90.0
                                prop.width.filled
                                prop.height.filled
                                prop.onKeyDown (fun (x) -> 
                                    if x.KeyEvent.Key = Key.Enter then
                                        dispatch SendText
                                        x.Handled <- true
                                    else 
                                        ())
                                textField.text model.TextToSend 
                                textField.onTextChanging (fun text -> dispatch (ChangeTextToSend text))
                            ]
                            View.button [
                                button.text "Send" 
                                prop.position.x.at 0
                                prop.position.y.percent 95.0
                                button.onClick (fun () -> dispatch SendText)
                            ]
                    ]
                ]
            ] 
        ]
   ]
 

module Views

open Terminal.Gui.Elmish
open Terminal.Gui
open Messages

let contentView (position: int) (title: string) (content: string) = 
    let lines = content.Split(System.Environment.NewLine);
    let length = lines.Length;
    let width = lines |> Array.map (fun x -> x.Length) |> Array.max

    View.frameView [
            frameView.title title 
            prop.position.x.at 0
            prop.position.y.percent position 
            prop.height.percent 40.0
            prop.width.filled
            frameView.children [
               View.textView [
                    prop.position.x.at 0
                    prop.position.y.at 0
                    prop.width.sized width
                    prop.height.sized length
                    textView.readOnly true 
                    textField.text content
                    prop.color (Color.White, Color.Gray)
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
                            contentView 0 "Request" model.SelectedConnectionRecieved
                            contentView 50 "Response" model.SelectedConnectionSent
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
                                        x.Handled <- true
                                        ())
                                textField.text model.TextToSend 
                                textField.onTextChanging (fun text -> dispatch (ChangeTextToSend text))
                            ]
                            View.button [
                                button.text "Close" 
                                prop.position.x.at 0 
                                prop.position.y.percent 95
                                button.onClick (fun () -> dispatch CloseCurrent)
                            ]
                     ]
                ]
            ] 
        ]
   ]
 

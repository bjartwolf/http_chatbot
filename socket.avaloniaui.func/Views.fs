namespace Socket.AvaloniaUi 

module SocketViews = 
    open Avalonia.Controls
    open Avalonia.Layout
    open Messages
    open Model
    open Avalonia.FuncUI.DSL
    open ConnectionView

    let mainView (model: Model) (dispatch) =
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
                connectionView model.Connections dispatch
                TextBox.create [
                    TextBox.background "Gray"
                    TextBox.dock Dock.Bottom
                    TextBox.horizontalAlignment HorizontalAlignment.Stretch
                    TextBox.text model.TextToSend
                    TextBox.onTextChanged (fun x -> dispatch(ChangeTextToSend x))
                    TextBox.onKeyDown( fun x -> 
                        if x.Key = Avalonia.Input.Key.Enter then 
                            x.Handled <- true
                            dispatch SendText)
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

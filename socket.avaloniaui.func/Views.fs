namespace Socket.AvaloniaUi 

open Avalonia.Input

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
                StackPanel.create [
                    StackPanel.dock Dock.Bottom
                    StackPanel.children [
                        TextBox.create [
                            TextBox.background "Gray"
                            TextBox.horizontalAlignment HorizontalAlignment.Stretch
                            TextBox.text model.TextToSend
                            TextBox.onTextChanged (fun x -> dispatch(ChangeTextToSend x))
                            TextBox.onKeyDown( fun x -> 
                                if x.Key = Avalonia.Input.Key.Enter then 
                                    x.Handled <- true
                                    dispatch SendText)
                        ]
                        Button.create [
                            Button.onClick (fun _ -> dispatch CloseCurrent)
                            Button.content "Close connection to current socket (escape)"
                            Button.horizontalAlignment HorizontalAlignment.Stretch
                            Button.hotKey (new KeyGesture (Key.Escape, KeyModifiers.None))
                        ]                
                        Button.create [
                            Button.dock Dock.Bottom
                            Button.onClick (fun _ -> dispatch SendText)
                            Button.content "Send text to selected socket (Enter)"
                            Button.horizontalAlignment HorizontalAlignment.Stretch
                        ]                

                    ]                
                ]                
                connectionView model.Connections dispatch

                ScrollViewer.create [
                    ScrollViewer.content (
                        TextBlock.create [
                            TextBlock.background "White"
                            TextBlock.foreground "Black"
                            TextBlock.width 400.0
                            TextBlock.dock Dock.Right
                            TextBlock.text model.SelectedConnectionSent
                        ]
                    )
                ]
                ScrollViewer.create [
                    ScrollViewer.content (
                         TextBlock.create [
                            TextBlock.background "Gray"
                            TextBlock.width 400.0
                            TextBlock.dock Dock.Right
                            TextBlock.text model.SelectedConnectionRecieved
                        ]
                    )
                ]
             ]
        ]

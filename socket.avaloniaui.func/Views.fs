namespace Socket.AvaloniaUi 

module SocketViews = 
    open Avalonia.Controls
    open Model
    open Avalonia.FuncUI.DSL

    let mainView (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                ControlsView.view model dispatch
                ConnectionView.view model.Connections dispatch

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

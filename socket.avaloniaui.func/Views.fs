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

                Grid.create [
                    Grid.rowDefinitions "50*,50*" 
                    Grid.children[
                        ScrollViewer.create [
                            Grid.row 0
                            ScrollViewer.content (
                                 TextBlock.create [
                                    TextBlock.background "Gray"
                                    TextBlock.text model.SelectedConnectionRecieved
                                ]
                            )
                        ]

                        ScrollViewer.create [
                            Grid.row 1
                            ScrollViewer.content (
                                TextBlock.create [
                                    TextBlock.background "Blue"
                                    TextBlock.text model.SelectedConnectionSent
                                ]
                            )
                        ]
                    ]
                ]
             ]
        ]

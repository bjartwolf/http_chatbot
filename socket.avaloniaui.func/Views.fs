namespace Socket.AvaloniaUi 

open Avalonia.Controls.Primitives
open Avalonia.Media

module SocketViews = 
    open Avalonia.Controls
    open Model
    open Avalonia.FuncUI.DSL

    // https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Themes.Fluent/Accents/FluentControlResources.xaml
    let mainView (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                ControlsView.view model dispatch
                ConnectionView.view model.Connections dispatch

                Grid.create [
                    Grid.rowDefinitions "50*,1*,50*" 
                    Grid.children[
                        Border.create [
                            Border.borderThickness 1
                            Border.borderBrush Brushes.Gray
                            Border.margin 2
                            Border.child (
                                ScrollViewer.create [
                                    //ScrollViewer.borderBrush Brushes.Black
                                    //ScrollViewer.borderThickness 2
                                    ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Visible
                                    ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Visible
                                    Grid.row 0
                                    ScrollViewer.content (
                                         TextBlock.create [
    //                                        TextBlock.background "Gray"
                                            TextBlock.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                                            TextBlock.verticalScrollBarVisibility ScrollBarVisibility.Disabled
                                            TextBlock.text model.SelectedConnectionRecieved
                                        ]
                                    )
                                ]
                             )
                        ]
                        GridSplitter.create [
                            Grid.row 1
                        ]
                        ScrollViewer.create [
                            ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Visible
                            ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Visible
                            Grid.row 2
                            ScrollViewer.content (
                                TextBlock.create [
                                    TextBlock.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                                    TextBlock.verticalScrollBarVisibility ScrollBarVisibility.Disabled
                                    TextBlock.text model.SelectedConnectionSent
                                ]
                            )
                        ]
                    ]
                ]
             ]
        ]

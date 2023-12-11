module RequestReplyView

open Avalonia.FuncUI.DSL
open Avalonia.Controls
open Avalonia.Media
open Avalonia.Controls.Primitives

let view (selectedContextRecieved: string) (selectedContentSent: string) =
    Grid.create [
        Grid.margin (10,0,0,0)
        Grid.rowDefinitions "25,50*,1*,50*" 
        Grid.children[
            TextBlock.create [
                Grid.row 0
                TextBlock.text "Request: " 
                TextBlock.fontWeight FontWeight.DemiBold
            ]

            Border.create [
                Grid.row 1
                Border.borderThickness 1
                Border.borderBrush Brushes.LightGray
                Border.margin 2
                Border.child (
                    ScrollViewer.create [
                        ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Visible
                        ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Visible
                        Grid.row 0
                        ScrollViewer.content (
                             TextBlock.create [
                                TextBlock.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                                TextBlock.verticalScrollBarVisibility ScrollBarVisibility.Disabled
                                TextBlock.text selectedContextRecieved 
                            ]
                        )
                    ]
                 )
            ]
            GridSplitter.create [
                Grid.row 2
            ]

            Grid.create [
                Grid.row 3
                Grid.rowDefinitions "25,50*" 
                Grid.children [
                     TextBlock.create [
                        Grid.row 0
                        TextBlock.text "Response: " 
                        TextBlock.fontWeight FontWeight.DemiBold
                     ]

                     Border.create [
                        Grid.row 1
                        Border.borderThickness 1
                        Border.borderBrush Brushes.LightGray
                        Border.margin 2
                        Border.child (
                            ScrollViewer.create [
                                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Visible
                                ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Visible
                                ScrollViewer.content (

                                    TextBlock.create [
                                        TextBlock.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                                        TextBlock.verticalScrollBarVisibility ScrollBarVisibility.Disabled
                                        TextBlock.text selectedContentSent 
                                    ]
                                )
                           ]
                        )
                    ]
                
                ]
            ]
       ]
    ]

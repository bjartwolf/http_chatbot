module RequestReplyView

open Avalonia.FuncUI.DSL
open Avalonia.Controls
open Avalonia.Media
open Avalonia.Controls.Primitives

let view (selectedContextRecieved: string) (selectedContentSent: string) =
    Grid.create [
        Grid.rowDefinitions "50*,1*,50*" 
        Grid.children[
            Border.create [
                Border.borderThickness 1
                Border.borderBrush Brushes.Gray
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
                        TextBlock.text selectedContentSent 
                    ]
                )
            ]
        ]
    ]

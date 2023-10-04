namespace Socket.AvaloniaUi 

module DesignSystem = 
    open Avalonia.Controls
    open Model
    open Avalonia.FuncUI.DSL

    // https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Themes.Fluent/Accents/FluentControlResources.xaml
    let mainView (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                RequestReplyView.view model.SelectedConnectionRecieved model.SelectedConnectionSent 
             ]
        ]

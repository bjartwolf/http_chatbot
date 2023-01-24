module Views

open Terminal.Gui.Elmish
open Terminal.Gui

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




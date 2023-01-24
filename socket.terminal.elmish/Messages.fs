module Messages

open TcpMailbox
open System.Net

type Connection = MailboxProcessor<lineFeed>*IPEndPoint
type Msg =
    | ConnectionEstablished of Connection 
    | ConnectionSelected of Connection 
    | ConnectionDataReceived of string*string*ConnectionStatus*ConnectionStatus
    | Tick 
    | SendText 
    | RefreshSentReceived 
    | ChangeTextToSend of string 




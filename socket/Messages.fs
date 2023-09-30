module Messages

open TcpMailbox
open Model

type Msg =
    | ConnectionEstablished of Connection 
    | ConnectionSelected of Connection 
    | ConnectionDataReceived of string*string*ConnectionStatus*ConnectionStatus
    | Tick 
    | SendText 
    | CloseCurrent 
    | ClosedCurrent 
    | RefreshSentReceived 
    | ChangeTextToSend of string 

```
cd socket.avaloniaui.func
dotnet run --insecure --port 13001 --ip 127.0.0.1 
```

Currently it is hardcoded to http://localhost:13001, try to point a browser to it and 
reply with 
```
HTTP/1.1 200 OK
Content-Type: text/hml

<html>
<h1>Hello</h1>
<img src="http://placekitten.com/200/200" />
```

# Roadmap
- Multi-line edit, need to fix upstream https://github.com/DieselMeister/Terminal.Gui.Elmish/issues/23
  Also need to think of how to send, cannot use Enter in multi-line edit
- Nicer scrollbars, need to fix upstream https://github.com/DieselMeister/Terminal.Gui.Elmish/issues/22
- Show states of closed connections
- TSL

remember to use nc localhost 13000 -q 0 to get the fin package sent

# TSL

Downloaded [certbot](https://certbot.eff.org/).
Run PowerShell as admin.
Fill in desired domain name, and then create a TXT record as described in your DNS and then you have the certificates in the required format.

```
certbot certonly --manual --preferred-challenges dns
```
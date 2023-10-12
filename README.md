# Running in HTTP mode
To run without a certificate

```
cd socket.avaloniaui.func
dotnet run --insecure --port 8080 --ip 127.0.0.1
```

Open a browser on ```http://127.0.0.1:8080``` or try ```curl http://localhost:8080``` 
Try replying with
```
HTTP/1.1 200 OK
Content-Type: text/hml

<html>
<h1>Hello</h1>
<img src="http://placekitten.com/200/200" />
</html>
```


# HTTPS/TLS

Download [certbot](https://certbot.eff.org/).
Run PowerShell as admin.

```
certbot certonly --manual --preferred-challenges dns
```

Fill in desired domain name, and then create a TXT record as described in your DNS and then you have the certificates in the required format.
Point to the certificate by updating the App.config or 

```
dotnet run --ip 127.0.0.1 --port 443 --certpemfilepath c:\Certbot\live\artisanal.bjartnes.dev\fullchain.pem --keypemfilepath c:\Certbot\live\artisanal.bjartnes.dev\privkey.pem
```

You need to add the IP to the host file or add it to DNS to get the domainname to match.

# Roadmap for the terminal.UI part (put on hold)
- Multi-line edit, need to fix upstream https://github.com/DieselMeister/Terminal.Gui.Elmish/issues/23
  Also need to think of how to send, cannot use Enter in multi-line edit
- Nicer scrollbars, need to fix upstream https://github.com/DieselMeister/Terminal.Gui.Elmish/issues/22
- Show states of closed connections


remember to use nc localhost 13000 -q 0 to get the fin package sent if playing with netcat


When running this on Ubuntu in Azure, I found it hard to run the UI as sudo to bind to 443, so I listen to 13005 and iptabled it with:
```
sudo iptables -t nat -I PREROUTING -p tcp --dport 443 -j REDIRECT --to-ports 13005
```


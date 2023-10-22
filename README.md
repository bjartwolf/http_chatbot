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
Content-Type: text/html

<html>
<h1>Hello</h1>
<img src="http://placekitten.com/200/200" />
</html>
```


# HTTPS/TLS

## Using dotnet dev cert

Using dotnet dev-certs is a simple and quick way on Windows to get started with a certificate that can be used with localhost and that is trusted.
It might not work as straight forward on other OSes.

```powershell
cd socket.avaloniaui.func
dotnet dev-certs https --export-path devcert.pem --no-password --format PEM --trust
dotnet run --ip 127.0.0.1 --port 14011 --certpemfilepath (Get-Item 'devcert.pem').FullName --keypemfilepath (Get-Item 'devcert.key').FullName

```

This will produce a ```devcert.pem``` and a ```devcert.key``` file that you can rever to (with full pathname) from the App.config or using command line parameters.
Listening to 127.0.0.1 should then allow you to connect securely on https://localhost:14011

On linux it might be easier to just create the certificate yourself with OpenSSL than battle with the devcerts, I am honestly not sure what is the least amount of hassle and it likely
depends on your distribution. 

## Using cert bot
If you want to use proper certificates to allow for being a proper webserver, the cheapest way is to use Let's Encrypt. Instructions are given here.
This does require you to own a domain to properly verify the name. I am using DNS verification so that is what I have documented,
but you need to be able to verify that you own the domain somehow. Details are given in the Let's Encrypt/certbot documentation.


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


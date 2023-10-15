# PCAP-Reader
PCAP reader using Tshark Shared Library.

#### Tshark Shared Library Source: 
```
https://github.com/AkibHossainOmi/Tshark-shared-Library.git
```
# Dependencies
* Wireshark Source Code
* Tshark Shared Library

# Install
- Ubuntu
```
git clone https://github.com/AkibHossainOmi/Tshark-shared-Library.git
./Using-Tshark-Shared-Library/Tshark-shared-Library/tshark_share.sh
dotnet add package Newtonsoft.Json
dotnet run -- tshark -Tjson -r your-packet.pcap
```

# FileTransfer (Phase 1)
This repo contains my 4 initial attempts in writing a client and server progams that can transfer an image and its metadata (filename and size only as string). The end goal is to learn network programming. The project has 4 versioned implementations, the first version is my very first atempt and it doesnt work at all. 

In this phase, I have achieved my file transfer goal using ```System.Net.Sockets.TCPClient``` and ```System.Net.Sockets.TCPListener``` directly without using HTTP. I learnt the difference between ```StreamWriter/StreamReader``` and ```BinaryWrtier/BinaryReader```. The former is specifically optimized to handle strings and strings only (AFAIK) while the latter can handle any binary data including images and strings. 

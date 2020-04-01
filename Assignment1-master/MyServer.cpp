#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <vector>
#include <queue>
#include <thread>

#pragma comment(lib, "Ws2_32.lib")

typedef struct UserData {
	std::string IP = "";
	std::string UserName = "";
	sockaddr_in Address;
};

typedef struct InboundMessagePacket {
	std::string SenderIP = "";
	std::string Message = "";
	sockaddr_in Address;
};

typedef struct OutboundMessagePacket {
	std::string Message = "";
	std::string ToWho = "";
	std::vector<sockaddr_in> AddressesToSendTo;
	std::string FromWho = "";
};

int NumberOfLobbies = 0;
typedef struct Lobby {
	std::vector<UserData> ConnectedUsers;
	std::vector<std::string> ChatLog;
	int LobbyID;
};

std::vector<Lobby> ActiveLobbies;
std::vector<UserData> ConnectedUsers;
std::queue<OutboundMessagePacket> MessagesToSend;
std::queue<InboundMessagePacket> MessagesReceived;

bool ServerRunning = true;


SOCKET server_socket;
const unsigned int BUF_LEN = 1024;

sockaddr_in fromAddr;
int fromlen = sizeof(fromAddr);
char recv_buf[BUF_LEN];

void SendToAll() {

	if (MessagesToSend.size() > 0) {
		OutboundMessagePacket MessagePacket = MessagesToSend.front();
		std::string Message = MessagePacket.Message;

		char* message = (char*)Message.c_str();

		for (int i = 0; i < ConnectedUsers.size(); i++) {
			sockaddr_in tempaddr = ConnectedUsers[i].Address;

			if (sendto(server_socket, message, BUF_LEN, 0,
				(struct sockaddr*) & tempaddr, sizeof(tempaddr)) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}
			std::cout << "Sent data to: " << ConnectedUsers[i].UserName << std::endl;
		}

		MessagesToSend.pop();
	}
}

void SendToSpecific() {

	if (MessagesToSend.size() > 0) {
		OutboundMessagePacket MessagePacket = MessagesToSend.front();
		std::string Message = MessagePacket.Message;

		char* message = (char*)Message.c_str();

		for (int i = 0; i < MessagePacket.AddressesToSendTo.size(); i++) {
			sockaddr_in tempaddr = MessagePacket.AddressesToSendTo[i];

			if (sendto(server_socket, message, BUF_LEN, 0,
				(struct sockaddr*) & tempaddr, sizeof(tempaddr)) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}

			std::cout << "Sent data to client: " << MessagePacket.FromWho << std::endl;
		}

		MessagesToSend.pop();
	}
}

void SendProcess() {
	while (ServerRunning == true) {
		if (MessagesToSend.size() > 0) {
			OutboundMessagePacket CurrentPacket = MessagesToSend.front();
			if (CurrentPacket.ToWho == "ALL") {
				SendToAll();
			}
			else if (CurrentPacket.ToWho == "SPECIFIC") {
				SendToSpecific();
			}
		}
	}
}
void RecvProcess() {
	while (ServerRunning == true) {
		memset(recv_buf, 0, BUF_LEN);
		if (recvfrom(server_socket, recv_buf, sizeof(recv_buf), 0, (struct sockaddr*) & fromAddr, &fromlen) == SOCKET_ERROR) {
			printf("recvfrom() failed...%d\n", WSAGetLastError());
		}

		char ipbuf[INET_ADDRSTRLEN];
		inet_ntop(AF_INET, &fromAddr, ipbuf, sizeof(ipbuf));

		std::string SendersIP = ipbuf;

		InboundMessagePacket TempPacket;
		TempPacket.SenderIP = SendersIP;
		TempPacket.Message = recv_buf;
		TempPacket.Address = fromAddr;
		MessagesReceived.push(TempPacket);
	}
}

void ProcessMsg() {
	if (MessagesReceived.size() > 0) 
	{
		InboundMessagePacket Current = MessagesReceived.front();
		std::string RecMsg = Current.Message;
		std::string SendersIP = Current.SenderIP;

		if (RecMsg.substr(0, 4) == "JOIN") 
		{
			
			OutboundMessagePacket JoinMsg;
			JoinMsg.ToWho = "SPECIFIC";
			UserData Player;
			Player.IP = SendersIP;
			Player.Address = fromAddr;
			if (ConnectedUsers.size() <= 0) 
			{
				Player.UserName = "Player1";
				ConnectedUsers.push_back(Player);
				JoinMsg.AddressesToSendTo.push_back(Player.Address);
			}
			else if (ConnectedUsers.size() > 0)
			{
				int Similarities = 0;
				for (int i = 0; i < ConnectedUsers.size(); i++) 
				{
					if (SendersIP == ConnectedUsers[i].IP) 
					{
						Similarities++;
					}
				}

				if (Similarities == 0) 
				{
					Player.UserName = "Player2";
					ConnectedUsers.push_back(Player);
					JoinMsg.AddressesToSendTo.push_back(Player.Address);
				}
			}

			if (JoinMsg.AddressesToSendTo.size() > 0) 
			{
				JoinMsg.Message = "CPP";
				JoinMsg.Message += "\n";
				JoinMsg.Message += "ACCEPT";
				JoinMsg.Message += "\n";
				JoinMsg.Message += Player.UserName;

				MessagesToSend.push(JoinMsg);
				std::cout << "a player joined the game" << std::endl;
			}

		}
		else if (RecMsg.substr(0, 4) == "UPDP1" || RecMsg.substr(0, 4) == "UPDP2") 
		{
			OutboundMessagePacket Temppckt;
			Temppckt.ToWho = "SPECIFIC";
			Temppckt.Message = "CS";
			Temppckt.Message += "\n";
			Temppckt.Message += RecMsg;

			for (int i = 0; i < ConnectedUsers.size(); i++) 
			{
				if (ConnectedUsers[i].IP != SendersIP) 
				{
					Temppckt.AddressesToSendTo.push_back(ConnectedUsers[i].Address);
				}

				if (ConnectedUsers[i].IP == SendersIP) 
				{
					Temppckt.FromWho = ConnectedUsers[i].UserName;
				}
			}
			MessagesToSend.push(Temppckt);
		}
		MessagesReceived.pop();

	}
}

void ServerProc() 
{
	while (ServerRunning == true) 
	{
		ProcessMsg();
	}
}

int main() 
{
	//Initialize winsock
	WSADATA wsa;

	int error;
	error = WSAStartup(MAKEWORD(2, 2), &wsa);

	if (error != 0) 
	{
		printf("Failed to initialize %d\n", error);
		return 1;
	}

	//Create a Server socket

	struct addrinfo* ptr = NULL, hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_DGRAM;
	hints.ai_protocol = IPPROTO_UDP;
	hints.ai_flags = AI_PASSIVE;

	if (getaddrinfo("localhost"/*NULL*/, "8888", &hints, &ptr) != 0) 
	{
		printf("Getaddrinfo failed!! %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	server_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	if (server_socket == INVALID_SOCKET)
	{
		printf("Failed creating a socket %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	// Bind socket
	if (bind(server_socket, ptr->ai_addr, (int)ptr->ai_addrlen) == SOCKET_ERROR) 
	{
		printf("Bind failed: %d\n", WSAGetLastError());
		closesocket(server_socket);
		freeaddrinfo(ptr);
		WSACleanup();
		return 1;
	}

	// Receive msg from client
	std::thread ServerProcess{ ServerProc };
	std::thread SendProc{ SendProcess };
	std::thread RecProc{ RecvProcess };

	ServerProcess.join();
	SendProc.join();
	RecProc.join();

	// Struct that will hold the IP address of the client that sent the message (we don't have accept() anymore to learn the address)
	closesocket(server_socket);
	freeaddrinfo(ptr);
	WSACleanup();

	return 0;
}
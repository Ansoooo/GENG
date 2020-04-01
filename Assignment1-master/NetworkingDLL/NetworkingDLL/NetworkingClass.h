#pragma once
#include "PluginSettings.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <queue>
#include <string>
#include <thread>

const unsigned int BUFFER_LENGTH = 1024;

class PLUGIN_API NetworkingClass {
public:
	NetworkingClass() {
		bufferlength = sizeof(fromAddr);
		StartClient();
	}

	~NetworkingClass() {
		if (shutdown(cli_socket, SD_BOTH) == SOCKET_ERROR) {
			printf("Shutdown failed!  %d\n", WSAGetLastError());
			closesocket(cli_socket);
			WSACleanup();
		}
		closesocket(cli_socket);
		freeaddrinfo(ptr);
		WSACleanup();
	}

	int InitWinSock();
	int CreateSocket();
	int SendPacket();
	int RecvPacket();
	int ShutDown();
	int StartClient();
	int JoinGame();

	void StartCommunication();
	void StopCommunication();
	void HandlePacket();
	void HandleMessage(std::istringstream&, std::string);
	bool GetClientStatus();
	void SetClientStatus(bool);

	void AddOutbound(std::string);
	std::string GetOutbound();
	void RemoveOutbound();

	void AddCSInbound(std::string);
	std::string GetCSInbound();
	void RemoveCSInbound();
	int GetCSInSize();

	std::string GetClientName();

private:
	//Winsock objects
	WSADATA wsa;
	SOCKET cli_socket;
	struct addrinfo* ptr = NULL, hints;
	struct sockaddr_in fromAddr;
	u_long mode = 1;

	//Buffers
	char send_buf[BUFFER_LENGTH];
	char recv_buf[BUFFER_LENGTH];
	int bufferlength;

	bool ClientRunning = false;

	//Packet vectors
	std::queue<std::string> InboundCPPMsgs;//For plugin
	std::queue<std::string> InboundCSMsgs;//For unity
	std::queue<std::string> OutboundMsgs;

	//Client values
	std::string ClientName = "";
};
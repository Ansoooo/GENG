#include "NetworkingClass.h"
#include <string>
#include <sstream>
/*----------------------------------------------------
//MESSAGES SETUP: 
//CPP or C# - Header
//UPDATE/ACCEPT - Message type
//After this comes the data you need to bring in
------------------------------------------------------*/

int NetworkingClass::InitWinSock() //error would be 1
{
	int error;
	error = WSAStartup(MAKEWORD(2, 2), &wsa);
	if (error != 0) {
		printf("Failed to initialize %d\n", error);
		return 1;
	}
	return 0;
}

//If any of this errors, the first if statement is 2
//and the second if statement is 3
int NetworkingClass::CreateSocket()
{
	memset(&hints, 0, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_DGRAM;
	hints.ai_protocol = IPPROTO_UDP;

	if (getaddrinfo("localhost", "8888", &hints, &ptr) != 0) {
		printf("Getaddrinfo failed!! %d\n", WSAGetLastError());
		WSACleanup();
		return 2;
	}

	cli_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	if (cli_socket == INVALID_SOCKET) {
		printf("Failed creating a socket %d\n", WSAGetLastError());
		WSACleanup();
		return 3;
	}

	ioctlsocket(cli_socket, FIONBIO, &mode);
	return 0;
}

//Send error = 4
int NetworkingClass::SendPacket()
{
	//Sends front of outbound message queue and removes that message from the queue
	if (OutboundMsgs.size() > 0) {
		std::string line = GetOutbound();
		char* message = (char*)line.c_str();

		if (sendto(cli_socket, message, BUFFER_LENGTH, 0,
			ptr->ai_addr, ptr->ai_addrlen) == SOCKET_ERROR) {
			printf("sendto() failed %d\n", WSAGetLastError());
			return 4;
		}

		RemoveOutbound();
		return 0;
	}

	return 4;
}

int NetworkingClass::RecvPacket()
{

	//Adds a message to the inbound message queue that is later processed by unity
	if (recvfrom(cli_socket, recv_buf, sizeof(recv_buf), 0, (struct sockaddr*) & fromAddr, &bufferlength) == SOCKET_ERROR) {
		printf("recvfrom() failed...%d\n", WSAGetLastError());
		return 5; //Receive error
	}

	std::string MsgRecvd = recv_buf;
	if (MsgRecvd != "") {
		InboundCPPMsgs.push(MsgRecvd);
	}
	return 0;
}

int NetworkingClass::ShutDown()
{
	if (shutdown(cli_socket, SD_BOTH) == SOCKET_ERROR) {
		printf("Shutdown failed!  %d\n", WSAGetLastError());
		closesocket(cli_socket);
		WSACleanup();
		return 6; //Shutdown error
	}
	closesocket(cli_socket);
	freeaddrinfo(ptr);
	WSACleanup();
	return 0;
}

int NetworkingClass::StartClient()
{
	int Error;

	Error = this->InitWinSock();
	if (Error > 0) {
		return Error;
	}

	Error = this->CreateSocket();
	if (Error > 0) {
		return Error;
	}
	return 0;
}

void NetworkingClass::StartCommunication() {

	ClientRunning = true;
}

void NetworkingClass::StopCommunication() {

	ClientRunning = false;
}

void NetworkingClass::HandlePacket()
{

	if (InboundCPPMsgs.size() > 0) {
		std::string Msg = InboundCPPMsgs.front();
		std::istringstream iss(Msg);
		std::string Header;
		std::getline(iss, Header);

		if (Header == "CPP") {
			this->HandleMessage(iss, Msg);
			InboundCPPMsgs.pop();
		}
		else if (Header == "CS") {
			InboundCSMsgs.push(Msg);
			InboundCPPMsgs.pop();
		}
	}
}

void NetworkingClass::HandleMessage(std::istringstream& iss,std::string Msg)
{
	std::string Line;
	while (std::getline(iss, Line)) {
		if (Line == "ACCEPT") {
			std::string Name;
			std::getline(iss, Name);
			this->ClientName = Name;
		}
	}
}

bool NetworkingClass::GetClientStatus()
{
	return this->ClientRunning;
}

void NetworkingClass::SetClientStatus(bool temp)
{
	ClientRunning = temp;
}

int NetworkingClass::JoinGame() //gets player's name
{
	std::string Msg = "JOIN";
	AddOutbound(Msg);
	this->StartCommunication();
	return 0;
}

void NetworkingClass::AddOutbound(std::string msg)
{
	OutboundMsgs.push(msg);
}

std::string NetworkingClass::GetOutbound()
{
	if (OutboundMsgs.size() > 0) {
		return OutboundMsgs.front();
	}
	return "NULL";
}

void NetworkingClass::RemoveOutbound()
{
	if (OutboundMsgs.size() > 0) {
		OutboundMsgs.pop();
	}
	
}

void NetworkingClass::AddCSInbound(std::string msg)
{
	InboundCSMsgs.push(msg);
}

std::string NetworkingClass::GetCSInbound()
{
	if (InboundCSMsgs.size() > 0) {
		return InboundCSMsgs.front();
	}
	return "NULL";
}

void NetworkingClass::RemoveCSInbound()
{
	if (InboundCSMsgs.size() > 0) {
		InboundCSMsgs.pop();
	}
}

int NetworkingClass::GetCSInSize()
{
	return InboundCSMsgs.size();
}

std::string NetworkingClass::GetClientName()
{
	return this->ClientName;
}

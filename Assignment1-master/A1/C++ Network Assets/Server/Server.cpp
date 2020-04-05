#include "Server.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <vector>
#include <queue>
#include <thread>
#include <iostream>
#include <fstream>
#include <cstring> // changed : to add isdigit() for score check
#include <map>
#include <cassert>

#pragma comment(lib, "Ws2_32.lib")
using namespace std;

int NumberOfLobbies = 0;

vector<Lobby> ActiveLobbies;
vector<User> users;
queue<OutboundPacket> outboundMessages;
queue<InboundPacket> inboundMessages;

bool ServerRunning = true;

SOCKET server_socket;
const unsigned int BUF_LEN = 512;

sockaddr_in fromAddr;
int fromlen = sizeof(fromAddr);
char recv_buf[BUF_LEN];

std::fstream scoreFile;
float highestScore = 0.0f;
std::vector<float> scores;

void CreateUpdate() {

	string message = "UPDATE\n";

	for (User user : users) {
		message += user.UserName + "\n" + user.Status + "\n" + to_string(user.Score) + "\n"; // changed : to send score to clients so they see aswell.
	}

	OutboundPacket TempPacket;
	TempPacket.Message = message;
	TempPacket.ToWho = "ALL";
	outboundMessages.push(TempPacket);
}

void Broadcast() {

	if (outboundMessages.size() > 0) {
		OutboundPacket MessagePacket = outboundMessages.front();
		string Message = MessagePacket.Message;

		char* message = (char*)Message.c_str();

		for (User user : users) {
			sockaddr_in tempaddr = user.Address;

			if (sendto(server_socket, message, BUF_LEN, 0,
				(struct sockaddr*) & tempaddr, sizeof(tempaddr)) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}
			cout << "Sent data to: " << user.UserName << endl;
		}
		outboundMessages.pop();
	}
}

void SendToSpecific() {

	if (outboundMessages.size() > 0) {
		OutboundPacket packet = outboundMessages.front();

		char* message = (char*)packet.Message.c_str();

		for (sockaddr_in receipient : packet.Receipients) {
			if (sendto(server_socket, message, BUF_LEN, 0,
				(struct sockaddr*) & receipient, sizeof(receipient)) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}
			cout << "Sent packet to client" << endl;
		}
		
		outboundMessages.pop();
	}
}

void SendProcess() {
	while (ServerRunning) {
		if (outboundMessages.size() > 0) {
			OutboundPacket CurrentPacket = outboundMessages.front();
			if (CurrentPacket.ToWho == "ALL") {
				Broadcast();
			}
			else if (CurrentPacket.ToWho == "SPECIFIC") {
				SendToSpecific();
			}
		}
	}
}

void RecProcess() {
	while (ServerRunning) {
		memset(recv_buf, 0, BUF_LEN);
		if (recvfrom(server_socket, recv_buf, sizeof(recv_buf), 0, (struct sockaddr*) & fromAddr, &fromlen) == SOCKET_ERROR) {
			printf("recvfrom() failed...%d\n", WSAGetLastError());
		}
		//There needs to be an else here! Before we were constantly appending to inboundMessages regardless of network traffic!!!
		else {
			char ipbuf[INET_ADDRSTRLEN];
			inet_ntop(AF_INET, &fromAddr, ipbuf, sizeof(ipbuf));

			string SendersIP = ipbuf;

			InboundPacket TempPacket;
			TempPacket.SenderIP = SendersIP;
			TempPacket.Message = recv_buf;
			TempPacket.Address = fromAddr;
			inboundMessages.push(TempPacket);
		}
	}
}

MessageType getMessageType(string message) {
	if (message.substr(0, 4) == "JOIN") return MessageType::Join;
	//if (message.substr(0, 6) == "INVITE") return MessageType::Invite;
	//if (message.substr(0, 7) == "DECLINE") return MessageType::Decline;
	//if (message.substr(0, 6) == "ACCEPT") return MessageType::Accept;
	//if (message.substr(0, 4) == "CHAT") return MessageType::Chat;
	//if (message.substr(0, 5) == "LEAVE") return MessageType::Leave;
	if (message.substr(0, 5) == "SCORE") return MessageType::Score;
	return MessageType::Invalid;
}

namespace Packet {
	OutboundPacket createOutboundPacket(User sender, User receipient, string who, string messageType) {
		OutboundPacket packet;
		packet.ToWho = who;
		packet.Receipients.push_back(receipient.Address);
		packet.Message = messageType;
		packet.Message += "\n";
		packet.Message += sender.UserName;

		return packet;
	}

	OutboundPacket createOutboundPacket(User sender, User receipient, string who, string messageType, string message) {
		OutboundPacket packet;
		packet.ToWho = who;
		packet.Receipients.push_back(receipient.Address);
		packet.Message = messageType;
		packet.Message += "\n";
		packet.Message += sender.UserName;
		packet.Message += "\n";
		packet.Message += message;

		return packet;
	}

	OutboundPacket createOutboundPacket(User sender, vector<User> receipients, string who, string messageType) {
		vector<sockaddr_in> receipientAddresses;
		for (User receipient : receipients) {
			receipientAddresses.push_back(receipient.Address);
		}

		OutboundPacket packet;
		packet.ToWho = who;
		packet.Receipients = receipientAddresses;
		packet.Message = messageType;
		packet.Message += "\n";
		packet.Message += sender.UserName;

		return packet;
	}

	OutboundPacket createOutboundPacket(User sender, vector<User> receipients, string who, string messageType, string message) {
		vector<sockaddr_in> receipientAddresses;
		for (User receipient : receipients) {
			receipientAddresses.push_back(receipient.Address);
		}

		OutboundPacket packet;
		packet.ToWho = who;
		packet.Receipients = receipientAddresses;
		packet.Message = messageType;
		packet.Message += "\n";
		packet.Message += sender.UserName;
		packet.Message += "\n";
		packet.Message += message;

		return packet;
	}
}

namespace MessageHandler {

	void join(string ip, string message) {
		string userName = message.substr(4);
		cout << userName << " has joined the server" << endl;
		User newUser;
		newUser.IP = ip;
		newUser.Status = "Online";
		newUser.UserName = userName;
		newUser.Score = 0; // changed : to initialize new user score
		newUser.Address = ::fromAddr;

		bool ipTaken = false;
		bool userNameTaken = false;

		for (User user : ::users) {
			if (newUser.IP == user.IP) {
				ipTaken = true;
			}

			if (newUser.UserName == user.UserName) {
				userNameTaken = true;
			}
		}

		if (userNameTaken) {
			newUser.UserName += "(2)";
		}

		if (!ipTaken) {
			::users.push_back(newUser);
		}
		CreateUpdate();
	}

	//Find the highest score, then enqueue it for sending to the client.
	void score(string ip, string message)
	{
		printf("Score packet received: %s\n", message.c_str());
		//Get the score as a float.
		float score = std::stof(message.substr(6, message.length() - 6));

		bool found = false;
		for (size_t i = 0; i < scores.size(); i++) {
			if (scores[i] == score)
				found = true;
			if (scores[i] > highestScore)
				highestScore = scores[i];
		}
		if (!found) {
			scoreFile.open("score.txt", ios::app);
			scores.push_back(score);
			scoreFile << score << endl;
			scoreFile.flush();
			scoreFile.close();
			if (score > highestScore)
				highestScore = score;
		}

		OutboundPacket TempPacket;
		TempPacket.Message = "SCORE#" + std::to_string(highestScore);
		TempPacket.ToWho = "ALL";
		outboundMessages.push(TempPacket);
	}

	/*void invite(string senderIp, string message) {
		cout << "Invite Initiated \n";
		string receipientUserName = message.substr(7);

		User sender;
		User receipient;

		bool senderFound = false;
		bool receipientFound = false;

		for (User user : ::users) {
			if (user.UserName == receipientUserName) {
				cout << "Receipient Found \n";
				receipientFound = true;
				receipient = user;
			}
			else if (user.IP == senderIp) {
				cout << "Sender Found \n";
				senderFound = true;
				sender = user;
			}
		}

		if (senderFound && receipientFound) {
			cout << "Sending packet \n";
			OutboundPacket packet = Packet::createOutboundPacket(sender, receipient, "SPECIFIC", "INVITE");
			outboundMessages.push(packet);
		}
	}*/

	/*void decline(string senderIp, string message) {
		cout << "DECLINED INVITE" << endl;
		string receipientUserName = message.substr(8);

		User sender;
		User receipient;

		bool senderFound = false;
		bool receipientFound = false;

		for (User user : ::users) {
			if (user.IP == senderIp) {
				senderFound = true;
				sender = user;
			}
			else if (user.UserName == receipientUserName) {
				receipientFound = true;
				receipient = user;
			}
		}

		if (senderFound && receipientFound) {
			OutboundPacket packet = Packet::createOutboundPacket(sender, receipient, "SPECIFIC", "DECLINED");
			::outboundMessages.push(packet);
		}
	}*/

	/*void accept(string senderIp, string message) {
		string receipientUserName = message.substr(7);
		cout << receipientUserName << " ACCEPTED INVITE" << endl;

		User sender;
		User receipient;

		bool senderFound = false;
		bool receipientFound = false;

		for (User user : ::users) {
			if (user.IP == senderIp) {
				senderFound = true;
				sender = user;
			}
			else if (user.UserName == receipientUserName) {
				receipientFound = true;
				receipient = user;
			}
		}

		if (!senderFound || !receipientFound) return;

		cout << "READ";

		for (int i = 0; i < users.size(); i++) {
			if (users[i].UserName == sender.UserName || users[i].UserName == receipient.UserName) {
				users[i].Status = "IN LOBBY";
			}
		}
		sender.Status = "IN LOBBY";
		receipient.Status = "IN LOBBY";

		CreateUpdate();

		OutboundPacket receipientPacket = Packet::createOutboundPacket(sender, receipient, "SPECIFIC", "JOIN");
		OutboundPacket senderPacket = Packet::createOutboundPacket(receipient, sender, "SPECIFIC", "JOIN");
		outboundMessages.push(receipientPacket);
		outboundMessages.push(senderPacket);

		Lobby newLobby;
		newLobby.LobbyID = NumberOfLobbies;
		newLobby.users.push_back(sender);
		newLobby.users.push_back(receipient);
		NumberOfLobbies++;
		ActiveLobbies.push_back(newLobby);
	}*/

	/*void chat(string senderIp, string message) {
		cout << "Chat received " << message;
		Lobby targetLobby;
		string SenderName;
		vector<User> receipients;

		User sender;
		bool senderFound = false;

		for (Lobby lobby : ActiveLobbies) {
			for (User user : lobby.users) {
				if (senderIp == user.IP) {
					senderFound = true;
					targetLobby = lobby;
					sender = user;
				}
			}
		}

		if (!senderFound) return;

		for (User user : targetLobby.users) {
			if (user.IP != sender.IP) {
				receipients.push_back(user);
			}
		}

		string messageBody = message.substr(5);
		cout << messageBody;

		OutboundPacket packet = Packet::createOutboundPacket(
			sender,
			receipients,
			"SPECIFIC",
			"CHAT",
			messageBody
		);

		outboundMessages.push(packet);
	} */

	/*void leave(string senderIp) {
		Lobby targetLobby;
		string SenderName;
		vector<User> receipients;

		User sender;
		bool senderFound = false;

		for (Lobby lobby : ActiveLobbies) {
			for (User user : lobby.users) {
				if (sender.IP == user.IP) {
					senderFound = true;
					targetLobby = lobby;
					sender = user;
				}
			}
		}

		if (!senderFound) return;

		for (User user : targetLobby.users) {
			if (user.IP != sender.IP) {
				receipients.push_back(user);
			}
		}

		OutboundPacket packet = Packet::createOutboundPacket(sender, receipients, "SPECIFIC", "LEAVE");

		for (int i = 0; i < users.size(); i++) {
			if (users[i].UserName == sender.UserName) {
				users[i].Status = "Online";
			}
		}

		for (int i = 0; i < users.size(); i++) {
			for (int j = 0; j < receipients.size(); j++) {
				if (receipients[j].UserName == users[i].UserName) {
					users[i].Status = "Online";
				}
			}
		}

		outboundMessages.push(packet);
		CreateUpdate();
	}*/
}

void handleMessage(string senderIp, string message) {
	MessageType messageType = getMessageType(message);
	switch (messageType) {
	case MessageType::Join:
		MessageHandler::join(senderIp, message);
		break;
	/*case MessageType::Invite:
		MessageHandler::invite(senderIp, message);
		break;
	case MessageType::Decline:
		MessageHandler::decline(senderIp, message);
		break;
	case MessageType::Accept:
		MessageHandler::accept(senderIp, message);
		break;
	case MessageType::Chat:
		MessageHandler::chat(senderIp, message);
		break;
	case MessageType::Leave:
		MessageHandler::leave(senderIp);*/
	case MessageType::Score:
		MessageHandler::score(senderIp, message);
	default:
		break;
	}
}

void handleScore() {

}

void Facilitate() {
	if (inboundMessages.size() == 0) return;

	InboundPacket packet = inboundMessages.front();
	string message = packet.Message;
	string senderIp = packet.SenderIP;
	
	handleMessage(senderIp, message);
	inboundMessages.pop();
}

void ServerProc() {
	while (ServerRunning) {
		Facilitate();
	}
}

int main() {
	WSADATA wsa;
	int error;
	error = WSAStartup(MAKEWORD(2, 2), &wsa);

	if (error != 0) {
		printf("Failed to initialize %d\n", error);
		return 1;
	}

	struct addrinfo* ptr = NULL, connection;

	memset(&connection, 0, sizeof(connection));
	connection.ai_family = AF_INET;
	connection.ai_socktype = SOCK_DGRAM;
	connection.ai_protocol = IPPROTO_UDP;
	connection.ai_flags = AI_PASSIVE;

	if (getaddrinfo(NULL, "8888", &connection, &ptr) != 0) {
		printf("Getaddrinfo failed!! %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}
	server_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	if (server_socket == INVALID_SOCKET) {
		printf("Failed creating a socket %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	if (::bind(server_socket, ptr->ai_addr, (int)ptr->ai_addrlen) < 0) {
		printf("Bind failed: %d\n", WSAGetLastError());
		closesocket(server_socket);
		freeaddrinfo(ptr);
		WSACleanup();
		return 1;
	}

	//Load in all scores.
	std::vector<float> scores;
	std::string line;
	while (getline(scoreFile, line))
		scores.push_back(std::stof(line));
	
	cout << "Running.\n";
	thread ServerProcess{ServerProc};
	thread SendProc{SendProcess};
	thread RecProc{RecProcess};
	
	ServerProcess.join();
	SendProc.join();
	RecProc.join();

	closesocket(server_socket);
	freeaddrinfo(ptr);
	WSACleanup();

	scoreFile.close();

	return 0;
}
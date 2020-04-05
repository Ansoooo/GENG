#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <vector>
#include <queue>
#include <thread>

#pragma comment(lib, "Ws2_32.lib")

using namespace std;

typedef struct User {
	string IP = "";
	string UserName = "";
	string Status = "";
	float Score = 0.0f;
	sockaddr_in Address;
};

enum MessageType {
	Join,
	//Invite,
	//Decline,
	//Accept,
	//Chat,
	//Leave,
	Score,
	Invalid
};

typedef struct InboundPacket {
	string SenderIP = "";
	string Message = "";
	sockaddr_in Address;
};

typedef struct OutboundPacket {
	string Message = "";
	string ToWho = "";
	vector<sockaddr_in> Receipients;
};

typedef struct Lobby {
	vector<User> users;
	vector<string> ChatLog;
	int LobbyID;
};

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <thread>
#include <sstream>
#include <vector>
#include <queue>
#include <thread>
#include <conio.h>
#pragma comment(lib, "Ws2_32.lib")

using namespace std;

typedef struct User {
	string Name;
	string Status;
	int Score; // changed : added to allow for scoring on client side
};

string MyName;
vector<User> OnlineUsers;
queue<string> ReceivedMessages;
queue<string> MessagesToSend;

bool StartUp = true;
bool InServer = false;
bool Skip = false;


//Networking variables
struct sockaddr_in fromAddr;
int fromlen = sizeof(fromAddr);

struct addrinfo* ptr = NULL, hints;
SOCKET cli_socket;
const unsigned int BUF_LEN = 512;
char recv_buf[BUF_LEN];

bool InviteSent = false;
bool InviteRec = false;
string Inviter;
bool InChatRoom = false;

void UpdateUsers(istringstream& iss, string Message) {
	string Name;
	string Status;
	string Score;
	while (getline(iss, Name))
	{
		getline(iss, Status);
		getline(iss, Score);

		bool userFound = false;
		for (int i = 0; i < OnlineUsers.size(); i++) {
			if (OnlineUsers[i].Name == Name) {
				userFound = true;
				if (OnlineUsers[i].Status != Status) {
					OnlineUsers[i].Status = Status;
				}
				if (OnlineUsers[i].Score != std::stof(Score, NULL)) { // changed : to update existing user score from incoming message
					OnlineUsers[i].Score = std::stof(Score, NULL);
				}
			}
		}
		if (!userFound) {
			User newUser;
			newUser.Name = Name;
			newUser.Status = Status;
			newUser.Score = std::stof(Score, NULL); // changed : to update new user score from incoming message
			OnlineUsers.push_back(newUser);
		}
	}
}
void DisplayUsers() {
	cout << "Me: " << MyName << endl;
	for (int i = 0; i < OnlineUsers.size(); i++) {

		cout << "[" << OnlineUsers[i].Status + "] : " + OnlineUsers[i].Name + " ----- User's score: " + to_string(OnlineUsers[i].Score) << endl << endl; // changed : to display user scores and made it prettier?
	}
}

void HandleRecMessages(){
	if (ReceivedMessages.size() > 0) {

		while (ReceivedMessages.size() > 0) // changed : so that it refreshes all updates instead of 1 each time user enters "1"
		{
			string Msg = ReceivedMessages.front();
			istringstream iss(Msg);
			string Type;
			getline(iss, Type);
			if (Type == "UPDATE") {
				system("cls");
				UpdateUsers(iss, Msg);
				DisplayUsers();
				Skip = false;
			}
			/*else if (Type == "INVITE") {
				InviteRec = true;
				string Name;
				getline(iss, Name);
				Inviter = Name;
				cout << "Invite Received From: " << Name << endl;
			}*/
			else if (Type == "JOIN") {
				InChatRoom = true;
			}
			/*else if (Type == "CHAT") {
				string Name;
				string Message;
				getline(iss, Name);
				getline(iss, Message);
				cout << Name << ": " << Message << endl;
			}*/
			/*else if (Type == "LEAVE") {
				InChatRoom = false;
				InviteRec = false;
				InviteSent = false;
				Skip = true;
			}*/
			else if (Type == "SCORE") { // changed : um idk why i uncommented this but seems to not affect anything
				string Name;
				string Score;
				getline(iss, Name);
				getline(iss, Score);
			}


			ReceivedMessages.pop();
		}
	}
}

void handleRefresh() {
	system("cls");
	DisplayUsers();
}

void handleReceiveInvite() {
	cout << "Enter (Y/N) Y = Accept invite, N = Decline invite" << endl;
	string line;
	getline(cin, line);

	string choice = line.substr(0, 1);
	if (choice == "Y" || choice == "y") {
		string Msg = "ACCEPT " + Inviter;
		MessagesToSend.push(Msg);
		InChatRoom = true;
	}
	else if (choice == "N" || choice == "n") {
		string Msg = "DECLINE " + Inviter;
		MessagesToSend.push(Msg);
		InviteRec = false;
		Inviter = "";
	}
}

//void handleSendScore(string line) {
//	string User = line.substr(6);
//
//	string Message = "SCORE" + User;
//	MessagesToSend.push(Message);
//	//InviteSent = true;
//}

void ClientProcess() {
	while (InServer == true) {
		if (!InChatRoom) {
			HandleRecMessages();
		}
		
		if (OnlineUsers.size() > 1 && InChatRoom == false && InviteRec == false && InviteSent == false && !Skip) {
			cout << "Enter 1 to refresh connection" << endl;
			//cout << "Enter 2 followed by the user's user name to see their score" << endl;
			
			string line;
			getline(cin, line);

			if (line.substr(0,1) == "1") {
				handleRefresh();
			}
			/*else if (line.substr(0, 1) == "2") {
				handleSendScore(line);
			}*/
		}
		else if (OnlineUsers.size() > 1 && InviteRec == true && InChatRoom == false && !Skip) {
			handleReceiveInvite();
		}
		else if (InChatRoom == true && !Skip) {
			string line;
			getline(cin, line);

			string Msg = "CHAT " + line;

			if (line == "leave" || line == "LEAVE") {
				Msg = "LEAVE";
				InChatRoom = false;
				InviteRec = false;
				InviteSent = false;
				Skip = true;
			}
			MessagesToSend.push(Msg);
		}
		else if (Skip) {
			system("CLS");
			cout << "Waiting..." << endl;
		}
	}
}

void ReceiveProcess() {
	while (InServer == true) {
		memset(recv_buf, 0, BUF_LEN);
		if (recvfrom(cli_socket, recv_buf, sizeof(recv_buf), 0, (struct sockaddr*) & fromAddr, &fromlen) == SOCKET_ERROR) {
			printf("Receive failed...%d\n", WSAGetLastError());
		}

		string Message = recv_buf;
		ReceivedMessages.push(Message);
		
		if (InChatRoom) {
			HandleRecMessages();
		}
	}
}

void SendProcess() {
	while (InServer == true) {
		if (MessagesToSend.size() > 0) {

			string Message = MessagesToSend.front();

			char* message = (char*)Message.c_str();
			if (sendto(cli_socket, message, BUF_LEN, 0,
				ptr->ai_addr, ptr->ai_addrlen) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
			}

			MessagesToSend.pop();
		}
	}
}

int main() {

	//Initialize winsock
	WSADATA wsa;

	int error;
	error = WSAStartup(MAKEWORD(2, 2), &wsa);

	if (error != 0) {
		printf("Failed to initialize %d\n", error);
		return 1;
	}



	//Create a client socket
	memset(&hints, 0, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_DGRAM;
	hints.ai_protocol = IPPROTO_UDP;

	//Ask for IP
	cout << "Please Enter the Server IP you want to join: ";
	string IPTOJOIN;
	getline(cin, IPTOJOIN);

	if (getaddrinfo(IPTOJOIN.c_str(), "8888", &hints, &ptr) != 0) {
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

	string Username;
	do {
		system("CLS");
		
		cout << "Server Joined Successfully" << endl;
		cout << "Please Enter Your Username " << endl;
		getline(cin, Username);
		if (Username != "") {
			MyName = Username;
			StartUp = false;

			string line = "JOIN" + Username;

			char* message = (char*)line.c_str();

			if (sendto(cli_socket, message, BUF_LEN, 0,
				ptr->ai_addr, ptr->ai_addrlen) == SOCKET_ERROR) {
				printf("sendto() failed %d\n", WSAGetLastError());
				return 4;
			}


			if (recvfrom(cli_socket, recv_buf, sizeof(recv_buf), 0, (struct sockaddr*) & fromAddr, &fromlen) == SOCKET_ERROR) {
				printf("recvfrom() failed...%d\n", WSAGetLastError());
				return 25;
			}

			string temp = recv_buf;
			ReceivedMessages.push(temp);
			
			InServer = true;
		}

	} while (StartUp == true);

	thread ClientProc { ClientProcess };
	thread RecProc { ReceiveProcess };
	thread SendProc {SendProcess};

	ClientProc.join();
	RecProc.join();
	SendProc.join();
	//Shutdown the socket

	if (shutdown(cli_socket, SD_BOTH) == SOCKET_ERROR) {
		printf("Shutdown failed!  %d\n", WSAGetLastError());
		closesocket(cli_socket);
		WSACleanup();
		return 6;
	}
	closesocket(cli_socket);
	freeaddrinfo(ptr);
	WSACleanup();

	return 0;
}
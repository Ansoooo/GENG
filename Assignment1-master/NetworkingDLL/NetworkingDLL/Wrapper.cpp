#include "Wrapper.h"
#include <comutil.h>
#include <stdio.h>
#include <atlbase.h>
#include <atlconv.h>
#include <comdef.h>

NetworkingClass Client;

int JoinGame()
{
	return Client.JoinGame();
}

BSTR GetName()
{
	std::string Mystr = Client.GetClientName();
	BSTR Name = SysAllocString(CA2W(Mystr.c_str()));
	return Name;
}

void AddMsg(const char * temp) {

	std::string Final = temp;
	Client.AddOutbound(Final);
}

BSTR GetInboundMsg()
{
	return SysAllocString(CA2W(Client.GetCSInbound().c_str()));
}

void RemoveInboundMsg()
{
	Client.RemoveCSInbound();
}

void SendPacket()
{
	Client.SendPacket();
}

void RecvPacket()
{
	Client.RecvPacket();
}

void HandlePacket()
{
	Client.HandlePacket();
}

void SetStatus(bool temp)
{
	Client.SetClientStatus(temp);
}

bool GetStatus()
{
	return Client.GetClientStatus();
}

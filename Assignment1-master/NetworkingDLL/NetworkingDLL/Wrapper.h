#pragma once
#include "PluginSettings.h"
#include "NetworkingClass.h"
#ifdef __cplusplus
extern "C"
{
#endif
	// Put your functions here

	PLUGIN_API int JoinGame();
	PLUGIN_API BSTR GetName();
	PLUGIN_API void SendPacket();
	PLUGIN_API void RecvPacket();
	PLUGIN_API void HandlePacket();
	PLUGIN_API void SetStatus(bool);
	PLUGIN_API bool GetStatus();
	PLUGIN_API void AddMsg(const char*);
	PLUGIN_API BSTR GetInboundMsg();
	PLUGIN_API void RemoveInboundMsg();

#ifdef __cplusplus
}
#endif
#pragma once
#include "PluginSettings.h"
#include "operationTracker.h"

#ifdef __cplusplus
extern "C"
{
#endif
	// operationTracker Functions
	PLUGIN_API void resetFile();
	PLUGIN_API void increLog(float, int, bool);
	PLUGIN_API void saveToFile();
	PLUGIN_API void getFromFile();
	PLUGIN_API float retriLog(int);

#ifdef __cplusplus
}
#endif
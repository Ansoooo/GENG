#pragma once
#ifdef NETWORKPLUGIN_EXPORTS
#define PLUGIN_API __declspec(dllexport)
#elif NETWORKPLUGIN_IMPORTS
#define PLUGIN_API __declspec(dllimport)
#else
#define PLUGIN_API
#endif
#pragma once
#include "PluginSettings.h"
class PLUGIN_API operationTracker
{
public:
	float savedPosiX[21] = { 0 };
	float savedPosiY[21] = { 0 };
	float savedPosiZ[21] = { 0 };

	int savedType[21] = { 0 };

	void savePosi(float, float, float, int);
	void saveType(int, int);
	float getPosiX(int);
	float getPosiY(int);
	float getPosiZ(int);
	int getType(int);
};
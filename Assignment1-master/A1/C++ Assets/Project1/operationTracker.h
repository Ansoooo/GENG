#pragma once
#include "PluginSettings.h"
#include <iostream>
#include <fstream>
using namespace std;

class PLUGIN_API operationTracker
{
public:
	float logOut[10] = { 0 };
	float logIn[10] = { 0 };
	ofstream logFileOut;
	ifstream logFileIn;

	void resetFile();
	void increLog(float, int, bool);
	void saveToFile();
	void getFromFile();
	float retriLog(int);
};
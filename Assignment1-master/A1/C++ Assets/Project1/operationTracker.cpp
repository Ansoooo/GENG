#include "operationTracker.h"

//ULM stuff
void operationTracker::resetFile()
{
	logFileOut.open("UserLog.txt", std::ofstream::trunc);
	for (int i = 0; i <= 9; i++)
	{
		logFileOut << 0 << " ";
	}
	logFileOut.close();

	for (int i = 0; i <= 9; i++)
	{
		logOut[i] = 0;
		logIn[i] = 0;
	}
}

void operationTracker::increLog(float _value, int _index, bool _continuousSave)
{
	logOut[_index] += _value;

	if (_continuousSave)
	{
		saveToFile();
	}
}

void operationTracker::saveToFile()
{
	logFileOut.open("UserLog.txt", std::ofstream::trunc);
	for (int i = 0; i <= 9; i++)
	{
		logFileOut << logOut[i] << " ";
	}
	logFileOut.close();
}

void operationTracker::getFromFile()
{
	logFileIn.open("UserLog.txt");
	
	while (!logFileIn.eof())
	{
		for (int i = 0; i <= 9; i++)
		{
			logFileIn >> logIn[i];
		}
	}
	logFileIn.close();
}

float operationTracker::retriLog(int _index)
{
	getFromFile();
	return logIn[_index];
}
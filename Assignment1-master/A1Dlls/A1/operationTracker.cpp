#include "operationTracker.h"

void operationTracker::savePosi(float _x, float _y, float _z, int index)
{
	savedPosiX[index] = _x;
	savedPosiY[index] = _y;
	savedPosiZ[index] = _z;
}
void operationTracker::saveType(int _type, int index)
{
	savedType[index] = _type;
}
float operationTracker::getPosiX(int index)
{
	return savedPosiX[index];
}
float operationTracker::getPosiY(int index)
{
	return savedPosiY[index];
}
float operationTracker::getPosiZ(int index)
{
	return savedPosiZ[index];
}
int operationTracker::getType(int index)
{
	return savedType[index];
}
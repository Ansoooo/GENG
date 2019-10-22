#include "Wrapper.h"

SimpleClass simpleClass;
int SimpleFunction()
{
	return simpleClass.SimpleFunction();
}

operationTracker OperationTracker;
void savePosi(float _x, float _y, float _z, int index)
{
	return OperationTracker.savePosi(_x, _y, _z, index);
}
void saveType(int _type, int index)
{
	return OperationTracker.saveType(_type, index);
}
float getPosiX(int index)
{
	return OperationTracker.getPosiX(index);
}
float getPosiY(int index)
{
	return OperationTracker.getPosiY(index);
}
float getPosiZ(int index)
{
	return OperationTracker.getPosiZ(index);
}
int getType(int index)
{
	return OperationTracker.getType(index);
}
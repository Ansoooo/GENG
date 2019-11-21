#include "Wrapper.h"

operationTracker OperationTracker;
void resetFile()
{
	return OperationTracker.resetFile();
}
void increLog(float _value, int _index, bool _continousSave)
{
	return OperationTracker.increLog(_value, _index, _continousSave);
}
void saveToFile()
{
	return OperationTracker.saveToFile();
}
void getFromFile()
{
	return OperationTracker.getFromFile();
}
float retriLog(int _index)
{
	return OperationTracker.retriLog(_index);
}
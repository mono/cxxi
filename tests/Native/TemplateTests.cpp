#include "TemplateTests.h"

template<class T>
AdderTemplate<T>::AdderTemplate()
: num(0)
{
}

template<class T>
AdderTemplate<T>::AdderTemplate(T value)
: num(value)
{
}

template<class T>
T AdderTemplate<T>::Number() const { return num; }

template<class T>
T AdderTemplate<T>::Add(T value)
{
	num += value;
	return num;
}


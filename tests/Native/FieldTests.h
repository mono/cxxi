#include "common.h"

class EXPORT HasField {
public:
	int number;
	HasField* other;
	HasField (int number, HasField* other);
};

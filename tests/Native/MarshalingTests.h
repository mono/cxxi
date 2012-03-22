#include "common.h"

class EXPORT ClassWithCopyCtor {
	int x;

public:
	ClassWithCopyCtor(int xarg) {
		x = xarg;
	}

	ClassWithCopyCtor(const ClassWithCopyCtor& f);

	static ClassWithCopyCtor Return (int x);

	int GetX ();
};

class EXPORT ClassWithDtor {
	int x;

public:
	ClassWithDtor(int xarg) {
		x = xarg;
	}

	~ClassWithDtor () {
	}

	static ClassWithDtor Return (int x);

	int GetX ();
};


class EXPORT ClassWithoutCopyCtor {
	int x;

public:
	ClassWithoutCopyCtor(int xarg) {
		x = xarg;
	}

	static ClassWithoutCopyCtor Return (int x);

	int GetX ();
};

class EXPORT Class {
	int x;

public:
	Class (int xarg) {
		x = xarg;
	}

	void CopyFromValue (Class c) {
		x = c.x;
	}

	void CopyTo (Class *c) {
		c->x = x;
	}

	bool IsNull (Class *c) {
		return !c ? true : false;
	}

	int GetX () {
		return x;
	}

	int& GetXRef () {
		return x;
	}
	
	static int ReturnInt(int x) { return x; }
	static unsigned int ReturnUInt(int x) { return (unsigned int)x; }
	static float ReturnFloat(int x) { return (float)x; }
	static double ReturnDouble(int x) { return (double)x; }
	static long double ReturnLDouble(int x) { return (long double)x; }
	static bool ReturnBool(int x) { return (x != 0); }
	static short ReturnShort(int x) { return (short)x; }
	static unsigned short ReturnUShort(int x) { return (unsigned short)x; }
	static long ReturnLong(int x) { return (long)x; }
	static unsigned long ReturnULong(int x) { return (unsigned long)x; }
	static char ReturnChar(int x) { return (char)x; }
	static unsigned char ReturnUChar(int x) { return (unsigned char)x; }
	static signed char ReturnSChar(int x) { return (signed char)x; }
};

		
		

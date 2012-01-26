#ifdef __GNUC__
#define EXPORT
#elif defined(_MSC_VER)
#define EXPORT __declspec(dllexport)
#else
#error Unknown compiler!
#endif

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
};

		
		

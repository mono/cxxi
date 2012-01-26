#ifdef __GNUC__
#define EXPORT
#elif defined(_MSC_VER)
#define EXPORT __declspec(dllexport)
#else
#error Unknown compiler!
#endif

class EXPORT HasField {
public:
	int number;
	HasField* other;
	HasField (int number, HasField* other);
};

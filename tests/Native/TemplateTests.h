#ifdef __GNUC__
#define EXPORT
#elif defined(_MSC_VER)
#define EXPORT __declspec(dllexport)
#else
#error Unknown compiler!
#endif

template<class T>
class EXPORT AdderTemplate
{
public:
	AdderTemplate();
	AdderTemplate(T value);
	~AdderTemplate() {}
	T Number() const;
	T Add(T value);

private:
	T num;
};

// Need typedefs in here for gccxml to work
typedef AdderTemplate<short> AdderShort;
typedef AdderTemplate<int> AdderInt;
typedef AdderTemplate<float> AdderFloat;
typedef AdderTemplate<double> AdderDouble;

// Create some instances so the type is exported
AdderShort atshort;
AdderInt atint;
AdderFloat atfloat;
AdderDouble atdouble;

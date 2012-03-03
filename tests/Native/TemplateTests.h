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

// Sigh... we need to actually use all the methods in order for them to be generated as well
void ForceSymbols()
{
	AdderInt a(1);
	int x = a.Number();
	a.Add(x);
	
	AdderShort b(1);
	short y = b.Number();
	b.Add(y);
	
	AdderFloat c(1.0);
	short z = c.Number();
	c.Add(z);
	
	AdderDouble d(1.0);
	short w = d.Number();
	d.Add(w);
}
#ifdef __GNUC__
#define EXPORT
#elif defined(_MSC_VER)
#define EXPORT __declspec(dllexport)
#else
#error Unknown compiler!
#endif

#include <stdio.h>

class EXPORT Compression {
public:
	static void Test1 (const Compression* a1, const char* a2, const Compression* a3, const char* a4);
	static int Test2 (const char *c1, const float *f1, const int *i1,
		const char *c2, const float *f2, const int *i2, double *d1, double *d2, int i) { printf("%d\n", i); return i; }
};

namespace Ns1 {
	class EXPORT Namespaced {
	public:
		static void Test1 ();
		static void Test2 (const Compression* a1);
		static int Test3 (const Compression *a1, const Compression *a2, float *f1, float *f2, int i) { printf("%d\n", i); return i; }
		static int Test4 (const Namespaced *n1, const Namespaced *n2, const Compression* a1, const Compression* a2, float *f1, float *f2, int i) { return i; }
		static int Test5 (Namespaced *n1, Namespaced *n2, const Compression* a1, const Compression* a2, int i) { return i; }
	};
}

namespace Ns1 { namespace Ns2 {
	class EXPORT Namespaced2 {
	public:
		Namespaced2 ();
		void Test1 ();
		Namespaced2* Test2 (Compression* a1);
		static int Test3 (Namespaced2 *n21, Namespaced2 *n22, Namespaced *n1, Namespaced *n2, int i) { printf("%d\n", i); return i; }
		static int Test4 (const Namespaced2 *n21, Namespaced2 *n22, const Namespaced *n1, Namespaced *n2,
			const Namespaced2 *n23, Namespaced2 *n24, const Namespaced *n3, Namespaced *n4, int i) { printf("%d\n", i); return i; }
		static int Test5 (const Namespaced2 *n21, const Namespaced2 *n22, const Compression *a1, const Compression *a2, int i) { printf("%d\n", i); return i; }
		static int Test6 (const Namespaced2 *n21, const Namespaced2 *n22, float *f1, float *f2, int i) { printf("%d\n", i); return i; }
	};
}}

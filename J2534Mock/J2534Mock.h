// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the J2534MOCK_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// J2534MOCK_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef J2534MOCK_EXPORTS
#define J2534MOCK_API __declspec(dllexport)
#else
#define J2534MOCK_API __declspec(dllimport)
#endif

// This class is exported from the J2534Mock.dll
class J2534MOCK_API CJ2534Mock {
public:
	CJ2534Mock(void);
	// TODO: add your methods here.
};

extern J2534MOCK_API int nJ2534Mock;

J2534MOCK_API int fnJ2534Mock(void);

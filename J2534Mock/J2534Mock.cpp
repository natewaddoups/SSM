// J2534Mock.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "J2534Mock.h"

class Device
{
public:
	wchar_t *szName;
	Device (wchar_t *szName)
	{
		this->szName = szName;
	}
};

class Channel
{
public:
	wchar_t *szName;
	unsigned long ProtocolId;

	Channel(wchar_t *szName)
	{
		this->szName = szName;
	}
};

class Config
{
public:
	unsigned long Parameter;
	unsigned long Value;

	void DumpConfig()
	{
		wchar_t *szMessage = new wchar_t[100]; 
		wsprintf(szMessage, L"    Config->Parameter: %08X\r\n", this->Parameter);
		OutputDebugString(szMessage);
		wsprintf(szMessage, L"    Config->Value: %08X\r\n", this->Value);
		OutputDebugString(szMessage);
		delete [] szMessage;
	}
};

class ConfigList
{
public:
	unsigned long NumOfParams;
	Config *pArray;
};

class PASSTHRU_MSG
{
public:
	unsigned long ProtocolId;
	unsigned long RxStatus;
	unsigned long TxFlags;
	unsigned long Timestamp;
	unsigned long DataSize;
	unsigned long ExtraDataIndex;
	unsigned char Data[4128];

	void DumpMessage()
	{
		wchar_t *szMessage = new wchar_t[100]; 
		wsprintf(szMessage, L"    Message->ProtocolId: %08X\r\n", this->ProtocolId);
		OutputDebugString(szMessage);

		wsprintf(szMessage, L"    Message->RxStatus: %08X\r\n", this->RxStatus);
		OutputDebugString(szMessage);

		wsprintf(szMessage, L"    Message->TxFlags: %08X\r\n", this->TxFlags);
		OutputDebugString(szMessage);

		wsprintf(szMessage, L"    Message->Timestamp: %08X\r\n", this->Timestamp);
		OutputDebugString(szMessage);

		wsprintf(szMessage, L"    Message->DataSize: %08X\r\n", this->DataSize);
		OutputDebugString(szMessage);

		wsprintf(szMessage, L"    Message->ExtraDataIndex: %08X\r\n", this->ExtraDataIndex);
		OutputDebugString(szMessage);

		wsprintf(szMessage, L"    Message->Data: %08X\r\n", (unsigned long) this->Data);
		OutputDebugString(szMessage);

		for (int i = 0; i < 10; i++)
		{
			wsprintf(szMessage, L"    %02X ", this->Data[i]);
			OutputDebugString(szMessage);
		}
		OutputDebugString(L"\r\n");

		delete [] szMessage;
	}
};

#define CCONV _cdecl

extern "C" {

J2534MOCK_API long CCONV PassThruOpen(
	void *pName,
	unsigned long *pDeviceId);

J2534MOCK_API long CCONV PassThruClose(
	unsigned long DeviceId);

J2534MOCK_API long CCONV PassThruSetProgrammingVoltage(
	unsigned long DeviceId,
	unsigned long PinNumber,
	unsigned long Voltage);

J2534MOCK_API long CCONV PassThruReadVersion(
	unsigned long DeviceId,
	char *pFirmwareVersion,
	char *pDllVersion,
	char *pApiVersion);

J2534MOCK_API long CCONV PassThruConnect(
	unsigned long DeviceId,
	unsigned long ProtocolId,
	unsigned long Flags,
	unsigned long BaudRate,
	unsigned long *pChannelId);

J2534MOCK_API long CCONV PassThruDisconnect(
	unsigned long ChannelId);

J2534MOCK_API long CCONV PassThruReadMsgs(
	unsigned long ChannelId,
	PASSTHRU_MSG *pMsg,
	unsigned long *pNumMsgs,
	unsigned long Timeout);

J2534MOCK_API long CCONV PassThruWriteMsgs(
	unsigned long ChannelId,
	PASSTHRU_MSG *pMsg,
	unsigned long *pNumMsgs,
	unsigned long Timeout);

J2534MOCK_API long CCONV PassThruStartPeriodicMsg(
	unsigned long ChannelId,
	PASSTHRU_MSG *pMsg,
	unsigned long *pMsgId,
	unsigned long TimeInterval);

J2534MOCK_API long CCONV PassThruStopPeriodicMsg(
	unsigned long ChannelId,
	unsigned long MsgId);

J2534MOCK_API long CCONV PassThruStartMsgFilter(
	unsigned long ChannelId,
	unsigned long FilterType,
	PASSTHRU_MSG *pMaskMsg,
	PASSTHRU_MSG *pPatternMsg,
	PASSTHRU_MSG *pFlowControlMsg,
	unsigned long *pFilterId);

J2534MOCK_API long CCONV PassThruStopMsgFilter(
	unsigned long ChannelId,
	unsigned long FilterId);

J2534MOCK_API long CCONV PassThruGetLastError(
	char *pErrorDescription);

J2534MOCK_API long CCONV PassThruIoctl(
	unsigned long ChannelId,
	unsigned long IoctlId,
	void *pInput,
	void *pOutput);
};

J2534MOCK_API long CCONV PassThruOpen(
	void *pName,
	unsigned long *pDeviceId)
{
	OutputDebugString(L"PassThruOpen\r\n");
	Device *pDevice = new Device(L"Mock Device");
	*pDeviceId = (unsigned long) pDevice;

	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruOpen returning %08X\r\n", *pDeviceId);
	OutputDebugString(szMessage);
	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruClose(
	unsigned long DeviceId)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruClose\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pDevice = %08X\r\n", DeviceId);
	OutputDebugString(szMessage);

	Device *pDevice = (Device*) DeviceId;
	wsprintf(szMessage, L"    Device = %s\r\n", pDevice->szName);
	OutputDebugString(szMessage);

	delete [] szMessage;
	delete pDevice;
	return 0;
}

J2534MOCK_API long CCONV PassThruSetProgrammingVoltage(
	unsigned long DeviceId,
	unsigned long PinNumber,
	unsigned long Voltage)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruSetProgrammingVoltage\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    DeviceId = %08X\r\n", DeviceId);
	OutputDebugString(szMessage);

	Device *pDevice = (Device*) DeviceId;
	wsprintf(szMessage, L"    Device = %s\r\n", pDevice->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    PinNumber = %d\r\n", PinNumber);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    Voltage = %d\r\n", Voltage);
	OutputDebugString(szMessage);

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruReadVersion(
	unsigned long DeviceId,
	char *pFirmwareVersion,
	char *pDllVersion,
	char *pApiVersion)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruReadVersion\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    DeviceId = %08X\r\n", DeviceId);
	OutputDebugString(szMessage);

	Device *pDevice = (Device*) DeviceId;
	wsprintf(szMessage, L"    Device = %s\r\n", pDevice->szName);
	OutputDebugString(szMessage);

	OutputDebugString(L"PassThruReadVersion setting strings...");
	pFirmwareVersion = "Mock Firmware";
	pDllVersion = "Mock DLL";
	pApiVersion = "Mock API";

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruConnect(
	unsigned long DeviceId,
	unsigned long ProtocolId,
	unsigned long Flags,
	unsigned long BaudRate,
	unsigned long *pChannelId)
{
	wchar_t *szMessage = new wchar_t[100]; 
	OutputDebugString(L"PassThruConnect\r\n");

	wsprintf(szMessage, L"	DeviceId = %08X\r\n", DeviceId);
	OutputDebugString(szMessage);
	
	Device *pDevice = (Device*) DeviceId;
	wsprintf(szMessage, L"    Device: %s\r\n", pDevice->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"	ProtocolId = %08X\r\n", ProtocolId);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"	Flags = %08X\r\n", Flags);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"	BaudRate = %d\r\n", BaudRate);
	OutputDebugString(szMessage);

	Channel *pChannel = new Channel(L"Mock Channel");
	pChannel->ProtocolId = ProtocolId;

	*pChannelId = (unsigned long) pChannel;
	
	wsprintf(szMessage, L"PassThruConnect returning ChannelId = %08X)\r\n", *pChannelId);
	OutputDebugString(szMessage);

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruDisconnect(
	unsigned long ChannelId)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruDisconnect (ChannelId = %08X)\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"Channel: %s\r\n", pChannel->szName);

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruReadMsgs(
	unsigned long ChannelId,
	PASSTHRU_MSG *pMsg,
	unsigned long *pNumMsgs,
	unsigned long Timeout)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruReadMsgs\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pMsg = %08X\r\n", (unsigned long) pMsg);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pNumMsgs = %08X\r\n", (unsigned long) pNumMsgs);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    *pNumMsgs = %08X\r\n", *pNumMsgs);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    Timeout = %08X\r\n", Timeout);
	OutputDebugString(szMessage);


	// Lenth = 8 * 7 + 6 = 62
	int ecuInitResponseLength = 62;

#pragma warning (push)
#pragma warning (disable: 4309)
#pragma warning (disable: 4838)
	char ecuInitResponse[] =
            {
                0x80, 0xF0, 0x10, 0x39, 0xFF, // 5 
                0xA2, 0x10, 0x11, 0x2F, 0x12, 0x78, 0x52, 0x06, // 8
                0x73, 0xFA, 0xCB, 0xA6, 0x2B, 0x81, 0xFE, 0xA8, // 8
                0x00, 0x00, 0x00, 0x60, 0xCE, 0x54, 0xF9, 0xB1, // 8
                0xE4, 0x00, 0x0C, 0x20, 0x00, 0x00, 0x00, 0x00, // 8
                0x00, 0xDC, 0x00, 0x00, 0x5D, 0x1F, 0x30, 0xC0, // 8
                0xF2, 0x26, 0x00, 0x00, 0x43, 0xFB, 0x00, 0xE1, // 8
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC1, 0xF0, // 8 * 7
                0x28,  
            };
#pragma warning (pop)

	PASSTHRU_MSG *pMessage = pMsg;
	pMessage->ProtocolId = pChannel->ProtocolId;
	pMessage->RxStatus = 1; // START_OF_MESSAGE
	pMessage->TxFlags =  0;
	pMessage->Timestamp = 0;
	pMessage->DataSize = ecuInitResponseLength;
	pMessage->ExtraDataIndex = pMessage->DataSize;

	for (int i = 0; i < ecuInitResponseLength; i++)
	{
		pMessage->Data[i] = ecuInitResponse[i];
	}

	*pNumMsgs = 1;

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruWriteMsgs(
	unsigned long ChannelId,
	PASSTHRU_MSG *pMsg,
	unsigned long *pNumMsgs,
	unsigned long Timeout)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruWriteMsgs\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pMsg = %08X\r\n", (unsigned long) pMsg);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pNumMsgs = %08X\r\n", (unsigned long) pNumMsgs);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    *pNumMsgs = %08X\r\n", *pNumMsgs);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    Timeout = %08X\r\n", Timeout);
	OutputDebugString(szMessage);

	for (unsigned long i = 0; i < *pNumMsgs; i++)
	{
		wsprintf(szMessage, L"Message %d\r\n", i);
		OutputDebugString(szMessage);

		pMsg[i].DumpMessage();
	}

	OutputDebugString(L"PassThruWriteMsgs done.\r\n");

	return 0;
}

J2534MOCK_API long CCONV PassThruStartPeriodicMsg(
	unsigned long ChannelId,
	PASSTHRU_MSG *pMsg,
	unsigned long *pMsgId,
	unsigned long TimeInterval)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruStartPeriodicMsg\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pMsg = %08X\r\n", (unsigned long) pMsg);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    *pMsgId = %08X\r\n", *pMsgId);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    TimeInterval = %08X\r\n", TimeInterval);
	OutputDebugString(szMessage);

	pMsg->DumpMessage();

	delete [] szMessage;
	*pMsgId = 0;
	return 0;
}

J2534MOCK_API long CCONV PassThruStopPeriodicMsg(
	unsigned long ChannelId,
	unsigned long MsgId)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruStopPeriodicMsg\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    MsgId = %08X\r\n", MsgId);
	OutputDebugString(szMessage);

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruStartMsgFilter(
	unsigned long ChannelId,
	unsigned long FilterType,
	PASSTHRU_MSG *pMaskMsg,
	PASSTHRU_MSG *pPatternMsg,
	PASSTHRU_MSG *pFlowControlMsg,
	unsigned long *pFilterId)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruStartMsgFilter\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    pMaskMsg = %08X\r\n", (unsigned long) pMaskMsg);
	OutputDebugString(szMessage);
	pMaskMsg->DumpMessage();

	wsprintf(szMessage, L"    pPatternMsg = %08X\r\n", (unsigned long) pPatternMsg);
	OutputDebugString(szMessage);
	pPatternMsg->DumpMessage();

	wsprintf(szMessage, L"    pFlowControlMsg = %08X\r\n", (unsigned long) pFlowControlMsg);
	OutputDebugString(szMessage);
	if (pFlowControlMsg)
	{
		pFlowControlMsg->DumpMessage();
	}

	wsprintf(szMessage, L"    pFilterId = %08X\r\n", pFilterId);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    *pFilterId = %08X\r\n", *pFilterId);
	OutputDebugString(szMessage);
	
	delete [] szMessage;
	*pFilterId = 0;
	return 0;
}

J2534MOCK_API long CCONV PassThruStopMsgFilter(
	unsigned long ChannelId,
	unsigned long FilterId)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruStopPeriodicMsg\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    FilterId = %08X\r\n", FilterId);
	OutputDebugString(szMessage);

	delete [] szMessage;
	return 0;
}

J2534MOCK_API long CCONV PassThruGetLastError(
	char *pErrorDescription)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruGetLastError\r\n");
	OutputDebugString(szMessage);
	///pErrorDescription = "No Error.";
	lstrcpyA("No Error.", pErrorDescription);
	return 0;
}

J2534MOCK_API long CCONV PassThruIoctl(
	unsigned long ChannelId,
	unsigned long IoctlId,
	void *pInput,
	void *pOutput)
{
	wchar_t *szMessage = new wchar_t[100]; 
	wsprintf(szMessage, L"PassThruIoctl\r\n");
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    ChannelId = %08X\r\n", ChannelId);
	OutputDebugString(szMessage);

	Channel *pChannel = (Channel*) ChannelId;
	wsprintf(szMessage, L"    Channel: %s\r\n", pChannel->szName);
	OutputDebugString(szMessage);

	wsprintf(szMessage, L"    IoctlId = %08X\r\n", IoctlId);
	OutputDebugString(szMessage);

	if (IoctlId == 0x02) // set_config
	{
		wsprintf(szMessage, L"ConfigList ptr: %08X\r\n", (unsigned long) pInput);
		OutputDebugString(szMessage);

		ConfigList *pConfigList = (ConfigList*) pInput;

		wsprintf(szMessage, L"ConfigList length: %d\r\n", (unsigned long) pConfigList->NumOfParams);
		OutputDebugString(szMessage);

		for (int i = 0; i < (int) pConfigList->NumOfParams; i++)
		{
			wsprintf(szMessage, L"Config #%d\r\n", i);
			OutputDebugString(szMessage);
			pConfigList->pArray[i].DumpConfig();
		}
	}

	delete [] szMessage;
	return 0;
}

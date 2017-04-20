﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NateW.J2534
{
    public interface IPassThru : IDisposable
    {
        /// <summary>
        /// Open a J2534 device
        /// </summary>
        /// <param name="pName">reserved for future use, must be null</param>
        /// <param name="pDeviceId">will be set to the id of the opened device</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruOpen(string pName, out UInt32 pDeviceId);

        /// <summary>
        /// Close a J2534 device
        /// </summary>
        /// <param name="DeviceId">id of the device to close</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruClose(UInt32 DeviceId);

        /// <summary>
        /// Opens a communication channel
        /// </summary>
        /// <param name="DeviceId">Returned from PassThruOpen</param>
        /// <param name="ProtocolId">See Protocol enumeration</param>
        /// <param name="Flags">See ConnectFlags enumeration</param>
        /// <param name="BaudRate">See BaudRate enumeration</param>
        /// <param name="pChannelID">Will be set to the id of the opened channel</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruConnect(
            UInt32 DeviceId,
            PassThruProtocol ProtocolId,
            PassThruConnectFlags Flags,
            PassThruBaudRate BaudRate,
            out UInt32 pChannelID);

        /// <summary>
        /// Closes a communication channel
        /// </summary>
        /// <param name="ChannelID">Channel ID provided by PassThruConnect</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruDisconnect(UInt32 ChannelID);

        /// <summary>
        /// Read messages and indications from the receive buffer.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Indicates how many message structures have been provided; on return, indicates how many messages were received.</param>
        /// <param name="Timeout">Read timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruReadMsg(
            UInt32 ChannelID,
            PassThruMsg pMsg,
            UInt32 Timeout);

        /// <summary>
        /// Read messages and indications from the receive buffer.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Indicates how many message structures have been provided; on return, indicates how many messages were received.</param>
        /// <param name="Timeout">Read timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruReadMsgs(
            UInt32 ChannelID,
            PassThruMsg[] pMsgs,
            ref UInt32 pNumMsgs,
            UInt32 Timeout);


        /// <summary>
        /// Send messages to the ECU.
        /// </summary>
        /// <remarks>
        /// This is a single-message hack to get around a marshaling problem w/ PassThruWriteMsgs.
        /// </remarks>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Pointer to number of messages to send.  On return, will indicate how many messages were sent before timeout expired (if timeout is nonzero) or how many messages were enqueued (if timeout is zero).</param>
        /// <param name="Timeout">Write timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruWriteMsg(
            UInt32 ChannelID,
            PassThruMsg pMsg,
            UInt32 Timeout);

        /// <summary>
        /// Send messages to the ECU.
        /// </summary>
        /// <remarks>
        /// DOES NOT MARSHAL THE MESSAGE ARRAY CORRECTLY.
        /// </remarks>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Pointer to number of messages to send.  On return, will indicate how many messages were sent before timeout expired (if timeout is nonzero) or how many messages were enqueued (if timeout is zero).</param>
        /// <param name="Timeout">Write timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruWriteMsgs(
            UInt32 ChannelID,
            PassThruMsg[] pMsg,
            ref UInt32 pNumMsgs,
            UInt32 Timeout);

        /// <summary>
        /// Immediately queue the given message, and re-send at the specified interval.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to a single message structure</param>
        /// <param name="pMsgId">Pointer to periodic-message identifier assigned by the PassThru DLL</param>
        /// <param name="TimeInterval">Interval between the start of successive transmissions, in milliseconds.  Valid range is 5-65535.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruStartPeriodicMsg(
            UInt32 ChannelID,
            PassThruMsg pMsg,
            out UInt32 pMsgId,
            UInt32 TimeInterval);

        /// <summary>
        /// Stop the given periodic message.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="MessageID">Periodic-message identifier returned by PassThruStartPeriodicMsg</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruStopPeriodicMsg(
            UInt32 ChannelID,
            UInt32 MessageID);

        /// <summary>
        /// Apply a filter to incoming messages.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="FilterType">See FilterType enumeration</param>
        /// <param name="pMaskMsg">This message will be bitwise-ANDed with incoming messages to mask irrelevant bits.</param>
        /// <param name="pPatternMsg">This message will be compared with the masked messsage; if equal the FilterType operation will be applied.</param>
        /// <param name="pFlowControlMsg">Must be null for Pass or Block filter types.  For FlowControl filters, points to the CAN ID used for segmented sends and receives.</param>
        /// <param name="pFilterID">Upon return, will be set with an ID for the newly applied filter.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruStartMsgFilter(
            UInt32 ChannelID,
            UInt32 FilterType,
            PassThruMsg pMaskMsg,
            PassThruMsg pPatternMsg,
            PassThruMsg pFlowControlMsg,
            out UInt32 pFilterID);

        /// <summary>
        /// Removes the given message filter.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="FilterID">Filter identifier returned from PassThruStartMsgFilter</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruStopMsgFilter(
            UInt32 ChannelID,
            UInt32 FilterID);
        
        /// <summary>
        /// Retreive version strings from the PassThru DLL.
        /// </summary>
        /// <param name="DeviceID">Device identifier returned from PassThruOpen</param>
        /// <param name="pFirmwareVersion">Firmware version string.  Allocate at least 80 characters.</param>
        /// <param name="pDllVersion">DLL version string.  Allocate at least 80 characters.</param>
        /// <param name="pApiVersion">API version string.  Allocate at least 80 characters.</param>
        /// <returns></returns>
        PassThruStatus PassThruReadVersion(
            UInt32 DeviceID,
            out string pFirmwareVersion,
            out string pDllVersion,
            out string pApiVersion);

        /// <summary>
        /// Retrieve error information regarding a previous PassThru API call.
        /// </summary>
        /// <param name="pErrorDescription">Pointer to error description buffer.  Allocate at least 80 characters.</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruGetLastError(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            out string pErrorDescription);

        /// <summary>
        /// IO Control
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="IoctlID">See IOCtl enumeration</param>
        /// <param name="pInput">Pointer to input structure</param>
        /// <param name="pOutput">Pointer to output structure</param>
        /// <returns>See Status enumeration</returns>
        PassThruStatus PassThruIoctl(
            UInt32 ChannelID,
            PassThruIOControl IoctlID,
            IntPtr pInput,
            IntPtr pOutput);
    }
}

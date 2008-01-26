﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Process: DebuggerObject, IExpirable
	{
		NDebugger debugger;
		
		ICorDebugProcess corProcess;
		ManagedCallback callbackInterface;
		
		#region IExpirable
		
		bool hasExpired = false;
		
		public event EventHandler Expired;
		
		public bool HasExpired {
			get {
				return hasExpired;
			}
		}
		
		internal void NotifyHasExpired()
		{
			if(!hasExpired) {
				hasExpired = true;
				if (Expired != null) {
					Expired(this, new ProcessEventArgs(this));
				}
				// Expire pause seesion first
				if (PauseSession != null) {
					ExpirePauseSession();
				}
				if (DebuggeeState != null) {
					ExpireDebuggeeState();
				}
			}
		}
		
		#endregion
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal ManagedCallback CallbackInterface {
			get {
				return callbackInterface;
			}
		}
		
		internal Process(NDebugger debugger, ICorDebugProcess corProcess)
		{
			this.debugger = debugger;
			this.corProcess = corProcess;
			
			this.callbackInterface = new ManagedCallback(this);
		}
		
		internal ICorDebugProcess CorProcess {
			get {
				return corProcess;
			}
		}
		
		static public Process CreateProcess(NDebugger debugger, string filename, string workingDirectory, string arguments)
		{
			return debugger.MTA2STA.Call<Process>(delegate{
			                                      	return StartInternal(debugger, filename, workingDirectory, arguments);
			                                      });
		}
		
		static unsafe Process StartInternal(NDebugger debugger, string filename, string workingDirectory, string arguments)
		{
			debugger.TraceMessage("Executing " + filename);
			
			uint[] processStartupInfo = new uint[17];
			processStartupInfo[0] = sizeof(uint) * 17;
			uint[] processInfo = new uint[4];
			
			ICorDebugProcess outProcess;
			
			if (workingDirectory == null || workingDirectory == "") {
				workingDirectory = System.IO.Path.GetDirectoryName(filename);
			}
			
			fixed (uint* pprocessStartupInfo = processStartupInfo)
				fixed (uint* pprocessInfo = processInfo)
					outProcess =
						debugger.CorDebug.CreateProcess(
							filename,   // lpApplicationName
							  // If we do not prepend " ", the first argument migh just get lost
							" " + arguments,                       // lpCommandLine
							ref _SECURITY_ATTRIBUTES.Default,                       // lpProcessAttributes
							ref _SECURITY_ATTRIBUTES.Default,                      // lpThreadAttributes
							1,//TRUE                    // bInheritHandles
							0x00000010 /*CREATE_NEW_CONSOLE*/,    // dwCreationFlags
							IntPtr.Zero,                       // lpEnvironment
							workingDirectory,                       // lpCurrentDirectory
							(uint)pprocessStartupInfo,        // lpStartupInfo
							(uint)pprocessInfo,               // lpProcessInformation,
							CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS   // debuggingFlags
							);
			
			return new Process(debugger, outProcess);
		}
		
		public string DebuggeeVersion {
			get {
				return debugger.DebuggeeVersion;
			}
		}
		
		public StackFrame SelectedStackFrame {
			get {
				if (SelectedThread == null) {
					return null;
				} else {
					return SelectedThread.SelectedStackFrame;
				}
			}
		}
		
		/// <summary>
		/// Fired when System.Diagnostics.Trace.WriteLine() is called in debuged process
		/// </summary>
		public event EventHandler<MessageEventArgs> LogMessage;
		
		protected internal virtual void OnLogMessage(MessageEventArgs arg)
		{
			TraceMessage ("Debugger event: OnLogMessage");
			if (LogMessage != null) {
				LogMessage(this, arg);
			}
		}
		
		public void TraceMessage(string message, params object[] args)
		{
			TraceMessage(string.Format(message, args));
		}
		
		public void TraceMessage(string message)
		{
			System.Diagnostics.Debug.WriteLine("Debugger:" + message);
			debugger.OnDebuggerTraceMessage(new MessageEventArgs(this, message));
		}
		
		public SourcecodeSegment NextStatement { 
			get {
				if (SelectedStackFrame == null || IsRunning) {
					return null;
				} else {
					return SelectedStackFrame.NextStatement;
				}
			}
		}
	}
	
	[Serializable]
	public class ProcessEventArgs: DebuggerEventArgs
	{
		Process process;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		public ProcessEventArgs(Process process): base(process == null ? null : process.Debugger)
		{
			this.process = process;
		}
	}
}

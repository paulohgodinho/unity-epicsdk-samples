// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTC
{
	/// <summary>
	/// This struct is used to call <see cref="RTCInterface.AddNotifyDisconnected" />.
	/// </summary>
	public class AddNotifyDisconnectedOptions
	{
		/// <summary>
		/// The Product User ID of the user trying to request this operation.
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The room this event is registered on.
		/// </summary>
		public string RoomName { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct AddNotifyDisconnectedOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_RoomName;

		public ProductUserId LocalUserId
		{
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public string RoomName
		{
			set
			{
				Helper.TryMarshalSet(ref m_RoomName, value);
			}
		}

		public void Set(AddNotifyDisconnectedOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = RTCInterface.AddnotifydisconnectedApiLatest;
				LocalUserId = other.LocalUserId;
				RoomName = other.RoomName;
			}
		}

		public void Set(object other)
		{
			Set(other as AddNotifyDisconnectedOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_LocalUserId);
			Helper.TryMarshalDispose(ref m_RoomName);
		}
	}
}
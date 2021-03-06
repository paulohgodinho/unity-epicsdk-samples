// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
	/// <summary>
	/// Input parameters for the <see cref="ConnectInterface.CopyProductUserExternalAccountByAccountId" /> function.
	/// </summary>
	public class CopyProductUserExternalAccountByAccountIdOptions
	{
		/// <summary>
		/// The Product User ID to look for when copying external account info from the cache.
		/// </summary>
		public ProductUserId TargetUserId { get; set; }

		/// <summary>
		/// External auth service account ID to look for when copying external account info from the cache.
		/// </summary>
		public string AccountId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct CopyProductUserExternalAccountByAccountIdOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_TargetUserId;
		private System.IntPtr m_AccountId;

		public ProductUserId TargetUserId
		{
			set
			{
				Helper.TryMarshalSet(ref m_TargetUserId, value);
			}
		}

		public string AccountId
		{
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public void Set(CopyProductUserExternalAccountByAccountIdOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyaccountidApiLatest;
				TargetUserId = other.TargetUserId;
				AccountId = other.AccountId;
			}
		}

		public void Set(object other)
		{
			Set(other as CopyProductUserExternalAccountByAccountIdOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_TargetUserId);
			Helper.TryMarshalDispose(ref m_AccountId);
		}
	}
}
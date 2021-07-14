using System;
using UnityEngine;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;

public class EpicPlatform : MonoBehaviour
{
    private static EpicPlatform _instance;
    public static EpicPlatform Instance {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EpicPlatform>();
            }

            return _instance; 
        }
    }

    public event Action OnAfterPlatformReleasedAndShutdown;
    public event Action OnAuthenticatedWithEpic;
    public event Action<ProductUserId> OnConnectedToEpicOnlineServices;
    
    [SerializeField]
    private string ProductName = "";
    [SerializeField]
    private string ProductVersion = "1.0";
    [SerializeField]
    private string ProductId = "";
    [SerializeField]
    private string SandboxId = "";
    [SerializeField]
    private string DeploymentId = "";
    [SerializeField]
    private string ClientId = "";
    [SerializeField]
    private string ClientSecret = "";

    private enum DeveloperLoginMode
    {
        DevAuthTool,
        AuthPortal
    }

    [Header("Developer Login")] [SerializeField]
    private DeveloperLoginMode LoginMode = DeveloperLoginMode.DevAuthTool;

    [SerializeField] 
    private string DevAuthToolPort = "";
    [SerializeField]
    private string DevAuthToolCredentialName = "";

    [Header("Logs")] 
    [SerializeField] 
    private bool EnableLogs = false;
    [SerializeField] 
    private LogCategory LogCategory = LogCategory.AllCategories;
    [SerializeField] private LogLevel LogLevel = LogLevel.VeryVerbose;

    public EpicAccountId EpicLocalUserId { get; private set; }
    public PlatformInterface PlatformInterface { get; private set; }
    private bool IsPlatformInterfaceInitialized { get; set; }

    private void Start()
    {
        OnAuthenticatedWithEpic += LoginToEpicOnlineServices;
        
        InitializePlatformInterface();
        SetupInternalSDKLogging();
        CreatePlatformInterface();
        LoginToAuthInterface();
    }

    private void InitializePlatformInterface()
    {
        var initializeOptions = new InitializeOptions()
        {
            ProductName = ProductName,
            ProductVersion = ProductVersion
        };

        var initializeResult = PlatformInterface.Initialize(initializeOptions);
        Log($"Initialized Result: {initializeResult}");

        // If you initialize the platform from a Native Plugin, it will return the Result as AlreadyConfigured
        if (initializeResult != Result.Success)
        {
            if (initializeResult != Result.AlreadyConfigured)
            {
                throw new System.Exception($"Failed to initialize platform: {initializeResult}");
            }
        }

        IsPlatformInterfaceInitialized = true;
    }

    private void SetupInternalSDKLogging()
    {
        if (EnableLogs == false)
        {
            return;
        }

        LoggingInterface.SetLogLevel(LogCategory, LogLevel);
        LoggingInterface.SetCallback(logMessage => { Debug.Log("<b>[EpicSDKInternalLog]</b> " + logMessage.Message); });
    }

    private void CreatePlatformInterface()
    {
        var options = new Options()
        {
            ProductId = ProductId,
            SandboxId = SandboxId,
            DeploymentId = DeploymentId,
            ClientCredentials = new ClientCredentials()
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret
            },
            Flags = PlatformFlags.WindowsEnableOverlayOpengl | PlatformFlags.WindowsEnableOverlayD3D9 |
                    PlatformFlags.WindowsEnableOverlayD3D10
        };

        PlatformInterface = PlatformInterface.Create(options);
        if (PlatformInterface == null)
        {
            throw new System.Exception("Failed to create Platform Interface");
        }
    }

    private void SetCredentialsTypeAndId(out LoginCredentialType loginCredentialType, out string loginCredentialId,
        out string loginCredentialToken)
    {
        if (Application.isEditor)
        {
            if (LoginMode == DeveloperLoginMode.DevAuthTool)
            {
                loginCredentialType = LoginCredentialType.Developer;
                loginCredentialId = DevAuthToolPort;
                loginCredentialToken = DevAuthToolCredentialName;
                return;
            }

            if (LoginMode == DeveloperLoginMode.AuthPortal)
            {
                loginCredentialType = LoginCredentialType.AccountPortal;
                loginCredentialId = null;
                loginCredentialToken = null;
                return;
            }
        }

        loginCredentialType = LoginCredentialType.ExchangeCode; // ExchangeCode is EGS Login Method
        loginCredentialId = null;
        loginCredentialToken = null;

        var args = System.Environment.GetCommandLineArgs();
        foreach (var arg in args)
        {
            if (arg.Contains("-AUTH_PASSWORD"))
            {
                var splitArg = arg.Split('=');
                Log("Setting Token to: " + splitArg[1]);
                loginCredentialToken = splitArg[1];
            }
        }
    }

    private void LoginToAuthInterface()
    {
        LoginCredentialType loginCredentialType;
        string loginCredentialId;
        string loginCredentialToken;

        SetCredentialsTypeAndId(out loginCredentialType, out loginCredentialId, out loginCredentialToken);

        var authInterface = PlatformInterface.GetAuthInterface();
        if (authInterface == null)
        {
            throw new Exception("Failed to get auth interface");
        }

        var loginOptions = new Epic.OnlineServices.Auth.LoginOptions()
        {
            Credentials = new Epic.OnlineServices.Auth.Credentials()
            {
                Type = loginCredentialType,
                Id = loginCredentialId,
                Token = loginCredentialToken
            },
            ScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList
        };
        
        authInterface.Login(loginOptions, null, loginCallbackInfo =>
        {
            if (loginCallbackInfo.ResultCode == Result.Success)
            {
                Log("Auth Interface Login succeeded");
                EpicLocalUserId = loginCallbackInfo.LocalUserId;
                
                OnAuthenticatedWithEpic?.Invoke();
            }
            else
            {
                throw new Exception($"Failed to Login Auth Interface {loginCallbackInfo.ResultCode}");
            }
        });
    }
    
    private void Update()
    {
        if (IsPlatformInterfaceInitialized == true)
        {
            PlatformInterface.Tick();
        }
    }

    private void LoginToEpicOnlineServices()
    {
        PlatformInterface.GetAuthInterface()
            .CopyUserAuthToken(new CopyUserAuthTokenOptions(), EpicLocalUserId, out var token);
        
        var connectInterface = PlatformInterface.GetConnectInterface();
        
        var credentials = new Epic.OnlineServices.Connect.Credentials()
        {
            Type = ExternalCredentialType.Epic,
            Token = token.AccessToken
        };
        
        var loginOptions = new Epic.OnlineServices.Connect.LoginOptions()
        {
            Credentials = credentials
        };
        
        Log("Attempting to login to EpicOnlineServices platform");
        connectInterface.Login(loginOptions, null, data =>
        {
            if (data.ResultCode == Result.Success)
            {
                Log("Logged in to EpicOnlineServices");
                OnConnectedToEpicOnlineServices?.Invoke(data.LocalUserId);
                return;
            }

            Log($"Failed to login to EpicOnlineServices with Result: {data.ResultCode}, attempting to create account");
            CreateEpicOnlineServicesAccount(data.ContinuanceToken);
        });
    }

    private void CreateEpicOnlineServicesAccount(ContinuanceToken continuanceToken)
    {
        var connectInterface = PlatformInterface.GetConnectInterface();
        
        connectInterface.CreateUser(new CreateUserOptions(){ContinuanceToken = continuanceToken}, null, data =>
        {
            if (data.ResultCode == Result.Success)
            {
                Log("Created EpicOnlineServices User and Logged In");
                OnConnectedToEpicOnlineServices?.Invoke(data.LocalUserId);
                return;
            }
            
            Log($"Failed to create EpicOnlineServices User. Result: {data.ResultCode}");
        });
    }
    
    private void OnApplicationQuit()
    {
        if (PlatformInterface == null)
        {
            OnAfterPlatformReleasedAndShutdown?.Invoke();
            return;
        }
        
        PlatformInterface.Release();
        PlatformInterface = null;
        PlatformInterface.Shutdown();
        
        OnAfterPlatformReleasedAndShutdown?.Invoke();
    }

    private void Log(string log)
    {
        Debug.Log($"<b>[EpicPlatform]</b> {log}");
    }
}